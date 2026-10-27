<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmModulosEditar.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.modulo.frmModulosEditar" %>
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

            $("#<%=ddlBitacoraTabla.ClientID%>").on("change", function () {
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

            $("#<%=ddlBitacoraParametro.ClientID%>").on("change", function () {
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

            $("#<%=ddlBitacoraSp.ClientID%>").on("change", function () {
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

            $("#<%=ddlNavegacionAccion.ClientID%>").on("change", function () {
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

            $("#<%=ddlNavegacionDestino.ClientID%>").on("change", function () {
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

            $("#<%=txtNombre.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoNombreModulo)%>");
            $("#<%=txtTituloPagina.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTituloModulo)%>");
            $("#<%=txtTituloContenido.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTituloModulo)%>");
            $("#<%=txtUrl.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoUrlModulo)%>");
            $("#<%=txtUrlPublica.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoUrlModulo)%>");

            inicializarBitacoraModulo();
            inicializarNavegacionModulo();
        });

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

        function inicializarNavegacionModulo() {
            var toggle = $("#<%=tglNavegacionEstado.ClientID%>");
            var hidden = $("#<%=hfNavegacionEstado.ClientID%>");

            if (hidden.val() === "0") {
                toggle.addClass("on");
            }
            else {
                toggle.removeClass("on");
            }

            toggle.off("click.navegacion").on("click.navegacion", function () {
                setTimeout(function () {
                    if (toggle.hasClass("on")) {
                        hidden.val("0");
                    }
                    else {
                        hidden.val("1");
                    }
                }, 0);
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
                            <div class="card-header d-flex align-items-center justify-content-between">
                                <h6 class="card-title mb-0"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>

                                <asp:Button
                                    ID="btnVerBitacora"
                                    runat="server"
                                    CssClass="btn ripple btn-outline-info"
                                    Text="Ver bitácora"
                                    CausesValidation="false"
                                    UseSubmitBehavior="false"
                                    OnClientClick="return abrirBitacora();" />
                            </div>

                            <div class="card-body">
                                <asp:HiddenField ID="hfId" runat="server" />
                                <asp:HiddenField ID="hfAdministrador" runat="server" />
                                <asp:HiddenField ID="hfMenuVisible" runat="server" />
                                <asp:HiddenField ID="hfBitacoraEstado" runat="server" />
                                <asp:HiddenField ID="hfEstado" runat="server" />

                                <div class="row">

                                    <!-- Columna izquierda: edición módulo -->
                                    <div class="col-xl-6 col-lg-6 col-md-12">

                                        <div class="card custom-card border">
                                            <div class="card-header">
                                                <h6 class="card-title mb-0"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>
                                            </div>

                                            <div class="card-body">

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
                                                        <div id="tglMenuVisible"
                                                             runat="server"
                                                             class="main-toggle br-3 text-center">
                                                            <span></span>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">Bitácora</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
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

                                                <div class="row row-xs align-items-center mg-b-20">
                                                    <div class="col-md-4">
                                                        <label class="mg-b-0">Estado</label>
                                                    </div>

                                                    <div class="col-md-8 mg-t-5 mg-md-t-0">
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
                                    <!-- End Columna izquierda -->

                                    <!-- Columna derecha: navegación -->
                                    <div class="col-xl-6 col-lg-6 col-md-12">

                                        <div class="card custom-card border">
                                            <div class="card-header">
                                                <h6 class="card-title mb-0">Navegación del módulo</h6>
                                            </div>

                                            <div class="card-body">
                                                <asp:HiddenField ID="hfNavegacionId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hfNavegacionEstado" runat="server" Value="0" />

                                                <div class="d-flex align-items-end w-100" style="gap: 20px;">

                                                    <div style="width: 110px;">
                                                        <label class="form-label mg-b-5">Acción</label>
                                                        <asp:DropDownList
                                                            ID="ddlNavegacionAccion"
                                                            runat="server"
                                                            CssClass="form-control">
                                                            <asp:ListItem Text="Seleccione" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="NUEVO" Value="NUEVO"></asp:ListItem>
                                                            <asp:ListItem Text="EDITAR" Value="EDITAR"></asp:ListItem>
                                                            <asp:ListItem Text="DETALLE" Value="DETALLE"></asp:ListItem>
                                                            <asp:ListItem Text="VOLVER" Value="VOLVER"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>

                                                    <div style="flex: 1;">
                                                        <label class="form-label mg-b-5">Módulo destino</label>
                                                        <asp:DropDownList
                                                            ID="ddlNavegacionDestino"
                                                            runat="server"
                                                            CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </div>

                                                    <div style="width: 90px;">
                                                        <label class="form-label mg-b-5 d-block">Estado</label>

                                                        <div id="tglNavegacionEstado"
                                                             runat="server"
                                                             class="main-toggle br-3 text-center on">
                                                            <span></span>
                                                        </div>
                                                    </div>

                                                    <div style="width: 130px;">
                                                        <asp:Button
                                                            ID="btnGuardarNavegacion"
                                                            runat="server"
                                                            CssClass="btn ripple btn-main-primary"
                                                            Style="width: 130px;"
                                                            Text="Agregar"
                                                            CausesValidation="false"
                                                            UseSubmitBehavior="false"
                                                            OnClick="btnGuardarNavegacion_Click" />
                                                    </div>

                                                </div>

                                                <div class="table-responsive mt-3">
                                                    <asp:GridView
                                                        ID="gvNavegacion"
                                                        runat="server"
                                                        AutoGenerateColumns="False"
                                                        DataKeyNames="Id"
                                                        CssClass="table table-hover text-start mg-b-0 border"
                                                        GridLines="None"
                                                        OnRowCommand="gvNavegacion_RowCommand"
                                                        EmptyDataText="No existen navegaciones configuradas para este módulo.">

                                                        <EmptyDataRowStyle CssClass="text-center tx-14 text-muted" />

                                                        <Columns>
                                                            <asp:BoundField HeaderText="Acción" DataField="Accion">
                                                                <HeaderStyle Width="20%" />
                                                                <ItemStyle Width="20%" />
                                                            </asp:BoundField>

                                                            <asp:BoundField HeaderText="Destino" DataField="ModuloNombreDestino">
                                                                <HeaderStyle Width="40%" />
                                                                <ItemStyle Width="40%" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Estado">
                                                                <HeaderStyle CssClass="text-center" Width="20%" />
                                                                <ItemStyle CssClass="text-center" Width="20%" />
                                                                <ItemTemplate>
                                                                    <span class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                                        : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>

                                                                        <i class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                                            ? "fe fe-check-circle me-1"
                                                                            : "fe fe-x-circle me-1" %>'></i>

                                                                        <%# Convert.ToInt16(Eval("Estado")) == 0 ? "Vigente" : "Anulado" %>
                                                                    </span>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Acciones">
                                                                <HeaderStyle CssClass="text-center" Width="20%" />
                                                                <ItemStyle CssClass="text-center" Width="20%" />
                                                                <ItemTemplate>
                                                                    <asp:LinkButton
                                                                        ID="lnkEditarNavegacion"
                                                                        runat="server"
                                                                        CssClass="btn ripple btn-outline-info btn-sm"
                                                                        ToolTip="Editar navegación"
                                                                        CommandName="EditarNavegacion"
                                                                        CommandArgument='<%# Eval("Id") %>'
                                                                        CausesValidation="false">
                                                                        <i class="fe fe-edit"></i>
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>

                                            </div>
                                        </div>

                                    </div>
                                    <!-- End Columna derecha -->

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