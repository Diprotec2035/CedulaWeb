<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmEmpresas.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.empresa.frmEmpresas" %>
<%@ Register Src="~/aplicacion/control/ucBitacora.ascx" TagPrefix="uc" TagName="Bitacora" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">         
        $(document).ready(function () {
            $("#<%=txtCoincidencia.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaEmpresa)%>");
            $("#<%=txtMotivoRevocacion.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoObservacion)%>");

            $(document).on("hidden.bs.modal", "#modalTokenGenerado", function () {
                $("#txtTokenGenerado").val("");
                $("#lblTokenVencimiento").text("");
            });
        });

        function abrirModalGenerarToken(rutEmpresa, nombreEmpresa) {
            $("#<%=hfTokenEmpresaRut.ClientID%>").val(rutEmpresa);
            $("#<%=hfTokenId.ClientID%>").val("");
            $("#lblEmpresaGenerarToken").text(nombreEmpresa);

            mostrarModalBootstrap("modalGenerarToken");
        }

        function confirmarGenerarToken() {
            cerrarModalBootstrap("modalGenerarToken");
            $("#global-loader").show();
            document.getElementById("<%=btnGenerarTokenOculto.ClientID%>").click();
        }

        function abrirModalRenovarToken(rutEmpresa, nombreEmpresa) {
            $("#<%=hfTokenEmpresaRut.ClientID%>").val(rutEmpresa);
            $("#<%=hfTokenId.ClientID%>").val("");
            $("#lblEmpresaRenovarToken").text(nombreEmpresa);

            mostrarModalBootstrap("modalRenovarToken");
        }

        function confirmarRenovarToken() {
            cerrarModalBootstrap("modalRenovarToken");
            $("#global-loader").show();
            document.getElementById("<%=btnRenovarTokenOculto.ClientID%>").click();
        }

        function abrirModalRevocarToken(rutEmpresa, tokenId, limpiarMotivo, nombreEmpresa) {
            $("#<%=hfTokenEmpresaRut.ClientID%>").val(rutEmpresa);
            $("#<%=hfTokenId.ClientID%>").val(tokenId);

            if (limpiarMotivo) {
                $("#<%=txtMotivoRevocacion.ClientID%>").val("");
            }

            if (nombreEmpresa) {
                $("#lblEmpresaRevocarToken").text(nombreEmpresa);
            }

            mostrarModalBootstrap("modalRevocarToken");
        }

        function confirmarRevocarToken() {
            var motivo = $("#<%=txtMotivoRevocacion.ClientID%>").val().trim();

            if (motivo.length === 0) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorWarning%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionEmpresaTokenMotivoRevocacionVacio)%>");
                return;
            }

            cerrarModalBootstrap("modalRevocarToken");
            $("#global-loader").show();
            document.getElementById("<%=btnRevocarTokenOculto.ClientID%>").click();
        }

        function mostrarModalTokenGenerado(token, fechaVencimiento) {
            $("#txtTokenGenerado").val(token);
            $("#lblTokenVencimiento").text(fechaVencimiento);

            mostrarModalBootstrap("modalTokenGenerado");
        }

        function copiarTokenGenerado() {
            var token = $("#txtTokenGenerado").val().trim();

            if (token.length === 0) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorWarning%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionEmpresaTokenCopiarVacio)%>");
                return;
            }

            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(token).then(function () {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsExitoEmpresaTokenCopiar)%>");
                }).catch(function () {
                    copiarTokenGeneradoFallback(token);
                });

                return;
            }

            copiarTokenGeneradoFallback(token);
        }

        function copiarTokenGeneradoFallback(token) {
            var inputTemporal = document.createElement("textarea");
            inputTemporal.value = token;
            inputTemporal.setAttribute("readonly", "");
            inputTemporal.style.position = "absolute";
            inputTemporal.style.left = "-9999px";

            document.body.appendChild(inputTemporal);
            inputTemporal.select();
            inputTemporal.setSelectionRange(0, inputTemporal.value.length);

            try {
                document.execCommand("copy");
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsExitoEmpresaTokenCopiar)%>");
            }
            catch (e) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorEmpresaTokenCopiar)%>");
            }

            document.body.removeChild(inputTemporal);
        }

        function mostrarModalBootstrap(modalId) {
            if (typeof bootstrap === "undefined") {
                setTimeout(function () {
                    mostrarModalBootstrap(modalId);
                }, 100);

                return;
            }

            var modalElemento = document.getElementById(modalId);
            var modalExistente = bootstrap.Modal.getInstance(modalElemento);

            if (modalExistente) {
                modalExistente.dispose();
            }

            var modal = new bootstrap.Modal(modalElemento, {
                backdrop: "static",
                keyboard: false
            });

            modal.show();
        }

        function cerrarModalBootstrap(modalId) {
            if (typeof bootstrap === "undefined") {
                return;
            }

            var modalElemento = document.getElementById(modalId);
            var modal = bootstrap.Modal.getInstance(modalElemento);

            if (modal) {
                modal.hide();
            }
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
                                            <asp:TextBox ID="txtCoincidencia" runat="server" CssClass="form-control" MaxLength="50" placeholder="nombre" data-filtro-sesion="Coincidencia"></asp:TextBox>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Estado</label>
                                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-control" data-filtro-sesion="Estado">
                                                <asp:ListItem Text="Todos" Value="2" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Vigente" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Anulado" Value="1"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Ordenar por</label>
                                            <asp:DropDownList ID="ddlOrdenarPor" runat="server" CssClass="form-control" data-filtro-sesion="OrdenarPor">
                                                <asp:ListItem Text="RUT" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Nombre" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Direcci&oacute;n" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Ciudad" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Comuna" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Dispositivo Externo" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Estado" Value="7"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Dirección</label>
                                            <asp:DropDownList ID="ddlDireccion" runat="server" CssClass="form-control" data-filtro-sesion="Direccion">
                                                <asp:ListItem Text="Ascendente" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Descendente" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-3 col-lg-4 col-md-12 col-sm-12 mb-2">
                                            <div class="row row-sm">
                                                <div class="col-6">
                                                    <asp:Button ID="btnBuscar" runat="server" CssClass="btn ripple btn-outline-info btn-block" Text="Buscar" OnClick="btnBuscar_Click" />
                                                </div>

                                                <div class="col-6">
                                                    <asp:Button ID="btnNuevo" runat="server" CssClass="btn ripple btn-outline-info btn-block" Text="Nuevo" OnClick="btnNuevo_Click" />
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

                                    <asp:HiddenField ID="hfTokenEmpresaRut" runat="server" />
                                    <asp:HiddenField ID="hfTokenId" runat="server" />

                                    <asp:Button ID="btnGenerarTokenOculto" runat="server" Text="Generar" Style="display: none;" CausesValidation="false" OnClick="btnGenerarTokenOculto_Click" />
                                    <asp:Button ID="btnRenovarTokenOculto" runat="server" Text="Renovar" Style="display: none;" CausesValidation="false" OnClick="btnRenovarTokenOculto_Click" />
                                    <asp:Button ID="btnRevocarTokenOculto" runat="server" Text="Revocar" Style="display: none;" CausesValidation="false" OnClick="btnRevocarTokenOculto_Click" />

                                    <asp:GridView ID="gvListado" runat="server" AutoGenerateColumns="False" DataKeyNames="Rut" OnRowDataBound="gvListado_RowDataBound" OnRowCommand="gvListado_RowCommand" OnRowCreated="gvListado_RowCreated" AllowPaging="True" PageSize="8" OnPageIndexChanging="gvListado_PageIndexChanging" CssClass="table table-hover text-start mg-b-0 border" Style="table-layout: fixed; width: 100%;" GridLines="None" PagerSettings-Mode="NumericFirstLast" PagerSettings-FirstPageText="Primera" PagerSettings-LastPageText="Última" PagerStyle-HorizontalAlign="Center">

                                        <EmptyDataRowStyle CssClass="text-center tx-14 text-muted" />

                                        <Columns>
                                            <asp:BoundField HeaderText="RUT" DataField="Rut">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Nombre" DataField="Nombre">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle Width="20%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Direcci&oacute;n" DataField="Direccion">
                                                <HeaderStyle Width="25%" />
                                                <ItemStyle Width="25%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Ciudad" DataField="Ciudad">
                                                <HeaderStyle Width="11%" />
                                                <ItemStyle Width="11%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Dispositivo Externo">
                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("DispositivoExterno")) == 0 ? "badge bg-success-transparent text-success px-3 py-2 fs-12" : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("DispositivoExterno")) == 0 ? "fe fe-check-circle me-1" : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("DispositivoExterno")) == 0 ? "Activado" : "Desactivado" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Estado">
                                                <HeaderStyle CssClass="text-center" Width="9%" />
                                                <ItemStyle CssClass="text-center" Width="9%" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Estado")) == 0 ? "badge bg-success-transparent text-success px-3 py-2 fs-12" : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>
                                                        <i class='<%# Convert.ToInt16(Eval("Estado")) == 0 ? "fe fe-check-circle me-1" : "fe fe-x-circle me-1" %>'></i>
                                                        <%# Convert.ToInt16(Eval("Estado")) == 0 ? "Vigente" : "Anulado" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Token">
                                                <HeaderStyle CssClass="text-center" Width="5%" />
                                                <ItemStyle CssClass="text-center" Width="5%" />
                                                <ItemTemplate>
                                                    <i class='<%# ObtenerIconoEstadoToken(Eval("TokenVigenciaCodigo")) + " " + ObtenerClaseEstadoToken(Eval("TokenVigenciaCodigo")) %>' title='<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("TokenVigenciaNombre")) + ((Convert.ToInt32(Eval("TokenVigenciaCodigo")) == 0 || Convert.ToInt32(Eval("TokenVigenciaCodigo")) == 1 || Convert.ToInt32(Eval("TokenVigenciaCodigo")) == 5) && Eval("TokenDiasRestantes") != null && Eval("TokenDiasRestantes") != System.DBNull.Value ? " - " + Convert.ToString(Eval("TokenDiasRestantes")) + " días restantes" : string.Empty)) %>'></i>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Acciones">
                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />
                                                <ItemTemplate>
                                                    <div class="d-inline-flex align-items-center justify-content-center flex-nowrap">
                                                        <asp:HyperLink ID="lnkEditar" runat="server" CssClass="btn ripple btn-outline-info btn-sm" ToolTip="Editar">
                                                            <i class="fe fe-edit"></i>
                                                        </asp:HyperLink>

                                                        <asp:PlaceHolder ID="phGenerarToken" runat="server" Visible='<%# PuedeGenerarToken(Eval("Estado"), Eval("TokenTotal")) %>'>
                                                            <a href="javascript:void(0);" class="btn ripple btn-outline-secondary btn-sm ms-1" title="Generar token" onclick='abrirModalGenerarToken("<%# HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("Rut"))) %>", "<%# HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("Nombre"))) %>"); event.stopPropagation(); return false;' ondblclick="event.stopPropagation(); return false;">
                                                                <i class="bi bi-key-fill"></i>
                                                            </a>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="phRenovarToken" runat="server" Visible='<%# PuedeRenovarToken(Eval("Estado"), Eval("TokenTotal"), Eval("TokenTotalOperativos")) %>'>
                                                            <a href="javascript:void(0);" class="btn ripple btn-outline-warning btn-sm ms-1" title="Renovar token" onclick='abrirModalRenovarToken("<%# HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("Rut"))) %>", "<%# HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("Nombre"))) %>"); event.stopPropagation(); return false;' ondblclick="event.stopPropagation(); return false;">
                                                                <i class="fe fe-refresh-cw"></i>
                                                            </a>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="phRevocarToken" runat="server" Visible='<%# PuedeRevocarToken(Eval("TokenId"), Eval("TokenTotalOperativos")) %>'>
                                                            <a href="javascript:void(0);" class="btn ripple btn-outline-danger btn-sm ms-1" title="Revocar token" onclick='abrirModalRevocarToken("<%# HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("Rut"))) %>", "<%# Convert.ToString(Eval("TokenId")) %>", true, "<%# HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("Nombre"))) %>"); event.stopPropagation(); return false;' ondblclick="event.stopPropagation(); return false;">
                                                                <i class="fe fe-slash"></i>
                                                            </a>
                                                        </asp:PlaceHolder>

                                                        <asp:LinkButton ID="btnBitacora" runat="server" CssClass="btn ripple btn-outline-info btn-sm ms-1" CommandName="Bitacora" CommandArgument='<%# Eval("Rut") %>' CausesValidation="false" ToolTip="Ver bitácora">
                                                            <i class="fe fe-eye"></i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>

                                    </asp:GridView>
                                </div>

                                <uc:Bitacora ID="ucBitacora" runat="server" />

                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Tabla -->
            </div>
        </div>
    </div>
    <!-- End Main Content-->

    <!-- Generar Token -->
    <div class="modal fade" id="modalGenerarToken" tabindex="-1" aria-labelledby="modalGenerarTokenLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content custom-card border-0">
                <div class="modal-body text-center p-5">

                    <div class="mb-4">
                        <span class="avatar avatar-xl rounded-circle bg-primary-transparent text-primary">
                            <i class="bi bi-key-fill fs-28"></i>
                        </span>
                    </div>

                    <h5 class="mb-2 fw-semibold" id="modalGenerarTokenLabel">
                        ¿Generar token?
                    </h5>

                    <p class="text-muted mb-2">
                        Se generará un token para la empresa:
                    </p>

                    <p class="fw-semibold text-primary mb-4" id="lblEmpresaGenerarToken"></p>

                    <div class="alert alert-warning mb-4" role="alert">
                        El token será válido durante 12 meses y se mostrará una sola vez.
                    </div>

                    <div class="d-flex justify-content-center gap-2">
                        <button type="button" class="btn btn-outline-secondary" style="min-width: 130px;" data-bs-dismiss="modal">
                            Cancelar
                        </button>

                        <button type="button" class="btn btn-outline-info" style="min-width: 130px;" onclick="confirmarGenerarToken();">
                            Sí, generar token
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Fin Generar Token -->

    <!-- Renovar Token -->
    <div class="modal fade" id="modalRenovarToken" tabindex="-1" aria-labelledby="modalRenovarTokenLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content custom-card border-0">
                <div class="modal-body text-center p-5">

                    <div class="mb-4">
                        <span class="avatar avatar-xl rounded-circle bg-warning-transparent text-warning">
                            <i class="fe fe-refresh-cw fs-28"></i>
                        </span>
                    </div>

                    <h5 class="mb-2 fw-semibold" id="modalRenovarTokenLabel">
                        ¿Renovar token?
                    </h5>

                    <p class="text-muted mb-2">
                        Se generará un nuevo token para la empresa:
                    </p>

                    <p class="fw-semibold text-primary mb-4" id="lblEmpresaRenovarToken"></p>

                    <div class="alert alert-warning mb-4" role="alert">
                        El token nuevo será válido durante 12 meses. El token anterior permanecerá operativo durante un máximo de 7 días.
                    </div>

                    <div class="d-flex justify-content-center gap-2">
                        <button type="button" class="btn btn-outline-secondary" style="min-width: 130px;" data-bs-dismiss="modal">
                            Cancelar
                        </button>

                        <button type="button" class="btn btn-outline-info" style="min-width: 130px;" onclick="confirmarRenovarToken();">
                            Sí, renovar token
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Fin Renovar Token -->

    <!-- Revocar Token -->
    <div class="modal fade" id="modalRevocarToken" tabindex="-1" aria-labelledby="modalRevocarTokenLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content custom-card border-0">
                <div class="modal-body p-5">

                    <div class="text-center mb-4">
                        <span class="avatar avatar-xl rounded-circle bg-danger-transparent text-danger">
                            <i class="fe fe-slash fs-28"></i>
                        </span>

                        <h5 class="mt-3 mb-2 fw-semibold" id="modalRevocarTokenLabel">
                            ¿Revocar token?
                        </h5>

                        <p class="text-muted mb-1">
                            Se revocará el token vigente de la empresa:
                        </p>

                        <p class="fw-semibold text-primary mb-0" id="lblEmpresaRevocarToken"></p>
                    </div>

                    <div class="mb-4">
                        <label class="form-label">Motivo de la revocación</label>
                        <asp:TextBox ID="txtMotivoRevocacion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="500" placeholder="Ingrese el motivo de la revocación" Style="resize: none;"></asp:TextBox>
                    </div>

                    <div class="alert alert-danger mb-4 text-center" role="alert">
                        La revocación es inmediata y el token dejará de ser válido para consumir la API.
                    </div>

                    <div class="d-flex justify-content-center gap-2">
                        <button type="button" class="btn btn-outline-secondary" style="min-width: 130px;" data-bs-dismiss="modal">
                            Cancelar
                        </button>

                        <button type="button" class="btn btn-outline-info" style="min-width: 130px;" onclick="confirmarRevocarToken();">
                            Sí, revocar token
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Fin Revocar Token -->

    <!-- Token Generado -->
    <div class="modal fade" id="modalTokenGenerado" tabindex="-1" aria-labelledby="modalTokenGeneradoLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content custom-card border-0">
                <div class="modal-body text-center p-5">

                    <div class="mb-4">
                        <span class="avatar avatar-xl rounded-circle bg-primary-transparent text-primary">
                            <i class="bi bi-key-fill fs-28"></i>
                        </span>
                    </div>

                    <h5 class="mb-2 fw-semibold" id="modalTokenGeneradoLabel">
                        Token generado
                    </h5>

                    <p class="text-muted mb-4">
                        Este token debe ser entregado al cliente para consumir la API de Cédula.
                    </p>

                    <div class="border rounded-3 p-3 mb-4 text-start">
                        <p class="mb-2 tx-13 text-muted">Token JWT</p>
                        <textarea id="txtTokenGenerado" class="form-control" rows="6" readonly="readonly" autocomplete="off" spellcheck="false" style="resize: none;"></textarea>
                        <p class="mb-0 mt-3 tx-13 text-muted">Válido hasta: <strong id="lblTokenVencimiento" class="text-primary"></strong></p>
                    </div>

                    <div class="alert alert-warning mb-4" role="alert">
                        El token se mostrará solo una vez. Debe copiarlo y almacenarlo de forma segura antes de cerrar esta ventana.
                    </div>

                    <div class="d-flex justify-content-center gap-2">
                        <button type="button" class="btn btn-outline-secondary" style="min-width: 130px;" onclick="copiarTokenGenerado();">
                            Copiar token
                        </button>

                        <button type="button" class="btn btn-outline-info" style="min-width: 130px;" data-bs-dismiss="modal">
                            Cerrar
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Fin Token Generado -->

</asp:Content>