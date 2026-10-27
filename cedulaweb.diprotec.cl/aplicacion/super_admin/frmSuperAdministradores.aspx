<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmSuperAdministradores.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.super_admin.frmSuperAdministradores" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        var rutSuperAdminMFAPendiente = "";

        $(document).ready(function () {
            $("#<%=txtCoincidencia.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaUsuario)%>");
        });

        function abrirConfirmacionRestablecerMFA(rutSuperAdmin) {
            if (!rutSuperAdmin) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "No fue posible identificar el super administrador seleccionado.");
                return false;
            }

            rutSuperAdminMFAPendiente = rutSuperAdmin;

            $("#rutSuperAdminRestablecerMFA").text(rutSuperAdmin);
            $("#modalConfirmarRestablecerMFA").modal("show");

            return false;
        }

        function confirmarRestablecimientoMFA() {
            if (!rutSuperAdminMFAPendiente) {
                $("#modalConfirmarRestablecerMFA").modal("hide");
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "No fue posible identificar el super administrador seleccionado.");
                return false;
            }

            $("#<%=hfRutSuperAdminMFA.ClientID%>").val(rutSuperAdminMFAPendiente);

            $("#modalConfirmarRestablecerMFA").modal("hide");
            $("#<%=btnRestablecerMFA.ClientID%>").click();

            return false;
        }

        $(document).on("hidden.bs.modal", "#modalConfirmarRestablecerMFA", function () {
            rutSuperAdminMFAPendiente = "";
            $("#rutSuperAdminRestablecerMFA").text("");
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
                                                MaxLength="100"
                                                placeholder="RUT, nombre, apellido o email"
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

                                                <asp:ListItem Text="RUT" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Nombre" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Apellido" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Email" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Estado" Value="5"></asp:ListItem>

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
                                        DataKeyNames="RutAdmin"
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

                                            <asp:BoundField HeaderText="RUT" DataField="RutAdmin">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Nombre" DataField="NombreAdmin">
                                                <HeaderStyle Width="18%" />
                                                <ItemStyle Width="18%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Apellido" DataField="ApellidoAdmin">
                                                <HeaderStyle Width="18%" />
                                                <ItemStyle Width="18%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Email" DataField="Email">
                                                <HeaderStyle Width="22%" />
                                                <ItemStyle Width="22%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Teléfono" DataField="Telefono">
                                                <HeaderStyle Width="12%" />
                                                <ItemStyle Width="12%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Estado">

                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />

                                                <ItemTemplate>

                                                    <span class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>

                                                        <i class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                            ? "fe fe-check-circle me-1"
                                                            : "fe fe-x-circle me-1" %>'></i>

                                                        <%# Convert.ToInt16(Eval("Estado")) == 0 ? "Vigente" : "Anulado" %>

                                                    </span>

                                                </ItemTemplate>

                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Acciones">

                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />

                                                <ItemTemplate>

                                                    <asp:HyperLink
                                                        ID="lnkEditar"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-sm"
                                                        ToolTip="Editar">
                                                        <i class="fe fe-edit"></i>
                                                    </asp:HyperLink>

                                                    <asp:LinkButton
                                                        ID="lnkRestablecerMFA"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-warning btn-sm ms-1"
                                                        ToolTip="Restablecer MFA"
                                                        CausesValidation="false">
                                                        <i class="fe fe-refresh-cw"></i>
                                                    </asp:LinkButton>

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

    <asp:HiddenField ID="hfRutSuperAdminMFA" runat="server" />

    <asp:Button
        ID="btnRestablecerMFA"
        runat="server"
        CssClass="d-none"
        Text="Restablecer MFA"
        CausesValidation="false"
        OnClick="btnRestablecerMFA_Click" />

    <!-- Confirmar Restablecimiento MFA -->
    <div
        class="modal fade"
        id="modalConfirmarRestablecerMFA"
        tabindex="-1"
        role="dialog"
        aria-labelledby="tituloConfirmarRestablecerMFA"
        aria-hidden="true"
        data-bs-backdrop="static"
        data-bs-keyboard="false">

        <div class="modal-dialog modal-dialog-centered" role="document">

            <div class="modal-content">

                <div class="modal-header">

                    <h6 class="modal-title" id="tituloConfirmarRestablecerMFA">
                        Restablecer MFA
                    </h6>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Cerrar">
                    </button>

                </div>

                <div class="modal-body">

                    <div class="text-center">

                        <i class="fe fe-alert-triangle text-warning fs-40 mb-3"></i>

                        <p class="mb-3">
                            Se restablecerá la autenticación multifactor del super administrador
                            <strong id="rutSuperAdminRestablecerMFA"></strong>.
                        </p>

                        <p class="text-muted mb-0">
                            La aplicación de autenticación actual y todos los códigos de recuperación quedarán invalidados inmediatamente. En su próximo acceso el super administrador deberá configurar MFA nuevamente.
                        </p>

                    </div>

                </div>

                <div class="modal-footer justify-content-center">

                    <button
                        type="button"
                        class="btn btn-outline-secondary"
                        data-bs-dismiss="modal">
                        Cancelar
                    </button>

                    <button
                        type="button"
                        class="btn btn-main-primary"
                        onclick="return confirmarRestablecimientoMFA();">
                        Confirmar
                    </button>

                </div>

            </div>

        </div>

    </div>
    <!-- Fin Confirmar Restablecimiento MFA -->

</asp:Content>