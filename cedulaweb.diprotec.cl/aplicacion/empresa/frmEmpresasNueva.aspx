<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmEmpresasNueva.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.empresa.frmEmpresasNueva" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            var campoRut = $("#<%=txtRut.ClientID%>");

            campoRut.off(".rutEmpresa");

            campoRut.on("input.rutEmpresa", function () {
                var campo = $(this);
                var valor = campo.val().toUpperCase().replace(/[^0-9K]/g, "");
                var terminaEnK = valor.charAt(valor.length - 1) === "K";
                var numeros = valor.replace(/K/g, "");

                numeros = numeros.substring(0, terminaEnK ? 8 : 9);

                campo.val(numeros + (terminaEnK ? "K" : ""));
                campo.removeClass("is-valid is-invalid");
            });

            campoRut.on("blur.rutEmpresa", function () {
                var campo = $(this);
                var rut = campo.val().replace("-", "").trim().toUpperCase();

                campo.removeClass("is-valid is-invalid");

                if ((rut.length > 1) && (rut.length <= 9) && ValidateRut(rut)) {
                    var run = rut.substring(0, rut.length - 1);
                    var dv = rut.substring(rut.length - 1);

                    campo.val(run + "-" + dv);
                    campo.addClass("is-valid");
                }
                else {
                    campo.val("");
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=txtNombre.ClientID%>").on("blur", function () {
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

            $("#<%=txtDireccion.ClientID%>").on("blur", function () {
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

            $("#<%=txtCiudad.ClientID%>").on("blur", function () {
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

            $("#<%=txtComuna.ClientID%>").on("blur", function () {
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

            $("#<%=txtContacto.ClientID%>").on("blur", function () {
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

            $("#<%=txtContactoTelefono.ClientID%>").on("blur", function () {
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

            $("#<%=txtContactoEmail.ClientID%>").off("blur").on("blur", function () {
                var campo = $(this);
                var email = campo.val().trim().toLowerCase();

                campo.removeClass("is-valid is-invalid");

                if (email === "") {
                    campo.addClass("is-invalid");
                    return;
                }

                if (ValidateEmail(email, "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsExpresionRegularEmail)%>")) {
                    campo.val(email);
                    campo.addClass("is-valid");
                }
                else {
                    campo.val("");
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=txtRut.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoRut)%>");
            $("#<%=txtNombre.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombreEmpresa)%>");
            $("#<%=txtDireccion.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoDireccion)%>");
            $("#<%=txtCiudad.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombreLugar)%>");
            $("#<%=txtComuna.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombreLugar)%>");
            $("#<%=txtContacto.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombrePersona)%>");
            $("#<%=txtContactoTelefono.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTelefono)%>");
            $("#<%=txtContactoEmail.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoEmail)%>");
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

        var urlRetornoToken = "";

        function mostrarModalTokenGenerado(token, fechaVencimiento, urlRetorno) {
            $("#txtTokenGenerado").val(token);
            $("#lblTokenVencimiento").text(fechaVencimiento);
            urlRetornoToken = urlRetorno;

            if (typeof bootstrap === "undefined") {
                setTimeout(function () {
                    mostrarModalTokenGenerado(token, fechaVencimiento, urlRetorno);
                }, 100);

                return;
            }

            var modalElemento = document.getElementById("modalTokenGenerado");
            var modal = new bootstrap.Modal(modalElemento, {
                backdrop: "static",
                keyboard: false
            });

            modal.show();
        }

        function redirigirListadoEmpresas() {
            if (urlRetornoToken !== "") {
                window.location = urlRetornoToken;
                return;
            }

            window.location = '<%=ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>';
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
                                <a href="<%=ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>"><%:ModuloNavegacionHelper.ObtenerTituloAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%></a>
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
                                                <label class="mg-b-0">RUT</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtRut"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="12"
                                                    placeholder="RUT empresa"
                                                    required="required"
                                                    aria-describedby="feedbackRut">
                                                </asp:TextBox>

                                                <div id="feedbackRut" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsErrorRutEmpresa%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Nombre</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtNombre"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="nombre empresa"
                                                    required="required"
                                                    aria-describedby="feedbackNombre">
                                                </asp:TextBox>

                                                <div id="feedbackNombre" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionEmpresaNombreVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Direcci&oacute;n</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtDireccion"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="direcci&oacute;n empresa"
                                                    required="required"
                                                    aria-describedby="feedbackDireccion">
                                                </asp:TextBox>

                                                <div id="feedbackDireccion" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionEmpresaDireccionVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Ciudad</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtCiudad"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="50"
                                                    placeholder="ciudad empresa"
                                                    required="required"
                                                    aria-describedby="feedbackCiudad">
                                                </asp:TextBox>

                                                <div id="feedbackCiudad" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionEmpresaCiudadVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Comuna</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtComuna"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="50"
                                                    placeholder="comuna empresa"
                                                    required="required"
                                                    aria-describedby="feedbackComuna">
                                                </asp:TextBox>

                                                <div id="feedbackComuna" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionEmpresaComunaVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Contacto</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtContacto"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="contacto empresa"
                                                    required="required"
                                                    aria-describedby="feedbackContacto">
                                                </asp:TextBox>

                                                <div id="feedbackContacto" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionEmpresaContactoVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Tel&eacute;fono Contacto</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtContactoTelefono"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="10"
                                                    placeholder="tel&eacute;fono contacto"
                                                    required="required"
                                                    aria-describedby="feedbackContactoTelefono">
                                                </asp:TextBox>

                                                <div id="feedbackContactoTelefono" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionEmpresaContactoTelefonoVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Email Contacto</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtContactoEmail"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="email contacto"
                                                    required="required"
                                                    aria-describedby="feedbackContactoEmail">
                                                </asp:TextBox>

                                                <div id="feedbackContactoEmail" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsErrorEmpresaContactoEmail%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Registra Dispositivo Externo</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfExterno" runat="server" Value="1" />

                                                <div id="tglExterno" runat="server" class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center">
                                            <div class="col-md-4"></div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:Button
                                                    ID="btnRegistrar"
                                                    runat="server"
                                                    CssClass="btn ripple btn-main-primary"
                                                    Style="min-width: 130px;"
                                                    Text="Registrar"
                                                    OnClientClick="return blockFormulario();"
                                                    OnClick="btnRegistrar_Click" />

                                                <asp:Button
                                                    ID="btnVolver"
                                                    runat="server"
                                                    CssClass="btn btn-outline-secondary ms-2"
                                                    Style="min-width: 130px;"
                                                    Text="Volver"
                                                    CausesValidation="false"
                                                    UseSubmitBehavior="false"
                                                    OnClick="btnVolver_Click" />
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

    <!-- Token JWT -->
    <div class="modal fade" id="modalTokenGenerado" tabindex="-1" aria-labelledby="modalTokenGeneradoLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content custom-card border-0">
                <div class="modal-body text-center p-5">

                    <div class="mb-4">
                        <span class="avatar avatar-xl rounded-circle bg-primary-transparent text-primary">
                            <i class="bi bi-key-fill fs-28"></i>
                        </span>
                    </div>

                    <h5 class="mb-2 fw-semibold" id="modalTokenGeneradoLabel">
                        <%=ConstantesModelo.ConsEmpresaTokenModalTitulo%>
                    </h5>

                    <p class="text-muted mb-4">
                        <%=ConstantesModelo.ConsEmpresaTokenModalDescripcion%>
                    </p>

                    <div class="border rounded-3 p-3 mb-4 text-start">
                        <p class="mb-2 tx-13 text-muted">Token JWT</p>
                        <textarea id="txtTokenGenerado" class="form-control" rows="6" readonly="readonly" autocomplete="off" spellcheck="false" style="resize: none;"></textarea>
                        <p class="mb-0 mt-3 tx-13 text-muted">Válido hasta: <strong id="lblTokenVencimiento" class="text-primary"></strong></p>
                    </div>

                    <div class="alert alert-warning mb-4" role="alert">
                        <%=ConstantesModelo.ConsEmpresaTokenModalAdvertencia%>
                    </div>

                    <div class="d-flex justify-content-center gap-2">
                        <button type="button" class="btn btn-outline-secondary" style="min-width: 130px;" onclick="copiarTokenGenerado();">
                            Copiar token
                        </button>

                        <button type="button" class="btn btn-outline-info" style="min-width: 130px;" onclick="redirigirListadoEmpresas();">
                            Entendido
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Fin Token JWT -->

</asp:Content>