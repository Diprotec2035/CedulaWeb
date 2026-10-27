<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="cedulaweb.diprotec.cl.Default" Async="true" %>
<%@ Import Namespace="Modelo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0" />
    <meta name="description" content="Diprotec"/>
    <meta name="author" content="Spruko™ Technologies Private Limited" />
    <meta name="keywords" content="" />
    <link rel="shortcut icon" type="image/x-icon" href="/assets/images/brand/favicon.ico" />

    <title><%=ConstantesModelo.ConsTituloWebPage%></title>

    <link href="/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="/assets/css/style.css" rel="stylesheet" />
    <link href="/assets/css/plugins.css" rel="stylesheet" />
    <link href="/assets/css/icons.css" rel="stylesheet" />
    <link href="/assets/switcher/css/switcher.css" rel="stylesheet" />
    <link href="/assets/switcher/demo.css" rel="stylesheet" />

    <script src="https://www.google.com/recaptcha/enterprise.js?render=<%=ConstantesModelo.ReCaptchaSiteKey_v3%>"></script>
    <script src="/assets/js/vendors/jquery.js"></script>
    <script src="/assets/js/vendors/jquery-text.js"></script>
    <script src="/assets/js/custom-switcher.js"></script>
    <script src="/assets/js/custom.js"></script>
    <script src="/assets/js/funciones.js"></script>
    <script src="/assets/plugins/bootstrap/popper.min.js"></script>
    <script src="/assets/plugins/bootstrap/js/bootstrap.min.js"></script>
    <script src="/assets/plugins/perfect-scrollbar/perfect-scrollbar.min.js"></script>
    <script src="/assets/switcher/js/switcher.js"></script>

    <script type="text/javascript">
        function inicializarLogin() {
            inicializarCampoRut("#<%=txtRutEmpresa.ClientID%>");
            inicializarCampoRut("#<%=txtRutUsuario.ClientID%>");

            $("#<%=txtPassword.ClientID%>").off("blur.validacionPassword").on("blur.validacionPassword", function () {
                var campo = $(this);
                var password = campo.val().trim();

                campo.removeClass("is-valid is-invalid");

                if (password.length > 0) {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=txtRutEmpresa.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoRut)%>");
            $("#<%=txtRutUsuario.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoRut)%>");
            $("#<%=txtPassword.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoPassword)%>");
        }

        function normalizarRutEntrada(valor) {
            var rut = (valor || "").toUpperCase().replace(/[^0-9K]/g, "");

            if (rut.indexOf("K") >= 0) {
                rut = rut.replace(/K/g, "");
                rut = rut.substring(0, 8) + "K";
            }
            else {
                rut = rut.substring(0, 9);
            }

            return rut;
        }

        function inicializarCampoRut(selector) {
            var campo = $(selector);

            campo.off("input.validacionRut").on("input.validacionRut", function () {
                var control = $(this);

                control.val(normalizarRutEntrada(control.val()));
                control.removeClass("is-valid is-invalid");
            });

            campo.off("blur.validacionRut").on("blur.validacionRut", function () {
                var control = $(this);
                var rut = normalizarRutEntrada(control.val());

                control.removeClass("is-valid is-invalid");

                if ((rut.length > 1) && (rut.length <= 9) && ValidateRut(rut)) {
                    var run = rut.substring(0, rut.length - 1);
                    var dv = rut.substring(rut.length - 1);

                    control.val(run + "-" + dv);
                    control.addClass("is-valid");
                }
                else {
                    control.val("");
                    control.addClass("is-invalid");
                }
            });
        }

        $(document).ready(function () {
            inicializarLogin();

            if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    inicializarLogin();
                });
            }
        });

        var enviandoLogin = false;

        function ejecutaReCaptcha() {
            if (enviandoLogin) {
                enviandoLogin = false;
                return true;
            }

            if (!document.getElementById("frmLogin").checkValidity()) {
                document.getElementById("frmLogin").reportValidity();
                return false;
            }

            $("#<%=btnIngresar.ClientID%>").val("Iniciando...");

            $("#lnkRecuperar").css({
                "pointer-events": "none",
                "opacity": "0.5"
            });

            grecaptcha.enterprise.ready(function () {
                grecaptcha.enterprise.execute('<%=ConstantesModelo.ReCaptchaSiteKey_v3%>', { action: 'login' }).then(function (token) {
                    document.getElementById('gRecaptchaResponse').value = token;
                    enviandoLogin = true;
                    document.getElementById('<%=btnIngresar.ClientID%>').click();

                    setTimeout(function () {
                        $("#<%=btnIngresar.ClientID%>").prop("disabled", true);
                    }, 0);
                })
                .catch(function () {
                    $("#<%=btnIngresar.ClientID%>").val("Iniciar Sesión");
                    $("#<%=btnIngresar.ClientID%>").prop("disabled", false);

                    $("#lnkRecuperar").css({
                        "pointer-events": "auto",
                        "opacity": "1"
                    });

                    inicializarLogin();

                    mostrarAlerta('danger', '', 'No fue posible validar reCAPTCHA.');
                });
            });

            return false;
        }

        function alternarPassword() {
            var campoPassword = document.getElementById("<%=txtPassword.ClientID%>");
            var icono = document.getElementById("iconoVerPassword");
            var boton = document.getElementById("btnVerPassword");

            if (!campoPassword || !icono) {
                return false;
            }

            if (campoPassword.type === "password") {
                campoPassword.type = "text";
                icono.classList.remove("fa-eye");
                icono.classList.add("fa-eye-slash");

                if (boton) {
                    boton.setAttribute("aria-label", "Ocultar contraseña");
                }
            }
            else {
                campoPassword.type = "password";
                icono.classList.remove("fa-eye-slash");
                icono.classList.add("fa-eye");

                if (boton) {
                    boton.setAttribute("aria-label", "Mostrar contraseña");
                }
            }

            campoPassword.focus();
            return false;
        }
    </script>
