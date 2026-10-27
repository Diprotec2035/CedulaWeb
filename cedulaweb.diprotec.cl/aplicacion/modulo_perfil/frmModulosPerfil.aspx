<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmModulosPerfil.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.modulo_perfil.frmModulosPerfil" %>
<%@ Register Src="~/aplicacion/control/ucBitacora.ascx" TagPrefix="uc" TagName="Bitacora" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        function cambiarToggleModuloPerfil(toggle) {
            if (!toggle) {
                return false;
            }

            var hiddenId = toggle.getAttribute("data-hidden-id");
            var modId = toggle.getAttribute("data-mod-id");
            var accion = toggle.getAttribute("data-accion");
            var updatedAtOriginal = toggle.getAttribute("data-updated-at") || "";

            if (!hiddenId || !modId || !accion) {
                return false;
            }

            var hidden = document.getElementById(hiddenId);

            if (!hidden) {
                return false;
            }

            var valorEncendido = toggle.getAttribute("data-valor-encendido");
            var valorApagado = toggle.getAttribute("data-valor-apagado");

            if (!valorEncendido) {
                valorEncendido = "0";
            }

            if (!valorApagado) {
                valorApagado = "1";
            }

            if (toggle.classList.contains("on")) {
                toggle.classList.remove("on");
                hidden.value = valorApagado;
            }
            else {
                toggle.classList.add("on");
                hidden.value = valorEncendido;
            }

            document.getElementById("<%=hfPermisoPerId.ClientID%>").value = document.getElementById("<%=ddlPerfil.ClientID%>").value;
            document.getElementById("<%=hfPermisoCatId.ClientID%>").value = document.getElementById("<%=ddlCategoria.ClientID%>").value;
            document.getElementById("<%=hfPermisoMpId.ClientID%>").value = toggle.getAttribute("data-mp-id") || "0";
            document.getElementById("<%=hfPermisoModId.ClientID%>").value = modId;
            document.getElementById("<%=hfPermisoAccion.ClientID%>").value = accion;
            document.getElementById("<%=hfPermisoValor.ClientID%>").value = hidden.value;
            document.getElementById("<%=hfPermisoUpdatedAtOriginal.ClientID%>").value = updatedAtOriginal;

            __doPostBack("ActualizarPermisoModulo", "");

            return false;
        }

        function abrirBitacora() {
            <%= ucBitacora.GetScriptAbrir() %>
            return false;
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Main Content-->
    <div class="main-content side-content pt-0">
        <div class="side-app">

            <asp:HiddenField ID="hfPermisoPerId" runat="server" />
            <asp:HiddenField ID="hfPermisoCatId" runat="server" />
            <asp:HiddenField ID="hfPermisoModId" runat="server" />
            <asp:HiddenField ID="hfPermisoAccion" runat="server" />
            <asp:HiddenField ID="hfPermisoValor" runat="server" />
            <asp:HiddenField ID="hfPermisoMpId" runat="server" />
            <asp:HiddenField ID="hfPermisoUpdatedAtOriginal" runat="server" />

            <div class="main-container container-fluid">
                <!-- Page Header -->
                <div class="page-header mb-2 mt-2">
                    <div>
                        <ol class="breadcrumb mb-0">
                            <li class="breadcrumb-item">
                                <a href="<%=ConstantesModelo.ConsModulosDefault%>">Inicio</a>
                            </li>
                            <li class="breadcrumb-item active text-primary" aria-current="page">
                                <%:ModuloContextoHelper.ObtenerTituloPagina(this)%>
                            </li>
                        </ol>
                    </div>
                </div>
                <!-- End Page Header -->

                <!-- Filtros -->
                <asp:Panel ID="pnlFiltros" runat="server" DefaultButton="btnBuscar">
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <div class="card custom-card">
                                <div class="card-body py-3">
                                    <div class="row row-sm align-items-end">

                                        <div class="col-xl-4 col-lg-4 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Perfil</label>
                                            <asp:DropDownList
                                                ID="ddlPerfil"
                                                runat="server"
                                                CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-4 col-lg-4 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Categoría</label>
                                            <asp:DropDownList
                                                ID="ddlCategoria"
                                                runat="server"
                                                CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-1 col-lg-0 d-none d-xl-block mb-2"></div>

                                        <div class="col-xl-3 col-lg-4 col-md-12 col-sm-12 mb-2">
                                            <div class="row row-sm">
                                                <div class="col-6">
                                                    <asp:Button
                                                        ID="btnBuscar"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-block"
                                                        Text="Buscar"
                                                        OnClick="btnBuscar_Click" />
                                                </div>

                                                <div class="col-6">
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <!-- End Filtros -->

                <!-- Tabla -->
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card custom-card">
                            <div class="card-header">
                                <h6 class="card-title"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>
                            </div>

                            <div class="card-body">
                                <div class="table-responsive p-0">

                                    <asp:GridView
                                        ID="gvListado"
                                        runat="server"
                                        AutoGenerateColumns="False"
                                        DataKeyNames="Id,IdModulo"
                                        OnRowDataBound="gvListado_RowDataBound"
                                        OnRowCreated="gvListado_RowCreated"
                                        OnRowCommand="gvListado_RowCommand"
                                        AllowPaging="True"
                                        PageSize="8"
                                        OnPageIndexChanging="gvListado_PageIndexChanging"
                                        CssClass="table table-hover text-start mg-b-0 border"
                                        Style="table-layout: fixed; width: 100%;"
                                        GridLines="None"
                                        PagerSettings-Mode="NumericFirstLast"
                                        PagerSettings-FirstPageText="Primera"
                                        PagerSettings-LastPageText="Última"
                                        PagerStyle-HorizontalAlign="Center">

                                        <EmptyDataRowStyle CssClass="text-center tx-14 text-muted" />

                                        <Columns>
                                            <asp:BoundField HeaderText="Módulo" DataField="IdModulo">
                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Nombre" DataField="NombreModulo">
                                                <HeaderStyle Width="35%" />
                                                <ItemStyle Width="35%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Módulo visible">
                                                <HeaderStyle CssClass="text-center" Width="15%" />
                                                <ItemStyle CssClass="text-center" Width="15%" />
                                                <ItemTemplate>
                                                    <asp:HiddenField
                                                        ID="hfHabilitado"
                                                        runat="server"
                                                        Value='<%# Convert.ToInt16(Eval("Estado")) %>' />

                                                    <div id="tglHabilitado"
                                                         runat="server"
                                                         class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                             ? "main-toggle on br-3 text-center mx-auto"
                                                             : "main-toggle br-3 text-center mx-auto" %>'>
                                                        <span></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%--
                                            <asp:TemplateField HeaderText="Permite consultas">
                                                <HeaderStyle CssClass="text-center" Width="15%" />
                                                <ItemStyle CssClass="text-center" Width="15%" />
                                                <ItemTemplate>
                                                    <asp:HiddenField
                                                        ID="hfConsulta"
                                                        runat="server"
                                                        Value='<%# Convert.ToInt16(Eval("Consulta")) %>' />

                                                    <div id="tglConsulta"
                                                         runat="server"
                                                         class='<%# Convert.ToInt16(Eval("Consulta")) == 0
                                                             ? "main-toggle on br-3 text-center mx-auto"
                                                             : "main-toggle br-3 text-center mx-auto" %>'>
                                                        <span></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            --%>

                                            <asp:TemplateField HeaderText="Nuevo elemento">
                                                <HeaderStyle CssClass="text-center" Width="15%" />
                                                <ItemStyle CssClass="text-center" Width="15%" />
                                                <ItemTemplate>
                                                    <asp:HiddenField
                                                        ID="hfNuevo"
                                                        runat="server"
                                                        Value='<%# Convert.ToInt16(Eval("Nuevo")) %>' />

                                                    <div id="tglNuevo"
                                                         runat="server"
                                                         class='<%# Convert.ToInt16(Eval("Nuevo")) == 0
                                                             ? "main-toggle on br-3 text-center mx-auto"
                                                             : "main-toggle br-3 text-center mx-auto" %>'>
                                                        <span></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Permite modificar">
                                                <HeaderStyle CssClass="text-center" Width="15%" />
                                                <ItemStyle CssClass="text-center" Width="15%" />
                                                <ItemTemplate>
                                                    <asp:HiddenField
                                                        ID="hfModificar"
                                                        runat="server"
                                                        Value='<%# Convert.ToInt16(Eval("Modifica")) %>' />

                                                    <div id="tglModificar"
                                                         runat="server"
                                                         class='<%# Convert.ToInt16(Eval("Modifica")) == 0
                                                             ? "main-toggle on br-3 text-center mx-auto"
                                                             : "main-toggle br-3 text-center mx-auto" %>'>
                                                        <span></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Bitácora">
                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />
                                                <ItemTemplate>
                                                    <asp:LinkButton
                                                        ID="btnBitacora"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-sm"
                                                        CommandName="Bitacora"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        CausesValidation="false"
                                                        ToolTip="Ver bitácora">
                                                        <i class="fe fe-eye"></i>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>

                                    </asp:GridView>

                                    <uc:Bitacora ID="ucBitacora" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Tabla -->
            </div>
        </div>
    </div>
    <!-- End Main Content-->

</asp:Content>