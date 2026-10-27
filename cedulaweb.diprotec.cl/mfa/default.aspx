<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="cedulaweb.diprotec.cl.mfa._default" %>
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

        function inicializarMFA() {
            var campo = $("#<%=txtCodigo.ClientID%>");

            campo.validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTelefono)%>");

            campo.off("input.validacionMFA").on("input.validacionMFA", function () {
                var control = $(this);
                var codigo = control.val().replace(/[^0-9]/g, "").substring(0, <%=ConstantesModelo.ConsMfaTotpCodigoLargo%>);

                control.val(codigo);
                control.removeClass("is-valid is-invalid");
            });

            campo.off("blur.validacionMFA").on("blur.validacionMFA", function () {
                var control = $(this);
                var codigo = control.val();

                control.removeClass("is-valid is-invalid");

                if (codigo.length === <%=ConstantesModelo.ConsMfaTotpCodigoLargo%>) {
                    control.addClass("is-valid");
                }
                else {
                    control.addClass("is-invalid");
                }
            });
        }

        function inicializarCodigoRecuperacion() {
            var campo = $("#<%=txtCodigoRecuperacion.ClientID%>");
            var caracteres = "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsMfaTotpCodigoRecuperacionCaracteres)%>";
            var separador = "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsMfaTotpCodigoRecuperacionSeparador)%>";
            var largo = <%=ConstantesModelo.ConsMfaTotpCodigoRecuperacionLargo%>;
            var largoBloque = <%=ConstantesModelo.ConsMfaTotpCodigoRecuperacionBloqueLargo%>;

            campo.off("input.validacionMFARecuperacion").on("input.validacionMFARecuperacion", function () {
                var control = $(this);
                var valor = control.val().toUpperCase();
                var normalizado = "";

                for (var indice = 0; indice < valor.length; indice++) {
                    if (caracteres.indexOf(valor.charAt(indice)) >= 0) {
                        normalizado += valor.charAt(indice);
                    }
                }

                normalizado = normalizado.substring(0, largo);

                if (normalizado.length > largoBloque) {
                    normalizado = normalizado.substring(0, largoBloque) + separador + normalizado.substring(largoBloque);
                }

                control.val(normalizado);
                control.removeClass("is-valid is-invalid");
            });

            campo.off("blur.validacionMFARecuperacion").on("blur.validacionMFARecuperacion", function () {
                var control = $(this);
                var codigo = control.val().replace(separador, "");

                control.removeClass("is-valid is-invalid");

                if (codigo.length === largo) {
                    control.addClass("is-valid");
                }
                else {
                    control.addClass("is-invalid");
                }
            });
        }

        function seleccionarTexto(control) {
            if (!control) {
                return;
            }

            control.focus();
            control.select();
        }

        function obtenerCodigosRecuperacionMFA() {
            var codigos = [];

            $(".codigo-recuperacion-mfa").each(function () {
                var codigo = $(this).text().trim();

                if (codigo !== "") {
                    codigos.push(codigo);
                }
            });

            return codigos.join("\r\n");
        }

        function copiarCodigosRecuperacionMFA() {
            var contenido = obtenerCodigosRecuperacionMFA();

            if (contenido === "") {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionCopiar)%>");
                return false;
            }

            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(contenido).then(function () {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsExitoMfaTotpCodigosRecuperacionCopiar)%>");
                }).catch(function () {
                    copiarCodigosRecuperacionMFAFallback(contenido);
                });

                return false;
            }

            copiarCodigosRecuperacionMFAFallback(contenido);
            return false;
        }

        function copiarCodigosRecuperacionMFAFallback(contenido) {
            var inputTemporal = document.createElement("textarea");

            inputTemporal.value = contenido;
            inputTemporal.setAttribute("readonly", "");
            inputTemporal.style.position = "absolute";
            inputTemporal.style.left = "-9999px";

            document.body.appendChild(inputTemporal);
            inputTemporal.select();
            inputTemporal.setSelectionRange(0, inputTemporal.value.length);

            try {
                document.execCommand("copy");
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsExitoMfaTotpCodigosRecuperacionCopiar)%>");
            }
            catch (e) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionCopiar)%>");
            }

            document.body.removeChild(inputTemporal);
        }

        var enviandoMFA = false;
        var segundosInicialesLoginPendiente = <%=SegundosRestantesLoginPendiente%>;
        var mostrarTemporizadorLoginPendiente = <%=MostrarTemporizadorLoginPendiente ? "true" : "false"%>;
        var inicioTemporizadorLoginPendiente = Date.now();
        var temporizadorLoginPendiente = null;
        var expiracionLoginPendienteProcesada = false;

        function obtenerSegundosRestantesLoginPendiente() {
            if (!mostrarTemporizadorLoginPendiente) {
                return 0;
            }

            var segundosTranscurridos = Math.floor((Date.now() - inicioTemporizadorLoginPendiente) / 1000);

            return Math.max(0, segundosInicialesLoginPendiente - segundosTranscurridos);
        }

        function completarDosDigitos(valor) {
            return valor < 10 ? "0" + valor : valor.toString();
        }

        function formatearTiempoLoginPendiente(segundosTotales) {
            var minutos = Math.floor(segundosTotales / 60);
            var segundos = segundosTotales % 60;

            return completarDosDigitos(minutos) + ":" + completarDosDigitos(segundos);
        }

        function inicializarTemporizadorLoginPendiente() {
            if (!mostrarTemporizadorLoginPendiente) {
                return;
            }

            if (temporizadorLoginPendiente !== null) {
                window.clearInterval(temporizadorLoginPendiente);
            }

            actualizarTemporizadorLoginPendiente();

            if (expiracionLoginPendienteProcesada) {
                return;
            }

            temporizadorLoginPendiente = window.setInterval(function () {
                actualizarTemporizadorLoginPendiente();
            }, 1000);
        }

        function actualizarTemporizadorLoginPendiente() {
            var segundosRestantes = obtenerSegundosRestantesLoginPendiente();
            var controlTiempo = document.getElementById("lblTiempoLoginPendiente");

            if (controlTiempo) {
                controlTiempo.textContent = formatearTiempoLoginPendiente(segundosRestantes);
            }

            if (segundosRestantes <= 0) {
                if (temporizadorLoginPendiente !== null) {
                    window.clearInterval(temporizadorLoginPendiente);
                    temporizadorLoginPendiente = null;
                }

                procesarExpiracionLoginPendiente();
            }
        }

        function procesarExpiracionLoginPendiente() {
            if (expiracionLoginPendienteProcesada) {
                return;
            }

            expiracionLoginPendienteProcesada = true;
            enviandoMFA = true;

            $("#divTiempoLoginPendiente").removeClass("alert-light").addClass("alert-warning");
            $("#lblDescripcionTiempoLoginPendiente").text("La autenticación temporal ha expirado. Volviendo al inicio de sesión...");
            $("#lblTiempoLoginPendiente").text("00:00");

            $("#<%=btnValidar.ClientID%>").prop("disabled", true).css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnValidarRecuperacion.ClientID%>").prop("disabled", true).css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnContinuar.ClientID%>").prop("disabled", true).css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnUsarCodigoRecuperacion.ClientID%>").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnUsarCodigoTotp.ClientID%>").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnVolver.ClientID%>").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorWarning%>", "", "La autenticación temporal ha expirado. Debe iniciar sesión nuevamente.");

            window.setTimeout(function () {
                __doPostBack("<%=btnVolver.UniqueID%>", "");
            }, 1500);
        }

        function prepararEnvioMFA() {
            if (mostrarTemporizadorLoginPendiente && obtenerSegundosRestantesLoginPendiente() <= 0) {
                procesarExpiracionLoginPendiente();
                return false;
            }

            if (enviandoMFA) {
                return false;
            }

            var formulario = document.getElementById("frmMFA");
            var campo = document.getElementById("<%=txtCodigo.ClientID%>");

            if (!formulario.checkValidity()) {
                formulario.reportValidity();
                return false;
            }

            if (!campo || campo.value.length !== <%=ConstantesModelo.ConsMfaTotpCodigoLargo%>) {
                $("#<%=txtCodigo.ClientID%>").removeClass("is-valid").addClass("is-invalid");
                return false;
            }

            enviandoMFA = true;

            $("#<%=btnValidar.ClientID%>").val("Validando...").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnVolver.ClientID%>").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            return true;
        }

        function prepararEnvioCodigoRecuperacion() {
            if (mostrarTemporizadorLoginPendiente && obtenerSegundosRestantesLoginPendiente() <= 0) {
                procesarExpiracionLoginPendiente();
                return false;
            }

            if (enviandoMFA) {
                return false;
            }

            var campo = document.getElementById("<%=txtCodigoRecuperacion.ClientID%>");
            var separador = "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsMfaTotpCodigoRecuperacionSeparador)%>";

            if (!campo || campo.value.replace(separador, "").length !== <%=ConstantesModelo.ConsMfaTotpCodigoRecuperacionLargo%>) {
                $("#<%=txtCodigoRecuperacion.ClientID%>").removeClass("is-valid").addClass("is-invalid");
                return false;
            }

            enviandoMFA = true;

            $("#<%=btnValidarRecuperacion.ClientID%>").val("Validando...").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnUsarCodigoTotp.ClientID%>").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            $("#<%=btnVolver.ClientID%>").css({
                "pointer-events": "none",
                "opacity": "0.65"
            });

            return true;
        }

        $(document).ready(function () {
            inicializarMFA();
            inicializarCodigoRecuperacion();
            inicializarTemporizadorLoginPendiente();
        });

    </script>
