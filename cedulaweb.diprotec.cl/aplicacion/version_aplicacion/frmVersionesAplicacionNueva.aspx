<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmVersionesAplicacionNueva.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.version_aplicacion.frmVersionesAplicacionNueva" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            $("#<%=txtFabricante.ClientID%>").on("blur", function () {
                validarTextoSimple($(this));
            });

            $("#<%=txtFabricante.ClientID%>").on("input", function () {
                setTimeout(function () {
                    $("#<%=txtFabricanteApk.ClientID%>").val($("#<%=txtFabricante.ClientID%>").val()).trigger("input");
                    limpiarValidacionApk();
                }, 0);
            });

            $("#<%=txtModelo.ClientID%>").on("blur", function () {
                validarTextoSimple($(this));
            });

            $("#<%=txtModelo.ClientID%>").on("input", function () {
                setTimeout(function () {
                    $("#<%=txtModeloApk.ClientID%>").val($("#<%=txtModelo.ClientID%>").val()).trigger("input");
                    limpiarValidacionApk();
                }, 0);
            });

            $("#<%=txtFabricanteApk.ClientID%>").on("blur", function () {
                validarTextoApk($(this));
                limpiarValidacionApk();
            });

            $("#<%=txtModeloApk.ClientID%>").on("blur", function () {
                validarTextoApk($(this));
                limpiarValidacionApk();
            });

            $("#<%=ddlAndroidVersion.ClientID%>").on("change", function () {
                validarDropDown($(this));
            });

            $("#<%=ddlVersionMayor.ClientID%>, #<%=ddlVersionMenor.ClientID%>, #<%=ddlVersionRevision.ClientID%>").on("change", function () {
                validarVersion(false);
                limpiarValidacionApk();
            });

            $("#<%=fuApk.ClientID%>").on("change", function () {
                validarNombreArchivoApk(true);
            });

            $("#<%=txtReleaseNotes.ClientID%>").on("blur", function () {
                validarTextoSimple($(this));
            });

            $("#<%=tglIsPublished.ClientID%>").on("click", function () {
                setTimeout(function () {
                    validarRelacionPublicadoEstado();
                }, 50);
            });

            $("#<%=tglIsActive.ClientID%>").on("click", function () {
                setTimeout(function () {
                    desactivarPublicadoSiNoEstado();
                }, 50);
            });

            $("#<%=txtFabricante.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoFabricanteVersionAplicacion)%>");
            $("#<%=txtModelo.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoModeloVersionAplicacion)%>");
            $("#<%=txtFabricanteApk.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoIdentificadorApkVersionAplicacion)%>");
            $("#<%=txtModeloApk.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoIdentificadorApkVersionAplicacion)%>");
            $("#<%=txtReleaseNotes.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoComentariosVersionAplicacion)%>");
        });

        function validarTextoSimple(campo) {
            var valor = campo.val().trim();

            campo.removeClass("is-valid is-invalid");

            if (valor.length > 0) {
                campo.addClass("is-valid");
            }
            else {
                campo.addClass("is-invalid");
            }
        }

        function validarTextoApk(campo) {
            var valor = campo.val().trim();

            campo.removeClass("is-valid is-invalid");

            if (valor.length > 0) {
                campo.addClass("is-valid");
            }
            else {
                campo.addClass("is-invalid");
            }
        }

        function validarDropDown(campo) {
            campo.removeClass("is-valid is-invalid");

            if (campo.val() !== "") {
                campo.addClass("is-valid");
            }
            else {
                campo.addClass("is-invalid");
            }
        }

        function obtenerNumero(controlId) {
            var valor = $("#" + controlId).val();

            if (valor === null || valor === "") {
                return 0;
            }

            var numero = parseInt(valor, 10);

            return isNaN(numero) ? 0 : numero;
        }

        function obtenerVersionAndroidActual() {
            var androidVersion = "A" + $("#<%=ddlAndroidVersion.ClientID%>").val();

            return androidVersion;
        }

        function obtenerVersionNameActual() {
            var mayor = obtenerNumero("<%=ddlVersionMayor.ClientID%>");
            var menor = obtenerNumero("<%=ddlVersionMenor.ClientID%>");
            var revision = obtenerNumero("<%=ddlVersionRevision.ClientID%>");

            return mayor + "." + menor + "." + revision;
        }

        function obtenerVersionCodeActual() {
            var mayor = obtenerNumero("<%=ddlVersionMayor.ClientID%>");
            var menor = obtenerNumero("<%=ddlVersionMenor.ClientID%>");
            var revision = obtenerNumero("<%=ddlVersionRevision.ClientID%>");

            return (mayor * 10000) + (menor * 100) + revision;
        }

        function obtenerFabricanteApkNormalizado() {
            var fabricanteApk = $("#<%=txtFabricanteApk.ClientID%>").val().trim();

            return fabricanteApk.toUpperCase();
        }

        function obtenerModeloApkNormalizado() {
            var modeloApk = $("#<%=txtModeloApk.ClientID%>").val().trim();

            return modeloApk.toUpperCase();
        }

        function obtenerNombreApkEsperado() {
            var fabricanteApk = obtenerFabricanteApkNormalizado();
            var modeloApk = obtenerModeloApkNormalizado();
            var versionAndroid = obtenerVersionAndroidActual();
            var versionName = obtenerVersionNameActual();

            if (fabricanteApk.length === 0 || modeloApk.length === 0) {
                return "";
            }

            return fabricanteApk + "_" + modeloApk + "_" + versionAndroid + "_CEDULA_" + versionName + ".apk";
        }

        function validarVersion(mostrarMensaje) {
            var versionCode = obtenerVersionCodeActual();

            var controles = $(
                "#<%=ddlVersionMayor.ClientID%>, " +
                "#<%=ddlVersionMenor.ClientID%>, " +
                "#<%=ddlVersionRevision.ClientID%>"
            );

            controles.removeClass("is-valid is-invalid");

            if (versionCode <= 0) {
                controles.addClass("is-invalid");

                if (mostrarMensaje) {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "La versión actual debe ser mayor a 0.0.0.");
                }

                return false;
            }

            controles.addClass("is-valid");
            return true;
        }

        function validarCamposApk(mostrarMensaje) {
            var fabricanteApk = $("#<%=txtFabricanteApk.ClientID%>");
            var modeloApk = $("#<%=txtModeloApk.ClientID%>");

            var fabricante = fabricanteApk.val().trim();
            var modelo = modeloApk.val().trim();

            var valido = true;

            fabricanteApk.removeClass("is-valid is-invalid");
            modeloApk.removeClass("is-valid is-invalid");

            if (fabricante.length === 0) {
                fabricanteApk.addClass("is-invalid");
                valido = false;
            }
            else {
                fabricanteApk.addClass("is-valid");
            }

            if (modelo.length === 0) {
                modeloApk.addClass("is-invalid");
                valido = false;
            }
            else {
                modeloApk.addClass("is-valid");
            }

            if (!valido && mostrarMensaje) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "Debe ingresar fabricante APK y modelo APK.");
            }

            return valido;
        }

        function validarNombreArchivoApk(mostrarMensaje) {
            var input = document.getElementById("<%=fuApk.ClientID%>");
            var campo = $("#<%=fuApk.ClientID%>");

            campo.removeClass("is-valid is-invalid");

            if (!input || !input.value) {
                campo.addClass("is-invalid");

                if (mostrarMensaje) {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "Debe seleccionar el archivo APK.");
                }

                return false;
            }

            if (!validarCamposApk(mostrarMensaje)) {
                campo.addClass("is-invalid");
                input.value = "";
                return false;
            }

            if (!validarVersion(mostrarMensaje)) {
                campo.addClass("is-invalid");
                input.value = "";
                return false;
            }

            var archivo = input.value.split("\\").pop().split("/").pop();
            var archivoLower = archivo.toLowerCase();

            if (!archivoLower.endsWith(".apk")) {
                campo.addClass("is-invalid");

                if (mostrarMensaje) {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "El archivo seleccionado debe tener extensión .apk.");
                }

                input.value = "";
                return false;
            }

            var esperado = obtenerNombreApkEsperado();

            if (esperado.length === 0) {
                campo.addClass("is-invalid");

                if (mostrarMensaje) {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "Debe ingresar fabricante APK y modelo APK para validar el archivo.");
                }

                input.value = "";
                return false;
            }

            if (archivo !== esperado) {
                campo.addClass("is-invalid");

                if (mostrarMensaje) {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "El archivo APK debe llamarse " + esperado + ".");
                }

                input.value = "";
                return false;
            }

            campo.addClass("is-valid");
            return true;
        }

        function limpiarValidacionApk() {
            $("#<%=fuApk.ClientID%>").removeClass("is-valid is-invalid");
        }

        function blockFormulario() {
            var formulario = document.getElementById("<%=Page.Form.ClientID%>");

            if (!formulario.checkValidity()) {
                formulario.reportValidity();
                return false;
            }

            if (!validarVersion(true)) {
                return false;
            }

            if (!validarCamposApk(true)) {
                return false;
            }

            if (!validarNombreArchivoApk(true)) {
                return false;
            }

            if (!validarRelacionPublicadoEstado()) {
                return false;
            }

            $("#<%=btnRegistrar.ClientID%>").val("Registrando...");
            $("#<%=btnVolver.ClientID%>").prop("disabled", true);

            setTimeout(function () {
                $("#<%=btnRegistrar.ClientID%>").prop("disabled", true);
            }, 0);

            return true;
        }

        function validarRelacionPublicadoEstado() {
            var publicado = $("#<%=hfIsPublished.ClientID%>").val();
            var estado = $("#<%=hfIsActive.ClientID%>").val();

            if (publicado === "0" && estado !== "0") {
                $("#<%=hfIsActive.ClientID%>").val("0");
                $("#<%=tglIsActive.ClientID%>").addClass("on");

                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorWarning%>", "", "Una versión publicada debe estar activa.");

                return false;
            }

            return true;
        }

        function desactivarPublicadoSiNoEstado() {
            var estado = $("#<%=hfIsActive.ClientID%>").val();

            if (estado !== "0") {
                $("#<%=hfIsPublished.ClientID%>").val("1");
                $("#<%=tglIsPublished.ClientID%>").removeClass("on");
            }
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
                                                <label class="mg-b-0">Fabricante Dispositivo</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtFabricante"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="50"
                                                    placeholder="fabricante informado por el dispositivo"
                                                    required="required"
                                                    aria-describedby="feedbackFabricante">
                                                </asp:TextBox>
                                                <div id="feedbackFabricante" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVersionAplicacionFabricanteVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Modelo Dispositivo</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtModelo"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="modelo informado por el dispositivo"
                                                    required="required"
                                                    aria-describedby="feedbackModelo">
                                                </asp:TextBox>
                                                <div id="feedbackModelo" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVersionAplicacionModeloVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Fabricante APK</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtFabricanteApk"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="50"
                                                    placeholder="ej: ZEBRA"
                                                    required="required"
                                                    aria-describedby="feedbackFabricanteApk">
                                                </asp:TextBox>
                                                <div id="feedbackFabricanteApk" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVersionAplicacionFabricanteApkVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Modelo APK</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtModeloApk"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="ej: TC27"
                                                    required="required"
                                                    aria-describedby="feedbackModeloApk">
                                                </asp:TextBox>
                                                <div id="feedbackModeloApk" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVersionAplicacionModeloApkVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Android Versi&oacute;n</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlAndroidVersion"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    Style="width: 80px;"
                                                    required="required"
                                                    aria-describedby="feedbackAndroidVersion">
                                                </asp:DropDownList>

                                                <div id="feedbackAndroidVersion" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVersionAplicacionAndroidVersionVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Versión</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <div class="d-flex align-items-center gap-2">
                                                    <asp:DropDownList
                                                        ID="ddlVersionMayor"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        Style="width: 80px;"
                                                        required="required">
                                                    </asp:DropDownList>

                                                    <span>.</span>

                                                    <asp:DropDownList
                                                        ID="ddlVersionMenor"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        Style="width: 80px;">
                                                    </asp:DropDownList>

                                                    <span>.</span>

                                                    <asp:DropDownList
                                                        ID="ddlVersionRevision"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        Style="width: 80px;">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Forzar Actualizaci&oacute;n</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfForceUpdate" runat="server" Value="1" />

                                                <div id="tglForceUpdate"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Activo</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfIsActive" runat="server" Value="1" />

                                                <div id="tglIsActive"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Publicado</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfIsPublished" runat="server" Value="1" />

                                                <div id="tglIsPublished"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Archivo APK</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:FileUpload
                                                    ID="fuApk"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    accept=".apk"
                                                    required="required"
                                                    aria-describedby="feedbackApk" />
                                                <div id="feedbackApk" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVersionAplicacionApkVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Comentarios</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtReleaseNotes"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="2000"
                                                    TextMode="MultiLine"
                                                    Rows="4"
                                                    placeholder="comentarios de la versión"
                                                    required="required"
                                                    aria-describedby="feedbackReleaseNotes">
                                                </asp:TextBox>
                                                <div id="feedbackReleaseNotes" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVersionAplicacionComentariosVacio%>
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

</asp:Content>