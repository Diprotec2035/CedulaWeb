<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmModulosNuevo.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.modulo.frmModulosNuevo" %>
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

            $("#<%=txtTituloPagina.ClientID%>").on("blur", function () {
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

            $("#<%=txtTituloContenido.ClientID%>").on("blur", function () {
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

            $("#<%=txtUrl.ClientID%>").on("blur", function () {
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

            $("#<%=txtUrlPublica.ClientID%>").on("blur", function () {
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

            $("#<%=ddlBitacoraSp.ClientID%>").on("blur", function () {
                var campo = $(this);
                var valor = campo.val().trim();

                campo.removeClass("is-valid is-invalid");

                if (valor.length > 0) {
                    campo.addClass("is-valid");
                }
            });

            $("#<%=ddlBitacoraParametro.ClientID%>").on("blur", function () {
                var campo = $(this);
                var valor = campo.val().trim();

                campo.removeClass("is-valid is-invalid");

                if (valor.length > 0) {
                    campo.addClass("is-valid");
                }
            });

            $("#<%=ddlCategoria.ClientID%>").on("change", function () {
                var campo = $(this);
                var valor = campo.val();

                campo.removeClass("is-valid is-invalid");

                if (valor !== "" && valor !== "0") {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            $("#<%=ddlOrden.ClientID%>").on("change", function () {
                var campo = $(this);
                var valor = campo.val();

                campo.removeClass("is-valid is-invalid");

                if (valor !== "" && valor !== "0") {
                    campo.addClass("is-valid");
                }
                else {
                    campo.addClass("is-invalid");
                }
            });

            inicializarBitacoraModulo();

            $("#<%=txtNombre.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombreModulo)%>");
            $("#<%=txtTituloPagina.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTituloModulo)%>");
            $("#<%=txtTituloContenido.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTituloModulo)%>");
            $("#<%=txtUrl.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoUrlModulo)%>");
            $("#<%=txtUrlPublica.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoUrlModulo)%>");
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

        function inicializarBitacoraModulo() {
            var toggle = $("#<%=tglBitacoraEstado.ClientID%>");
            var hidden = $("#<%=hfBitacoraEstado.ClientID%>");
            var detalle = $("#<%=divBitacoraDetalle.ClientID%>");

            if (hidden.val() === "0") {
                detalle.show();
                toggle.addClass("on");
            }
            else {
                detalle.hide();
                toggle.removeClass("on");
            }

            toggle.off("click.bitacora").on("click.bitacora", function () {
                setTimeout(function () {
                    if (toggle.hasClass("on")) {
                        hidden.val("0");
                        detalle.slideDown(150);
                    }
                    else {
                        hidden.val("1");
                        detalle.slideUp(150);
                    }
                }, 0);
            });
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
                                <a href="<%=cedulaweb.diprotec.cl.aplicacion.helpers.ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>">
                                    <%:cedulaweb.diprotec.cl.aplicacion.helpers.ModuloNavegacionHelper.ObtenerTituloAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>
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
                                                <label class="mg-b-0">Categoría</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlCategoria"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    required="required"
                                                    aria-describedby="feedbackCategoria">
                                                </asp:DropDownList>
                                                <div id="feedbackCategoria" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionModuloCategoriaVacia%>
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
                                                    MaxLength="50"
                                                    placeholder="nombre módulo"
                                                    required="required"
                                                    aria-describedby="feedbackNombre">
                                                </asp:TextBox>
                                                <div id="feedbackNombre" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionModuloNombreVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Título de página</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtTituloPagina"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="150"
                                                    placeholder="título mostrado en la página"
                                                    required="required"
                                                    aria-describedby="feedbackTituloPagina">
                                                </asp:TextBox>
                                                <div id="feedbackTituloPagina" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionModuloTituloPaginaVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Título de contenido</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtTituloContenido"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="150"
                                                    placeholder="título mostrado en el contenido"
                                                    required="required"
                                                    aria-describedby="feedbackTituloContenido">
                                                </asp:TextBox>
                                                <div id="feedbackTituloContenido" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionModuloTituloContenidoVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Orden</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlOrden"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    Style="width: 80px;"
                                                    required="required"
                                                    aria-describedby="feedbackOrden">
                                                </asp:DropDownList>
                                                <div id="feedbackOrden" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionModuloOrdenVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">URL real</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtUrl"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="/aplicacion/modulo/formulario.aspx"
                                                    required="required"
                                                    aria-describedby="feedbackUrl">
                                                </asp:TextBox>
                                                <div id="feedbackUrl" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionModuloUrlVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">URL pública</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtUrlPublica"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="mod-url-publica"
                                                    required="required"
                                                    aria-describedby="feedbackUrlPublica">
                                                </asp:TextBox>
                                                <div id="feedbackUrlPublica" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionModuloUrlPublicaVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Administrador</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfAdministrador" runat="server" Value="1" />

                                                <div id="tglAdministrador"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Visible menú</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfMenuVisible" runat="server" Value="0" />

                                                <div id="tglMenuVisible"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center on">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Bitácora</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfBitacoraEstado" runat="server" Value="1" />

                                                <div id="tglBitacoraEstado"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="divBitacoraDetalle" runat="server">
                                            <div class="row row-xs align-items-center mg-b-20">
                                                <div class="col-md-4">
                                                    <label class="mg-b-0">Tabla</label>
                                                </div>
                                                <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                    <asp:DropDownList
                                                        ID="ddlBitacoraTabla"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlBitacoraTabla_SelectedIndexChanged"
                                                        aria-describedby="feedbackBitacoraTabla">
                                                    </asp:DropDownList>
                                                    <div id="feedbackBitacoraTabla" class="invalid-feedback">
                                                        <%=ConstantesModelo.ConsValidacionBaseDatosTablaVacia%>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row row-xs align-items-center mg-b-20">
                                                <div class="col-md-4">
                                                    <label class="mg-b-0">Campo identificador</label>
                                                </div>
                                                <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                    <asp:DropDownList
                                                        ID="ddlBitacoraParametro"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        aria-describedby="feedbackBitacoraParametro">
                                                    </asp:DropDownList>
                                                    <div id="feedbackBitacoraParametro" class="invalid-feedback">
                                                        <%=ConstantesModelo.ConsValidacionModuloBitacoraParametroVacio%>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row row-xs align-items-center mg-b-20">
                                                <div class="col-md-4">
                                                    <label class="mg-b-0">SP asociado</label>
                                                </div>
                                                <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                    <asp:DropDownList
                                                        ID="ddlBitacoraSp"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        aria-describedby="feedbackBitacoraSp">
                                                    </asp:DropDownList>
                                                    <div id="feedbackBitacoraSp" class="invalid-feedback">
                                                        <%=ConstantesModelo.ConsValidacionModuloBitacoraSpVacio%>
                                                    </div>
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