</head>

<body class="app sidebar-mini login-img">
    <form id="frmMFA" runat="server" autocomplete="off" novalidate="novalidate">
        <asp:ScriptManager ID="smMFA" runat="server"></asp:ScriptManager>

        <div id="global-loader">
            <img src="/assets/images/svgs/loader.svg" alt="loader" />
        </div>

        <div class="page main-signin-wrapper">
            <div class="row text-center ps-0 pe-0 ms-0 me-0">
                <div class="<%=pnlEnrolamiento.Visible || pnlCodigosRecuperacion.Visible ? "col-xl-4 col-lg-6 col-md-6 d-block mx-auto" : "col-xl-3 col-lg-5 col-md-5 d-block mx-auto"%>">
                    <div class="card custom-card">

                        <div class="text-center mb-2 pt-3">
                            <a href="javascript:void(0);" onclick="return false;" tabindex="-1" aria-label="Logo DIPROTEC">
                                <img src="/assets/images/brand/logo.png" class="header-brand-img" alt="Diprotec" />
                                <img src="/assets/images/brand/logo1.png" class="header-brand-img theme-logos" alt="Diprotec" />
                            </a>
                        </div>

                        <div class="card-body pd-25">

                            <% if (MostrarTemporizadorLoginPendiente) { %>

                                <div id="divTiempoLoginPendiente" class="alert alert-light border py-2 mb-3 text-center">
                                    <span id="lblDescripcionTiempoLoginPendiente" class="tx-13 text-muted">
                                        La autenticación temporal expira en
                                    </span>

                                    <span id="lblTiempoLoginPendiente" class="tx-13 fw-semibold text-dark ms-1">
                                        --:--
                                    </span>
                                </div>

                            <% } %>

                            <asp:Panel ID="pnlFormulario" runat="server">

                                <asp:Panel ID="pnlEnrolamiento" runat="server" Visible="false">

                                    <div class="mb-3">
                                        <i class="fa fa-shield text-primary" style="font-size: 46px;"></i>
                                    </div>

                                    <asp:Panel ID="pnlEnrolamientoDetalle" runat="server" Visible="false">

                                        <h4 class="text-center mb-3">
                                            Configurar autenticación multifactor
                                        </h4>

                                        <p class="text-muted mb-2" style="font-size: 14px; line-height: 1.6;">
                                            Abra su aplicación de autenticación y escanee el código QR para agregar esta cuenta.
                                        </p>

                                    </asp:Panel>

                                    <div class="text-center mb-3">

                                        <asp:Image
                                            ID="imgCodigoQR"
                                            runat="server"
                                            CssClass="img-fluid border rounded p-2 bg-white"
                                            Width="230px"
                                            Height="230px"
                                            AlternateText="Código QR de configuración MFA" />

                                    </div>

                                    <div class="col-xl-9 col-lg-10 col-md-10 d-block mx-auto">

                                        <asp:Panel ID="pnlConfiguracionManual" runat="server">

                                            <div class="alert alert-light border text-start mb-3">

                                                <div class="fw-semibold mb-1">
                                                    Clave de configuración manual
                                                </div>

                                                <div class="text-muted mb-3" style="font-size: 13px;">
                                                    Utilice esta clave únicamente si no puede escanear el código QR.
                                                </div>

                                                <div class="form-group mb-0">

                                                    <asp:TextBox
                                                        ID="txtClaveManual"
                                                        runat="server"
                                                        CssClass="form-control text-center"
                                                        ReadOnly="true"
                                                        autocomplete="off"
                                                        onclick="seleccionarTexto(this);">
                                                    </asp:TextBox>

                                                </div>

                                            </div>

                                        </asp:Panel>

                                    </div>

                                </asp:Panel>

                                <asp:Panel ID="pnlValidacion" runat="server" Visible="false">

                                    <div class="mb-3">
                                        <i class="fa fa-mobile text-primary" style="font-size: 52px;"></i>
                                    </div>

                                    <h4 class="text-center mb-3">
                                        Verificar identidad
                                    </h4>

                                    <p class="text-muted mb-4" style="font-size: 14px; line-height: 1.6;">
                                        Ingrese el código de seis dígitos generado por su aplicación autenticadora.
                                    </p>

                                </asp:Panel>

                                <asp:Panel ID="pnlCodigoTotp" runat="server">

                                    <div class="<%=pnlEnrolamiento.Visible ? "col-xl-9 col-lg-10 col-md-10 d-block mx-auto" : string.Empty%>">

                                        <div class="form-group text-start">

                                            <label>Código de autenticación</label>

                                            <asp:TextBox
                                                ID="txtCodigo"
                                                runat="server"
                                                CssClass="form-control text-center"
                                                MaxLength="6"
                                                placeholder="Código de 6 dígitos"
                                                required="required"
                                                autocomplete="one-time-code"
                                                inputmode="numeric"
                                                aria-describedby="feedbackCodigo">
                                            </asp:TextBox>

                                            <div id="feedbackCodigo" class="invalid-feedback">
                                                <%=ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido%>
                                            </div>

                                        </div>

                                        <asp:Button
                                            ID="btnValidar"
                                            runat="server"
                                            CssClass="btn ripple btn-main-primary btn-block"
                                            OnClick="btnValidar_Click"
                                            OnClientClick="return prepararEnvioMFA();" />

                                        <asp:LinkButton
                                            ID="btnUsarCodigoRecuperacion"
                                            runat="server"
                                            Visible="false"
                                            CssClass="d-block text-center mt-3"
                                            CausesValidation="false"
                                            OnClick="btnUsarCodigoRecuperacion_Click">
                                            Usar código de recuperación
                                        </asp:LinkButton>

                                    </div>

                                </asp:Panel>

                                <asp:Panel ID="pnlRecuperacion" runat="server" Visible="false">

                                    <div class="mb-3">
                                        <i class="fa fa-key text-primary" style="font-size: 46px;"></i>
                                    </div>

                                    <h4 class="text-center mb-3">
                                        Código de recuperación
                                    </h4>

                                    <p class="text-muted mb-4" style="font-size: 14px; line-height: 1.6;">
                                        Ingresa uno de los códigos de recuperación que guardaste al configurar la autenticación multifactor.
                                    </p>

                                    <div class="form-group text-start">

                                        <label>Código de recuperación</label>

                                        <asp:TextBox
                                            ID="txtCodigoRecuperacion"
                                            runat="server"
                                            CssClass="form-control text-center"
                                            placeholder="XXXX-XXXX"
                                            required="required"
                                            autocomplete="off"
                                            autocapitalize="characters"
                                            spellcheck="false"
                                            aria-describedby="feedbackCodigoRecuperacion">
                                        </asp:TextBox>

                                        <div id="feedbackCodigoRecuperacion" class="invalid-feedback">
                                            <%=ConstantesModelo.ConsValidacionMfaTotpCodigoRecuperacionFormatoInvalido%>
                                        </div>

                                    </div>

                                    <asp:Button
                                        ID="btnValidarRecuperacion"
                                        runat="server"
                                        CssClass="btn ripple btn-main-primary btn-block"
                                        Text="Validar código"
                                        OnClick="btnValidarRecuperacion_Click"
                                        OnClientClick="return prepararEnvioCodigoRecuperacion();" />

                                    <asp:LinkButton
                                        ID="btnUsarCodigoTotp"
                                        runat="server"
                                        CssClass="d-block text-center mt-3"
                                        CausesValidation="false"
                                        OnClick="btnUsarCodigoTotp_Click">
                                        Usar aplicación de autenticación
                                    </asp:LinkButton>

                                </asp:Panel>

                                <asp:LinkButton
                                    ID="btnVolver"
                                    runat="server"
                                    CssClass="d-block text-center mt-3 link-danger"
                                    CausesValidation="false"
                                    OnClick="btnVolver_Click">

                                    <i class="fa fa-arrow-left me-1"></i>
                                    Volver al login

                                </asp:LinkButton>

                            </asp:Panel>

                            <asp:Panel ID="pnlCodigosRecuperacion" runat="server" Visible="false" CssClass="text-center">

                                <div class="mb-3">
                                    <i class="fa fa-key text-primary" style="font-size: 46px;"></i>
                                </div>

                                <h4 class="text-center mb-3">
                                    Códigos de recuperación
                                </h4>

                                <p class="text-muted mb-3" style="font-size: 14px; line-height: 1.6;">
                                    Guarda estos códigos en un lugar seguro. Podrás utilizar uno de ellos para acceder si no tienes disponible tu aplicación de autenticación.
                                </p>

                                <div class="alert alert-warning text-start mb-3">
                                    Estos códigos se mostrarán una sola vez. Cada código podrá utilizarse una única vez.
                                </div>

                                <div class="row mb-3">

                                    <asp:Repeater
                                        ID="rptCodigosRecuperacion"
                                        runat="server"
                                        EnableViewState="false">

                                        <ItemTemplate>

                                            <div class="col-md-6 mb-2">

                                                <div class="codigo-recuperacion-mfa form-control text-center fw-semibold">
                                                    <%# Container.DataItem %>
                                                </div>

                                            </div>

                                        </ItemTemplate>

                                    </asp:Repeater>

                                </div>

                                <div class="row row-sm">

                                    <div class="col-6">

                                        <button
                                            type="button"
                                            class="btn ripple btn-outline-info btn-block"
                                            onclick="return copiarCodigosRecuperacionMFA();">
                                            Copiar códigos
                                        </button>

                                    </div>

                                    <div class="col-6">

                                        <asp:Button
                                            ID="btnContinuar"
                                            runat="server"
                                            CssClass="btn ripple btn-main-primary btn-block"
                                            Text="Continuar"
                                            CausesValidation="false"
                                            OnClick="btnContinuar_Click" />

                                    </div>

                                </div>

                            </asp:Panel>

                            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="text-center p-3">

                                <div class="mb-3">
                                    <i class="fa fa-exclamation-triangle text-warning" style="font-size: 52px;"></i>
                                </div>

                                <h4 class="text-warning mb-3">
                                    No fue posible continuar
                                </h4>

                                <p class="text-muted mb-4" style="font-size: 14px; line-height: 1.6;">
                                    <asp:Literal ID="litError" runat="server"></asp:Literal>
                                </p>

                                <asp:Button
                                    ID="btnVolverError"
                                    runat="server"
                                    CssClass="btn ripple btn-outline-secondary btn-block"
                                    Text="Volver al login"
                                    CausesValidation="false"
                                    OnClick="btnVolver_Click" />

                            </asp:Panel>

                        </div>

                    </div>

                </div>

            </div>

        </div>

        <a href="#top" id="back-to-top">
            <i class="fa fa-angle-up"></i>
        </a>

    </form>

    <div id="divAlerta" class="position-fixed bottom-0 end-0 p-3" style="z-index: 9999;"></div>

</body>
</html>