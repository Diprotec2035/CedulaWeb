<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmEmpresasEditar.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.empresa.frmEmpresasEditar" %>
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

            $("#<%=btnActualizar.ClientID%>").val("Actualizando...");
            $("#<%=btnVolver.ClientID%>").prop("disabled", true);

            setTimeout(function () {
                $("#<%=btnActualizar.ClientID%>").prop("disabled", true);
            }, 0);

            return true;
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
                                                    MaxLength="10"
                                                    placeholder="RUT empresa"
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
                                                <label class="mg-b-0">Teléfono Contacto</label>
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
                                                <label class="mg-b-0">Email</label>
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
                                                <asp:HiddenField ID="hfExterno" runat="server" />

                                                <div id="tglExterno"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Estado</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfEstado" runat="server" />

                                                <div id="tglEstado"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

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