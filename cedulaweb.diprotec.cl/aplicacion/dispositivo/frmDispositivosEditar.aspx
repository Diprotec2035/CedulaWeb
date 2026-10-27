<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmDispositivosEditar.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.dispositivo.frmDispositivosEditar" %>
<%@ Register Src="~/aplicacion/control/ucBitacora.ascx" TagPrefix="uc" TagName="Bitacora" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            $("#<%=ddlMarca.ClientID%>").on("change blur", function () {
                var campo = $(this);
                var valor = campo.val();

                campo.removeClass("is-valid is-invalid");

                if (valor !== null && valor !== "") {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=ddlModelo.ClientID%>").on("change blur", function () {
                var campo = $(this);
                var valor = campo.val();

                campo.removeClass("is-valid is-invalid");

                if (valor !== null && valor !== "") {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=txtComentarios.ClientID%>").on("blur", function () {
                var campo = $(this);
                var valor = campo.val().trim();

                campo.removeClass("is-valid is-invalid");

                if (valor.length > 0) {
                    campo.addClass("is-valid");
                }
            });

            $("#<%=txtComentarios.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoComentarioDispositivo)%>");

            inicializarToggleDispositivo();
        });

        function inicializarToggleDispositivo() {
            var toggle = $("#<%=tglEstado.ClientID%>");
            var hiddenEstado = $("#<%=hfEstado.ClientID%>");
            var hiddenEstadoActual = $("#<%=hfEstadoActual.ClientID%>");
            
            toggle.off("click").on("click", function () {
                var estadoActual = hiddenEstadoActual.val();

                if (estadoActual === "2") {
                    var cambiarAAnulado = confirm("El dispositivo se encuentra activado. ¿Desea cambiarlo a Anulado?\n\nAceptar: Anulado\nCancelar: Vigente");

                    if (cambiarAAnulado) {
                        hiddenEstado.val("1");
                        toggle.removeClass("on");
                    }
                    else {
                        hiddenEstado.val("0");
                        toggle.addClass("on");
                    }

                    hiddenEstadoActual.val(hiddenEstado.val());
                    return;
                }

                if (toggle.hasClass("on")) {
                    hiddenEstado.val("0");
                }
                else {
                    hiddenEstado.val("1");
                }

                hiddenEstadoActual.val(hiddenEstado.val());
            });
        }

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
                                                <label class="mg-b-0">Marca</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlMarca"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged"
                                                    required="required"
                                                    aria-describedby="feedbackMarca">
                                                </asp:DropDownList>
                                                <div id="feedbackMarca" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionDispositivoMarcaVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Modelo</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlModelo"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    required="required"
                                                    aria-describedby="feedbackModelo">
                                                </asp:DropDownList>
                                                <div id="feedbackModelo" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionDispositivoModeloVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Part Number</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox ID="txtPartnumber" runat="server" CssClass="form-control" MaxLength="50" ReadOnly="true" aria-readonly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Serial Number</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox ID="txtSerialnumber" runat="server" CssClass="form-control" MaxLength="50" ReadOnly="true" aria-readonly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Comentarios</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox ID="txtComentarios" runat="server" CssClass="form-control" MaxLength="100" placeholder="comentarios dispositivo" aria-describedby="feedbackComentarios"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div id="divEstadoActual"
                                             runat="server"
                                             class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Estado actual</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <span id="spnEstadoActual"
                                                      runat="server"
                                                      class="badge bg-info-transparent text-info px-3 py-2 fs-12">
                                                </span>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Estado</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfEstadoActual" runat="server" />
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
                                                <asp:Button ID="btnActualizar" runat="server" CssClass="btn ripple btn-main-primary" Style="min-width: 130px;" Text="Actualizar" OnClientClick="return blockFormulario();" OnClick="btnActualizar_Click" />
                                                <asp:Button ID="btnVolver" runat="server" CssClass="btn btn-outline-secondary ms-2" Style="min-width: 130px;" Text="Volver" CausesValidation="false" UseSubmitBehavior="false" OnClick="btnVolver_Click" />
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