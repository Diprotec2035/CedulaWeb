<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmSuperAdministradoresEditar.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.super_admin.frmSuperAdministradoresEditar" %>
<%@ Register Src="~/aplicacion/control/ucBitacora.ascx" TagPrefix="uc" TagName="Bitacora" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
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

            $("#<%=txtApellido.ClientID%>").on("blur", function () {
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

            $("#<%=txtPassword.ClientID%>").on("blur", function () {
                validarPasswordEdicion();
            });

            $("#<%=txtPasswordConfirmar.ClientID%>").on("blur", function () {
                validarPasswordEdicion();
            });

            $("#<%=txtTelefono.ClientID%>").on("blur", function () {
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

            $("#<%=txtEmail.ClientID%>").off("blur").on("blur", function () {
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

            $("#<%=txtNombre.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombrePersona)%>");
            $("#<%=txtApellido.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombrePersona)%>");
            $("#<%=txtPassword.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoPassword)%>");
            $("#<%=txtPasswordConfirmar.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoPassword)%>");
            $("#<%=txtTelefono.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTelefono)%>");
            $("#<%=txtEmail.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoEmail)%>");

            inicializarSeguridadMFA();
        });

        function validarPasswordEdicion() {
            var campoPassword = $("#<%=txtPassword.ClientID%>");
            var campoPasswordConfirmar = $("#<%=txtPasswordConfirmar.ClientID%>");
            var password = campoPassword.val();
            var passwordConfirmar = campoPasswordConfirmar.val();

            campoPassword.removeClass("is-valid is-invalid");
            campoPasswordConfirmar.removeClass("is-valid is-invalid");

            if (password === "" && passwordConfirmar === "") {
                return true;
            }

            if (password === "") {
                campoPassword.addClass("is-invalid");
            }
            else {
                campoPassword.addClass("is-valid");
            }

            if (passwordConfirmar !== "" && password === passwordConfirmar) {
                campoPasswordConfirmar.addClass("is-valid");
            }
            else {
                campoPasswordConfirmar.addClass("is-invalid");
            }

            return password !== "" && passwordConfirmar !== "" && password === passwordConfirmar;
        }

        function blockFormulario() {
            var formulario = document.getElementById("<%=Page.Form.ClientID%>");

            if (!formulario.checkValidity()) {
                formulario.reportValidity();
                return false;
            }

            if (!validarPasswordEdicion()) {
                return false;
            }

            $("#<%=btnActualizar.ClientID%>").val("Actualizando...");
            $("#<%=btnVolver.ClientID%>").prop("disabled", true);

            setTimeout(function () {
                $("#<%=btnActualizar.ClientID%>").prop("disabled", true);
            }, 0);

            return true;
        }

        function inicializarSeguridadMFA() {
            var campo = $("#<%=txtCodigoMFA.ClientID%>");

            campo.off("input.seguridadMFA").on("input.seguridadMFA", function () {
                var control = $(this);
                var codigo = control.val().replace(/[^0-9]/g, "").substring(0, <%=ConstantesModelo.ConsMfaTotpCodigoLargo%>);

                control.val(codigo);
                control.removeClass("is-valid is-invalid");
            });

            campo.off("blur.seguridadMFA").on("blur.seguridadMFA", function () {
                var control = $(this);
                var codigo = control.val();

                control.removeClass("is-valid is-invalid");

                if (codigo.length === <%=ConstantesModelo.ConsMfaTotpCodigoLargo%>) {
                    control.addClass("is-valid");
                }
                else if (codigo.length > 0) {
                    control.addClass("is-invalid");
                }
            });
        }

        function prepararRegeneracionMFA() {
            var campo = $("#<%=txtCodigoMFA.ClientID%>");
            var codigo = campo.val();

            campo.removeClass("is-valid is-invalid");

            if (codigo.length !== <%=ConstantesModelo.ConsMfaTotpCodigoLargo%>) {
                campo.addClass("is-invalid");
                campo.focus();
                return false;
            }

            campo.addClass("is-valid");

            $("#<%=btnRegenerarCodigos.ClientID%>").val("Regenerando...");

            setTimeout(function () {
                $("#<%=btnRegenerarCodigos.ClientID%>").prop("disabled", true);
            }, 0);

            return true;
        }

        function obtenerCodigosRecuperacionPerfil() {
            var codigos = [];

            $(".codigo-recuperacion-perfil").each(function () {
                var codigo = $(this).text().trim();

                if (codigo !== "") {
                    codigos.push(codigo);
                }
            });

            return codigos.join("\r\n");
        }

        function copiarCodigosRecuperacionPerfil() {
            var contenido = obtenerCodigosRecuperacionPerfil();

            if (contenido === "") {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionCopiar)%>");
                return false;
            }

            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(contenido).then(function () {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsExitoMfaTotpCodigosRecuperacionCopiar)%>");
                }).catch(function () {
                    copiarCodigosRecuperacionPerfilFallback(contenido);
                });

                return false;
            }

            copiarCodigosRecuperacionPerfilFallback(contenido);
            return false;
        }

        function copiarCodigosRecuperacionPerfilFallback(contenido) {
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

                            <div class="card-header d-flex align-items-center justify-content-between">
                                <h6 class="card-title mb-0"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>

                                <asp:Button
                                    ID="btnVerBitacora"
                                    runat="server"
                                    CssClass="btn ripple btn-outline-info"
                                    Text="Ver bitácora"
                                    OnClientClick="return abrirBitacora();" />
                            </div>

                            <div class="card-body">
                                <div class="row">

                                    <div class="<%=pnlSeguridadMFA.Visible ? "col-xl-6 col-lg-6 col-md-12" : "col-xl-8 col-lg-10 col-md-12 mx-auto"%>">

                                        <div class="card custom-card mb-0">

                                            <div class="card-header">
                                                <h6 class="card-title mb-0"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>
                                            </div>

                                            <div class="card-body">

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">RUT</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                        <asp:TextBox
                                                            ID="txtRut"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            MaxLength="10"
                                                            placeholder="RUT super administrador"
                                                            Enabled="false"
                                                            autocomplete="off">
                                                        </asp:TextBox>
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
                                                            placeholder="nombre super administrador"
                                                            required="required"
                                                            aria-describedby="feedbackNombre">
                                                        </asp:TextBox>

                                                        <div id="feedbackNombre" class="invalid-feedback">
                                                            <%=ConstantesModelo.ConsValidacionSuperAdministradorNombreVacio%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">Apellido</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                        <asp:TextBox
                                                            ID="txtApellido"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            MaxLength="100"
                                                            placeholder="apellido super administrador"
                                                            required="required"
                                                            aria-describedby="feedbackApellido">
                                                        </asp:TextBox>

                                                        <div id="feedbackApellido" class="invalid-feedback">
                                                            <%=ConstantesModelo.ConsValidacionSuperAdministradorApellidoVacio%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">Contrase&ntilde;a</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                        <asp:TextBox
                                                            ID="txtPassword"
                                                            TextMode="Password"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            MaxLength="20"
                                                            placeholder="password super administrador"
                                                            autocomplete="new-password"
                                                            aria-describedby="feedbackPassword">
                                                        </asp:TextBox>

                                                        <div id="feedbackPassword" class="invalid-feedback">
                                                            <%=ConstantesModelo.ConsValidacionSuperAdministradorPasswordVacio%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">Confirmar Contrase&ntilde;a</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                        <asp:TextBox
                                                            ID="txtPasswordConfirmar"
                                                            TextMode="Password"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            MaxLength="20"
                                                            placeholder="password super administrador"
                                                            autocomplete="new-password"
                                                            aria-describedby="feedbackPasswordConfirmar">
                                                        </asp:TextBox>

                                                        <div id="feedbackPasswordConfirmar" class="invalid-feedback">
                                                            <%=ConstantesModelo.ConsValidacionSuperAdministradorPasswordConfirmarVacio%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">Teléfono</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                        <asp:TextBox
                                                            ID="txtTelefono"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            MaxLength="10"
                                                            placeholder="tel&eacute;fono super administrador"
                                                            required="required"
                                                            aria-describedby="feedbackTelefono">
                                                        </asp:TextBox>

                                                        <div id="feedbackTelefono" class="invalid-feedback">
                                                            <%=ConstantesModelo.ConsValidacionSuperAdministradorTelefonoVacio%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">Email</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                        <asp:TextBox
                                                            ID="txtEmail"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            MaxLength="100"
                                                            placeholder="email super administrador"
                                                            required="required"
                                                            aria-describedby="feedbackEmail">
                                                        </asp:TextBox>

                                                        <div id="feedbackEmail" class="invalid-feedback">
                                                            <%=ConstantesModelo.ConsErrorSuperAdministradorEmail%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <asp:Panel ID="pnlEstado" runat="server">

                                                    <div class="row row-xs align-items-center mg-b-20">
                                                        <div class="col-md-4">
                                                            <label class="mg-b-0">Estado</label>
                                                        </div>

                                                        <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                            <asp:HiddenField ID="hfEstado" runat="server" />

                                                            <div
                                                                id="tglEstado"
                                                                runat="server"
                                                                class="main-toggle br-3 text-center">
                                                                <span></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </asp:Panel>

                                                <div class="row row-xs align-items-center">
                                                    <div class="col-md-4"></div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">

                                                        <asp:Button
                                                            ID="btnActualizar"
                                                            runat="server"
                                                            CssClass="btn ripple btn-main-primary"
                                                            Style="min-width: 130px;"
                                                            Text="Actualizar"
                                                            OnClientClick="return blockFormulario();"
                                                            OnClick="btnActualizar_Click" />

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

                                    <asp:Panel
                                        ID="pnlSeguridadMFA"
                                        runat="server"
                                        Visible="false"
                                        CssClass="col-xl-6 col-lg-6 col-md-12">

                                        <div class="card custom-card mb-0">

                                            <div class="card-header">
                                                <h6 class="card-title mb-0">Seguridad MFA</h6>
                                            </div>

                                            <div class="card-body">

                                                <asp:Panel ID="pnlRegenerarCodigos" runat="server" Visible="false">

                                                    <div class="mg-b-25">
                                                        <label class="mg-b-5 d-block">Regenerar códigos de recuperación</label>

                                                        <div class="text-muted" style="text-align: justify;">
                                                            Ingrese un código actual de su aplicación de autenticación para generar 10 códigos nuevos. Los códigos anteriores quedarán invalidados. Si acaba de iniciar sesión, espere a que su aplicación genere el siguiente código.
                                                        </div>
                                                    </div>

                                                    <div class="row row-xs align-items-center mg-b-20">
                                                        <div class="col-md-4">
                                                            <label class="mg-b-0">Código de autenticación</label>
                                                        </div>

                                                        <div class="col-md-8 mg-t-5 mg-md-t-0">

                                                            <asp:TextBox
                                                                ID="txtCodigoMFA"
                                                                runat="server"
                                                                CssClass="form-control text-center"
                                                                Style="width: 130px;"
                                                                MaxLength="6"
                                                                placeholder="000000"
                                                                autocomplete="one-time-code"
                                                                inputmode="numeric"
                                                                aria-describedby="feedbackCodigoMFA">
                                                            </asp:TextBox>

                                                            <div id="feedbackCodigoMFA" class="invalid-feedback" style="width: 250px;">
                                                                <%=ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido%>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="row row-xs align-items-center">
                                                        <div class="col-md-4"></div>

                                                        <div class="col-md-8 mg-t-5 mg-md-t-0">

                                                            <asp:Button
                                                                ID="btnRegenerarCodigos"
                                                                runat="server"
                                                                CssClass="btn ripple btn-main-primary"
                                                                Style="min-width: 130px;"
                                                                Text="Regenerar"
                                                                CausesValidation="false"
                                                                UseSubmitBehavior="false"
                                                                OnClientClick="if (!prepararRegeneracionMFA()) { return false; }"
                                                                OnClick="btnRegenerarCodigos_Click" />

                                                        </div>
                                                    </div>

                                                </asp:Panel>

                                                <asp:Panel ID="pnlCodigosRecuperacion" runat="server" Visible="false">

                                                    <div class="mg-b-20">
                                                        <label class="mg-b-5 d-block">Nuevos códigos de recuperación</label>

                                                        <div class="text-muted" style="text-align: justify;">
                                                            Guarde estos códigos en un lugar seguro. Se mostrarán una sola vez y cada código puede utilizarse una sola vez.
                                                        </div>
                                                    </div>

                                                    <div class="row">

                                                        <div class="col-md-10 mx-auto">

                                                            <div class="row g-2">

                                                                <asp:Repeater ID="rptCodigosRecuperacion" runat="server" EnableViewState="false">

                                                                    <ItemTemplate>

                                                                        <div class="col-md-6">

                                                                            <div class="codigo-recuperacion-perfil form-control text-center fw-semibold">
                                                                                <%#: Container.DataItem %>
                                                                            </div>

                                                                        </div>

                                                                    </ItemTemplate>

                                                                </asp:Repeater>

                                                            </div>

                                                            <div class="d-flex justify-content-center mg-t-20">

                                                                <button
                                                                    type="button"
                                                                    class="btn ripple btn-main-primary"
                                                                    style="width: 130px;"
                                                                    onclick="return copiarCodigosRecuperacionPerfil();">
                                                                    Copiar códigos
                                                                </button>

                                                            </div>

                                                        </div>

                                                    </div>

                                                </asp:Panel>

                                            </div>

                                        </div>

                                    </asp:Panel>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Row -->

                <uc:Bitacora ID="ucBitacora" runat="server" />

            </div>
        </div>
    </div>
    <!-- End Main Content-->

</asp:Content>