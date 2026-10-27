<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmVersionesAplicacionEditar.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.version_aplicacion.frmVersionesAplicacionEditar" %>
<%@ Register Src="~/aplicacion/control/ucBitacora.ascx" TagPrefix="uc" TagName="Bitacora" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

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

        function blockFormulario() {
            var formulario = document.getElementById("<%=Page.Form.ClientID%>");

            if (!formulario.checkValidity()) {
                formulario.reportValidity();
                return false;
            }

            if (!validarRelacionPublicadoEstado()) {
                return false;
            }

            $("#<%=btnActualizar.ClientID%>").val("Actualizando...");
            $("#<%=btnVolver.ClientID%>").prop("disabled", true);

            setTimeout(function () {
                $("#<%=btnActualizar.ClientID%>").prop("disabled", true);
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
                                                    ReadOnly="true">
                                                </asp:TextBox>
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
                                                    ReadOnly="true">
                                                </asp:TextBox>
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
                                                    ReadOnly="true">
                                                </asp:TextBox>
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
                                                    ReadOnly="true">
                                                </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Android Versi&oacute;n</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtAndroidVersion"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    Style="width: 80px;"
                                                    ReadOnly="true">
                                                </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Versi&oacute;n</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtVersionName"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    Style="width: 120px;"
                                                    ReadOnly="true">
                                                </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Archivo APK</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HyperLink
                                                    ID="lnkApk"
                                                    runat="server"
                                                    CssClass="text-primary" />
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Forzar Actualizaci&oacute;n</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfForceUpdate" runat="server" />

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
                                                <asp:HiddenField ID="hfIsActive" runat="server" />

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
                                                <asp:HiddenField ID="hfIsPublished" runat="server" />

                                                <div id="tglIsPublished"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
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