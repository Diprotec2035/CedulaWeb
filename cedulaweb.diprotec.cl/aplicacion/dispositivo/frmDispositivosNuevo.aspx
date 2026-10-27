<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmDispositivosNuevo.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.dispositivo.frmDispositivosNuevo" Async="true" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            $("#<%=ddlMarca.ClientID%>").on("change blur", function () {
                var campo = $(this);
                var valor = campo.val();

                campo.removeClass("is-valid is-invalid");

                if (valor !== null && valor !== "") {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=ddlModelo.ClientID%>").on("change blur", function () {
                var campo = $(this);
                var valor = campo.val();

                campo.removeClass("is-valid is-invalid");

                if (valor !== null && valor !== "") {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=txtPartnumber.ClientID%>").on("blur", function () {
                var campo = $(this);
                var valor = campo.val().trim();

                campo.removeClass("is-valid is-invalid");

                if (valor.length > 0) {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=txtSerialnumber.ClientID%>").on("blur", function () {
                var campo = $(this);
                var valor = campo.val().trim();

                campo.removeClass("is-valid is-invalid");

                if (valor.length > 0) {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=txtComentarios.ClientID%>").on("blur", function () {
                var campo = $(this);
                var valor = campo.val().trim();

                campo.removeClass("is-valid is-invalid");

                if (valor.length > 0) {
                    campo.addClass("is-valid");
                }
            });

            $("#<%=txtPartnumber.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoPartNumber)%>");
            $("#<%=txtSerialnumber.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoSerialNumber)%>");
            $("#<%=txtComentarios.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoComentarioDispositivo)%>");
        });

        function blockFormulario() {
            var formulario = document.getElementById("<%=Page.Form.ClientID%>");

            if (!formulario.checkValidity()) {
                formulario.reportValidity();
                return false;
            }

            $("#<%=btnRegistrar.ClientID%>").val("Registrando...");
            $("#<%=btnVolver.ClientID%>").prop("disabled", true);

            setTimeout(function () {
                $("#<%=btnRegistrar.ClientID%>").prop("disabled", true);
            }, 0);

            return true;
        }

        function mostrarModalCodigoActivacion(codigo) {
            $("#lblCodigoActivacionGenerado").text(codigo);

            if (typeof bootstrap === "undefined") {
                setTimeout(function () {
                    mostrarModalCodigoActivacion(codigo);
                }, 100);

                return;
            }

            var modalElemento = document.getElementById("modalCodigoActivacion");
            var modal = new bootstrap.Modal(modalElemento, {
                backdrop: "static",
                keyboard: false
            });

            modal.show();
        }

        function redirigirListadoDispositivos() {
            window.location = '<%=cedulaweb.diprotec.cl.aplicacion.helpers.ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>';
        }

        function copiarCodigoActivacion() {
            var codigo = $("#lblCodigoActivacionGenerado").text().trim();

            if (codigo.length === 0) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorWarning%>", "", "No hay código de activación para copiar.");
                return;
            }

            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(codigo).then(function () {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "Código de activación copiado al portapapeles.");
                }).catch(function () {
                    copiarCodigoActivacionFallback(codigo);
                });

                return;
            }

            copiarCodigoActivacionFallback(codigo);
        }

        function copiarCodigoActivacionFallback(codigo) {
            var inputTemporal = document.createElement("input");
            inputTemporal.value = codigo;
            inputTemporal.setAttribute("readonly", "");
            inputTemporal.style.position = "absolute";
            inputTemporal.style.left = "-9999px";

            document.body.appendChild(inputTemporal);
            inputTemporal.select();
            inputTemporal.setSelectionRange(0, 99999);

            try {
                document.execCommand("copy");
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "Código de activación copiado al portapapeles.");
            }
            catch (e) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "No fue posible copiar el código de activación.");
            }

            document.body.removeChild(inputTemporal);
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
                            <li class="breadcrumb-item">
                                <a href="<%=ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>">
                                    <%:ModuloNavegacionHelper.ObtenerTituloAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>
                                </a>
                            </li>
                            <li class="breadcrumb-item active text-primary" aria-current="page">
                                <%:ModuloContextoHelper.ObtenerTituloPagina(this)%>
                            </li>
                        </ol>
                    </div>
                </div>
                <!-- End Page Header -->

                <!-- Row -->
                <div class="row">
                    <div class="col-lg-12 col-md-12">
                        <div class="card custom-card">
                            <div class="card-header">
                                <h6 class="card-title"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>
                            </div>

                            <div class="card-body">
                                <div class="row">
                                    <div class="col-xl-8 col-lg-10 col-md-12 mx-auto">

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Marca</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlMarca"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged"
                                                    required="required"
                                                    aria-describedby="feedbackMarca">
                                                </asp:DropDownList>
                                                <div id="feedbackMarca" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionDispositivoMarcaVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Modelo</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlModelo"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    required="required"
                                                    aria-describedby="feedbackModelo">
                                                </asp:DropDownList>
                                                <div id="feedbackModelo" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionDispositivoModeloVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Part Number</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox ID="txtPartnumber" runat="server" CssClass="form-control" MaxLength="50" placeholder="partnumber dispositivo" required="required" aria-describedby="feedbackPartnumber"></asp:TextBox>
                                                <div id="feedbackPartnumber" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionDispositivoPartnumberVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Serial Number</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox ID="txtSerialnumber" runat="server" CssClass="form-control" MaxLength="50" placeholder="serialnumber dispositivo" required="required" aria-describedby="feedbackSerialnumber"></asp:TextBox>
                                                <div id="feedbackSerialnumber" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionDispositivoSerialnumberVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Comentarios</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox ID="txtComentarios" runat="server" CssClass="form-control" MaxLength="100" placeholder="comentarios dispositivo" aria-describedby="feedbackComentarios"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center">
                                            <div class="col-md-4"></div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:Button ID="btnRegistrar" runat="server" CssClass="btn ripple btn-main-primary" Style="min-width: 130px;" Text="Registrar" OnClientClick="return blockFormulario();" OnClick="btnRegistrar_Click" />
                                                <asp:Button ID="btnVolver" runat="server" CssClass="btn btn-outline-secondary ms-2" Style="min-width: 130px;" Text="Volver" CausesValidation="false" UseSubmitBehavior="false" OnClick="btnVolver_Click" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Row -->
            </div>
        </div>
    </div>
    <!-- End Main Content-->

    <!-- Código de Activación -->
    <div class="modal fade" id="modalCodigoActivacion" tabindex="-1" aria-labelledby="modalCodigoActivacionLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content custom-card border-0">
                <div class="modal-body text-center p-5">

                    <div class="mb-4">
                        <span class="avatar avatar-xl rounded-circle bg-primary-transparent text-primary">
                            <i class="bi bi-key-fill fs-28"></i>
                        </span>
                    </div>

                    <h5 class="mb-2 fw-semibold" id="modalCodigoActivacionLabel">
                        Código de activación generado
                    </h5>

                    <p class="text-muted mb-4">
                        Este código debe ser entregado al usuario para activar el dispositivo.
                    </p>

                    <div class="border rounded-3 p-3 mb-4">
                        <p class="mb-1 tx-13 text-muted">Código de activación</p>
                        <h3 class="mb-0 fw-bold text-primary" id="lblCodigoActivacionGenerado"></h3>
                    </div>

                    <div class="alert alert-warning mb-4" role="alert">
                        Este código es de un solo uso. Si el usuario lo olvida o lo pierde, deberá regenerarse uno nuevo.
                    </div>

                    <div class="d-flex justify-content-center gap-2">
                        <button type="button"
                                class="btn btn-outline-secondary"
                                style="min-width: 130px;"
                                onclick="copiarCodigoActivacion();">
                            Copiar c&oacute;digo
                        </button>

                        <button type="button"
                                class="btn btn-outline-info"
                                style="min-width: 130px;"
                                onclick="redirigirListadoDispositivos();">
                            Entendido
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Fin Código de Activación -->
</asp:Content>