</head>

<body class="app sidebar-mini login-img">
    <form id="frmLogin" runat="server" autocomplete="off">
        <asp:HiddenField ID="gRecaptchaResponse" runat="server" ClientIDMode="Static" />
        <asp:ScriptManager ID="smLogin" runat="server"></asp:ScriptManager>

        <!-- Loader -->
        <div id="global-loader">
            <img src="/assets/images/svgs/loader.svg" alt="loader" />
        </div>
        <!-- /Loader -->

        <!-- Page -->
        <div class="page main-signin-wrapper">
            <!-- Row -->
            <div class="row text-center ps-0 pe-0 ms-0 me-0">
                <div class=" col-xl-3 col-lg-5 col-md-5 d-block mx-auto">
                    <div class="card custom-card">
                        <div class="text-center mb-2 pt-3">
                            <a href="javascript:void(0);" onclick="return false;" tabindex="-1" aria-label="Logo DIPROTEC">
                                <img src="/assets/images/brand/logo.png" class="header-brand-img" alt="Diprotec" />
                                <img src="/assets/images/brand/logo1.png" class="header-brand-img theme-logos" alt="Diprotec" />
                            </a>
                        </div>

                        <div class="card-body pd-25">
                            <asp:UpdatePanel ID="UpdatePanelLogin" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>

                                    <asp:Panel
                                        ID="pnlCuentaSuspendida"
                                        runat="server"
                                        Visible="false"
                                        CssClass="text-center p-4">

                                        <div class="mb-4">
                                            <i class="fa fa-exclamation-triangle text-warning" style="font-size: 58px;"></i>
                                        </div>

                                        <h4 class="text-warning mb-3">Cuenta suspendida</h4>

                                        <p class="text-muted mb-3" style="font-size: 15px; line-height: 1.6;">
                                            <asp:Literal ID="litCuentaSuspendida" runat="server"></asp:Literal>
                                        </p>

                                        <div class="alert alert-light border text-start mb-0" style="font-size: 14px;">
                                            <strong>Pasos a seguir:</strong><br />
                                            1. Contacte a su ejecutivo comercial DIPROTEC.<br />
                                            2. Solicite la revisión del estado de su cuenta empresa.<br />
                                            3. Una vez regularizada la situación, intente iniciar sesión nuevamente.
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pnlFormularioLogin" runat="server">

                                        <h4 class="text-center pb-5">Iniciar Sesi&oacute;n</h4>

                                        <div class="form-group text-start">
                                            <label>RUT empresa</label>
                                            <asp:TextBox
                                                ID="txtRutEmpresa"
                                                runat="server"
                                                class="form-control"
                                                MaxLength="12"
                                                placeholder="RUT empresa"
                                                required="required"
                                                autocomplete="off"
                                                aria-describedby="feedbackRutEmpresa">
                                            </asp:TextBox>
                                            <div id="feedbackRutEmpresa" class="invalid-feedback">
                                                <%=ConstantesModelo.ConsErrorRutEmpresa%>
                                            </div>
                                        </div>

                                        <div class="form-group text-start">
                                            <label>RUT usuario</label>
                                            <asp:TextBox
                                                ID="txtRutUsuario"
                                                runat="server"
                                                class="form-control"
                                                MaxLength="12"
                                                placeholder="RUT usuario"
                                                required="required"
                                                autocomplete="off"
                                                aria-describedby="feedbackRutUsuario">
                                            </asp:TextBox>
                                            <div id="feedbackRutUsuario" class="invalid-feedback">
                                                <%=ConstantesModelo.ConsErrorRutUsuario%>
                                            </div>
                                        </div>

                                        <div class="form-group text-start">
                                            <label>Contrase&ntilde;a</label>

                                            <div class="position-relative">
                                                <asp:TextBox
                                                    ID="txtPassword"
                                                    TextMode="Password"
                                                    runat="server"
                                                    class="form-control pe-5"
                                                    MaxLength="20"
                                                    placeholder="contraseña"
                                                    required="required"
                                                    autocomplete="new-password"
                                                    aria-describedby="feedbackPassword">
                                                </asp:TextBox>

                                                <button
                                                    id="btnVerPassword"
                                                    type="button"
                                                    class="btn position-absolute top-50 end-0 translate-middle-y border-0 bg-transparent"
                                                    style="z-index: 5;"
                                                    aria-label="Mostrar contraseña"
                                                    onclick="return alternarPassword();">
                                                    <i id="iconoVerPassword" class="fa fa-eye"></i>
                                                </button>
                                            </div>

                                            <div id="feedbackPassword" class="invalid-feedback">
                                                <%=ConstantesModelo.ConsValidacionUsuarioPasswordVacio%>
                                            </div>
                                        </div>

                                        <asp:Button
                                            ID="btnIngresar"
                                            runat="server"
                                            class="btn ripple btn-main-primary btn-block"
                                            Text="Iniciar Sesi&oacute;n"
                                            OnClick="btnIngresar_Click"
                                            OnClientClick="return ejecutaReCaptcha();" />

                                        <a id="lnkRecuperar" href="<%=ConstantesModelo.ConsHttpRecuperar%>" class="d-block text-center mt-3 link-danger">¿Olvidó su contraseña?</a>

                                    </asp:Panel>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnIngresar" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
            <!-- End Row -->
        </div>
        <!-- End Page -->

        <!-- Back to top -->
        <a href="#top" id="back-to-top"><i class="fa fa-angle-up"></i></a>
    </form>

    <div id="divAlerta" class="position-fixed bottom-0 end-0 p-3" style="z-index: 9999;"></div>
</body>
</html>