<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="cedulaweb.diprotec.cl.recuperar._default" Async="true" %>
<%@ Import Namespace="Modelo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0" />
    <meta name="description" content="Diprotec" />
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
        $(document).ready(function () {
            inicializarCampoRut("#<%=txtRutEmpresa.ClientID%>");
            inicializarCampoRut("#<%=txtRutUsuario.ClientID%>");

            $("#<%=txtEmailUsuario.ClientID%>").off("blur").on("blur", function () {
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

            $("#<%=txtRutEmpresa.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoRut)%>");
            $("#<%=txtRutUsuario.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoRut)%>");
            $("#<%=txtEmailUsuario.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoEmail)%>");
        });

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

        var enviandoLogin = false;

        function ejecutaReCaptcha() {
            if (enviandoLogin) {
                enviandoLogin = false;
                return true;
            }

            if (!document.getElementById("frmRecuperar").checkValidity()) {
                document.getElementById("frmRecuperar").reportValidity();
                return false;
            }

            $("#<%=btnReset.ClientID%>").val("Reseteando...");

            $("#lnkVolver").css({
                "pointer-events": "none",
                "opacity": "0.5"
            });

            grecaptcha.enterprise.ready(function () {
                grecaptcha.enterprise.execute('<%=ConstantesModelo.ReCaptchaSiteKey_v3%>', { action: 'login' }).then(function (token) {
                    document.getElementById('gRecaptchaResponse').value = token;
                    enviandoLogin = true;
                    document.getElementById('<%=btnReset.ClientID%>').click();

                    setTimeout(function () {
                        $("#<%=btnReset.ClientID%>").prop("disabled", true);
                    }, 0);
                })
                .catch(function () {
                    $("#<%=btnReset.ClientID%>").val("Resetear Contraseña");
                    $("#<%=btnReset.ClientID%>").prop("disabled", false);

                    $("#lnkVolver").css({
                        "pointer-events": "auto",
                        "opacity": "1"
                    });

                    mostrarAlerta('danger', '', 'No fue posible validar reCAPTCHA.');
                });
            });

            return false;
        }
    </script>
</head>

<body class="app sidebar-mini login-img">
    <form id="frmRecuperar" runat="server">
        <asp:HiddenField ID="gRecaptchaResponse" runat="server" ClientIDMode="Static" />
        <asp:ScriptManager ID="smRecuperar" runat="server"></asp:ScriptManager>

        <!-- Loader -->
        <div id="global-loader">
            <img src="/assets/images/svgs/loader.svg" alt="loader" />
        </div>
        <!-- /Loader -->

        <!-- Page -->
        <div class="page main-signin-wrapper">

            <!-- Row -->
            <div class="row text-center ps-0 pe-0 ms-0 me-0">
                <div class="col-xl-3 col-lg-5 col-md-5 d-block mx-auto">
                    <div class="card custom-card">

                        <div class="text-center mb-2 pt-3">
                            <a href="<%=ConstantesModelo.ConsHttpInicio%>">
                                <img src="/assets/images/brand/logo.png" class="header-brand-img" alt="logo" />
                                <img src="/assets/images/brand/logo1.png" class="header-brand-img theme-logos" alt="logo" />
                            </a>
                        </div>

                        <div class="card-body pd-25">
                            <h4 class="text-center pb-5">Resetear Contrase&ntilde;a</h4>

                            <div class="form-group text-start">
                                <label>RUT empresa</label>

                                <asp:TextBox
                                    ID="txtRutEmpresa"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="12"
                                    placeholder="RUT empresa"
                                    required="required"
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
                                    CssClass="form-control"
                                    MaxLength="12"
                                    placeholder="RUT usuario"
                                    required="required"
                                    aria-describedby="feedbackRutUsuario">
                                </asp:TextBox>

                                <div id="feedbackRutUsuario" class="invalid-feedback">
                                    <%=ConstantesModelo.ConsErrorRutUsuario%>
                                </div>
                            </div>

                            <div class="form-group text-start">
                                <label>Email Usuario</label>

                                <asp:TextBox
                                    ID="txtEmailUsuario"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="100"
                                    placeholder="email usuario"
                                    required="required"
                                    aria-describedby="feedbackEmailUsuario">
                                </asp:TextBox>

                                <div id="feedbackEmailUsuario" class="invalid-feedback">
                                    <%=ConstantesModelo.ConsErrorUsuarioEmail%>
                                </div>
                            </div>

                            <asp:UpdatePanel ID="UpdatePanel" runat="server">
                                <ContentTemplate>
                                    <asp:Button
                                        ID="btnReset"
                                        runat="server"
                                        CssClass="btn ripple btn-main-primary btn-block"
                                        Text="Resetear Contrase&ntilde;a"
                                        OnClick="btnReset_Click"
                                        OnClientClick="return ejecutaReCaptcha();" />
                                </ContentTemplate>

                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <a id="lnkVolver" href="<%=ConstantesModelo.ConsHttpInicio%>" class="d-block text-center mt-3 link-danger">
                                <i class="fa fa-arrow-left me-1"></i>
                                Volver al login
                            </a>
                        </div>

                    </div>
                </div>
            </div>
            <!-- End Row -->

        </div>
        <!-- End Page -->

        <!-- Back to top -->
        <a href="#top" id="back-to-top">
            <i class="fa fa-angle-up"></i>
        </a>
    </form>

    <div id="divAlerta" class="position-fixed bottom-0 end-0 p-3" style="z-index: 9999;"></div>
</body>
</html>