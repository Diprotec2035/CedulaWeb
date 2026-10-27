<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmReglasNueva.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.regla.frmReglasNueva" %>
<%@ Import Namespace="Modelo" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>

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

            $("#<%=txtNombre.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombreRegla)%>");
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
                                <a href='<%=ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%>'><%:ModuloNavegacionHelper.ObtenerTituloAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)%></a>
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
                                                <label class="mg-b-0">Nombre</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtNombre"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="nombre regla"
                                                    required="required"
                                                    aria-describedby="feedbackNombre">
                                                </asp:TextBox>
                                                <div id="feedbackNombre" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionReglaNombreVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Pide nombre</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfNombreApellido" runat="server" />

                                                <div id="tglNombreApellido"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Pide empresa</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfEmpresa" runat="server" />

                                                <div id="tglEmpresa"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Pide patente</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfPatente" runat="server" />

                                                <div id="tglPatente"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Pide comentario</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfComentario" runat="server" />

                                                <div id="tglComentario"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Pide fotografía</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfFotografia" runat="server" />

                                                <div id="tglFotografia"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Indica entrada / salida</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfEntradaSalida" runat="server" />

                                                <div id="tglEntradaSalida"
                                                     runat="server"
                                                     class="main-toggle br-3 text-center">
                                                    <span></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Lista blanca / negra</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:DropDownList
                                                    ID="ddlListaBlancaNegra"
                                                    runat="server"
                                                    CssClass="form-control">
                                                    <asp:ListItem Text="Ninguna" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Lista blanca" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Lista negra" Value="2"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Permite eliminar enviados</label>
                                            </div>

                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfEliminaEnviados" runat="server" />

                                                <div id="tglEliminaEnviados"
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
