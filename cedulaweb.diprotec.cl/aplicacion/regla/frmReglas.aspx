<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmReglas.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.regla.frmReglas" %>
<%@ Import Namespace="Modelo" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%=txtCoincidencia.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaRegla)%>");
        });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Main Content-->
    <div class="main-content side-content pt-0">
        <div class="side-app">

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

                                        <div class="col-xl-3 col-lg-4 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Coincidencia</label>
                                            <asp:TextBox
                                                ID="txtCoincidencia"
                                                runat="server"
                                                CssClass="form-control"
                                                MaxLength="50"
                                                placeholder="nombre"
                                                data-filtro-sesion="Coincidencia">
                                            </asp:TextBox>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Estado</label>
                                            <asp:DropDownList
                                                ID="ddlEstado"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="Estado">
                                                <asp:ListItem Text="Todos" Value="2" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Vigente" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Anulado" Value="1"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Ordenar por</label>
                                            <asp:DropDownList
                                                ID="ddlOrdenarPor"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="OrdenarPor">
                                                <asp:ListItem Text="Código" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Nombre" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Eliminar enviados" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Estado" Value="4"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Dirección</label>
                                            <asp:DropDownList
                                                ID="ddlDireccion"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="Direccion">
                                                <asp:ListItem Text="Ascendente" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Descendente" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

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
                                                    <asp:Button
                                                        ID="btnNuevo"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-block"
                                                        Text="Nuevo"
                                                        OnClick="btnNuevo_Click" />
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
                                        DataKeyNames="Id"
                                        OnRowDataBound="gvListado_RowDataBound"
                                        OnRowCreated="gvListado_RowCreated"
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
                                            <asp:BoundField HeaderText="ID" DataField="Id">
                                                <HeaderStyle Width="5%" CssClass="text-center" />
                                                <ItemStyle Width="5%" CssClass="text-center" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Nombre" DataField="Nombre">
                                                <HeaderStyle Width="18%" />
                                                <ItemStyle Width="18%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Pide nombre">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("NombreApellido")) == 1
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("NombreApellido")) == 1
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("NombreApellido")) == 1 ? "Sí" : "No" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Pide empresa">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Empresa")) == 1
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("Empresa")) == 1
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("Empresa")) == 1 ? "Sí" : "No" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Pide patente">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Patente")) == 1
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("Patente")) == 1
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("Patente")) == 1 ? "Sí" : "No" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Pide comentario">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Comentario")) == 1
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("Comentario")) == 1
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("Comentario")) == 1 ? "Sí" : "No" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Pide fotografía">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Fotografia")) == 1
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("Fotografia")) == 1
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("Fotografia")) == 1 ? "Sí" : "No" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Entrada / salida">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("EntradaSalida")) == 1
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("EntradaSalida")) == 1
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("EntradaSalida")) == 1 ? "Sí" : "No" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Eliminar enviados">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("EliminaEnviados")) == 1
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("EliminaEnviados")) == 1
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("EliminaEnviados")) == 1 ? "Sí" : "No" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Estado">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Eval("EstadoNombre") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Acciones">
                                                <HeaderStyle CssClass="text-center" />
                                                <ItemStyle CssClass="text-center" />
                                                <ItemTemplate>
                                                    <asp:HyperLink
                                                        ID="lnkEditar"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-sm"
                                                        ToolTip="Editar">
                                                        <i class="fe fe-edit"></i>
                                                    </asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>

                                    </asp:GridView>
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
