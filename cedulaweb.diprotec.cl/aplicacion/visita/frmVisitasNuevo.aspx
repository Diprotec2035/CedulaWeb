<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmVisitasNuevo.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.visita.frmVisitasNuevo" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            var campoRut = $("#<%=txtRut.ClientID%>");

            campoRut.off(".rutVisita");

            campoRut.on("input.rutVisita", function () {
                var campo = $(this);
                var valor = campo.val().toUpperCase().replace(/[^0-9K]/g, "");
                var terminaEnK = valor.charAt(valor.length - 1) === "K";
                var numeros = valor.replace(/K/g, "");

                numeros = numeros.substring(0, terminaEnK ? 8 : 9);

                campo.val(numeros + (terminaEnK ? "K" : ""));
                campo.removeClass("is-valid is-invalid");
            });

            campoRut.on("blur.rutVisita", function () {
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

            $("#<%=txtNombre.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombrePersona)%>");
            $("#<%=txtEmpresa.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombreEmpresa)%>");
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
                                                <label class="mg-b-0">RUT</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtRut"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="10"
                                                    placeholder="12345678-9"
                                                    required="required"
                                                    aria-describedby="feedbackRut">
                                                </asp:TextBox>
                                                <div id="feedbackRut" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVisitaRutInvalido%>
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
                                                    MaxLength="150"
                                                    placeholder="nombre de la visita"
                                                    required="required"
                                                    aria-describedby="feedbackNombre">
                                                </asp:TextBox>
                                                <div id="feedbackNombre" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionVisitaNombreVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Empresa</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtEmpresa"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="50"
                                                    placeholder="empresa de la visita">
                                                </asp:TextBox>
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
