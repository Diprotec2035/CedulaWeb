<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmVisitasImportarNuevo.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.visita.frmVisitasImportarNuevo" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            $("#<%=fuArchivoCarga.ClientID%>").on("change", function () {
                validarArchivoCsv(true);
            });

        });

        function validarArchivoCsv(mostrarMensaje) {
            var input = document.getElementById("<%=fuArchivoCarga.ClientID%>");
            var campo = $("#<%=fuArchivoCarga.ClientID%>");

            campo.removeClass("is-valid is-invalid");

            if (!input || !input.value) {
                campo.addClass("is-invalid");

                if (mostrarMensaje) {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "Debe seleccionar el archivo CSV.");
                }

                return false;
            }

            var archivo = input.value.split("\\").pop().split("/").pop();
            var archivoLower = archivo.toLowerCase();

            if (!archivoLower.endsWith(".csv")) {
                campo.addClass("is-invalid");

                if (mostrarMensaje) {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "", "El archivo seleccionado debe tener extensión .CSV.");
                }

                input.value = "";
                return false;
            }

            campo.addClass("is-valid");
            return true;
        }

        function blockFormulario() {
            var formulario = document.getElementById("<%=Page.Form.ClientID%>");

            if (!formulario.checkValidity()) {
                formulario.reportValidity();
                return false;
            }

            if (!validarArchivoCsv(true)) {
                return false;
            }

            $("#<%=btnProcesar.ClientID%>").val("Procesando...");
            $("#<%=btnVolver.ClientID%>").prop("disabled", true);

            setTimeout(function () {
                $("#<%=btnProcesar.ClientID%>").prop("disabled", true);
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
                                                <label class="mg-b-0">Archivo</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:FileUpload
                                                    ID="fuArchivoCarga"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    accept=".csv"
                                                    required="required"
                                                    aria-describedby="feedbackArchivoCarga" />

                                                <div id="feedbackArchivoCarga" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionCargaArchivoCsvVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-xl-10 col-lg-10 col-md-12 mx-auto">
                                                <div class="alert alert-info mb-0 py-2 px-3 text-center" role="alert">
                                                    <i class="fe fe-info me-1"></i>
                                                    <strong>Formato esperado:</strong>
                                                    el archivo debe contener las columnas de RUT, nombre y empresa de la visita.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center">
                                            <div class="col-md-4"></div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:Button
                                                    ID="btnProcesar"
                                                    runat="server"
                                                    CssClass="btn ripple btn-main-primary"
                                                    Style="min-width: 130px;"
                                                    Text="Procesar"
                                                    OnClientClick="return blockFormulario();"
                                                    OnClick="btnProcesar_Click" />

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