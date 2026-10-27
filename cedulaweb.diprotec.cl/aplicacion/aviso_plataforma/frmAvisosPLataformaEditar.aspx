<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmAvisosPlataformaEditar.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.aviso_plataforma.frmAvisosPlataformaEditar" %>
<%@ Register Src="~/aplicacion/control/ucBitacora.ascx" TagPrefix="uc" TagName="Bitacora" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            inicializarFechasAvisoPlataforma();
            inicializarToggleEstado();

            $("#<%=txtTitulo.ClientID%>").on("blur", function () {
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

            $("#<%=txtMensaje.ClientID%>").on("blur", function () {
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

            $("#<%=txtVersion.ClientID%>").on("blur", function () {
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

            $("#<%=txtFechaInicio.ClientID%>").on("blur", function () {
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

            $("#<%=txtFechaTermino.ClientID%>").on("blur", function () {
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

            $("#<%=txtTitulo.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoTituloAvisoPlataforma)%>");
            $("#<%=txtMensaje.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoMensajeAvisoPlataforma)%>");
            $("#<%=txtVersion.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoVersionAvisoPlataforma)%>");
        });

        function blockFormulario() {
            var formulario = document.getElementById("<%=Page.Form.ClientID%>");

            if (!formulario.checkValidity()) {
                formulario.reportValidity();
                return false;
            }

            sincronizarHiddenFechas();

            $("#<%=btnActualizar.ClientID%>").val("Actualizando...");
            $("#<%=btnVolver.ClientID%>").prop("disabled", true);

            setTimeout(function () {
                $("#<%=btnActualizar.ClientID%>").prop("disabled", true);
            }, 0);

            return true;
        }

        function inicializarToggleEstado() {
            var toggle = $("#tglEstado");
            var hidden = $("#<%=hfEstado.ClientID%>");

            if (hidden.val() === "0") {
                toggle.addClass("on");
            }
            else {
                toggle.removeClass("on");
            }

            toggle.off("click").on("click", function () {
                $(this).toggleClass("on");

                if ($(this).hasClass("on")) {
                    hidden.val("0");
                }
                else {
                    hidden.val("1");
                }
            });
        }

        function obtenerFechaActual() {
            var fecha = new Date();

            fecha.setHours(0, 0, 0, 0);

            return fecha;
        }

        function formatearFecha(fecha) {
            var dia = fecha.getDate().toString().padStart(2, "0");
            var mes = (fecha.getMonth() + 1).toString().padStart(2, "0");
            var anio = fecha.getFullYear();

            return dia + "-" + mes + "-" + anio;
        }

        function obtenerFechaDesdeTexto(valor) {
            if (valor === null || valor.trim() === "") {
                return null;
            }

            var partes = valor.split("-");

            if (partes.length !== 3) {
                return null;
            }

            var dia = parseInt(partes[0], 10);
            var mes = parseInt(partes[1], 10) - 1;
            var anio = parseInt(partes[2], 10);

            var fecha = new Date(anio, mes, dia);

            fecha.setHours(0, 0, 0, 0);

            return fecha;
        }

        function sincronizarHiddenFechas() {
            $("#<%=hdnFechaInicio.ClientID%>").val($("#<%=txtFechaInicio.ClientID%>").val());
            $("#<%=hdnFechaTermino.ClientID%>").val($("#<%=txtFechaTermino.ClientID%>").val());
        }

        function inicializarValorFechas() {
            var fechaInicio = $("#<%=txtFechaInicio.ClientID%>").val();
            var fechaTermino = $("#<%=txtFechaTermino.ClientID%>").val();

            var hiddenInicio = $("#<%=hdnFechaInicio.ClientID%>").val();
            var hiddenTermino = $("#<%=hdnFechaTermino.ClientID%>").val();

            if (fechaInicio === "" && hiddenInicio !== "") {
                fechaInicio = hiddenInicio;
                $("#<%=txtFechaInicio.ClientID%>").val(fechaInicio);
            }

            if (fechaTermino === "" && hiddenTermino !== "") {
                fechaTermino = hiddenTermino;
                $("#<%=txtFechaTermino.ClientID%>").val(fechaTermino);
            }

            $("#<%=hdnFechaInicio.ClientID%>").val(fechaInicio);
            $("#<%=hdnFechaTermino.ClientID%>").val(fechaTermino);
        }

        function inicializarFechasAvisoPlataforma() {
            inicializarValorFechas();

            if (typeof $.fn.datepicker !== "function") {
                console.error("No se encontró datepicker.");
                sincronizarHiddenFechas();
                return;
            }

            try {
                $("#<%=txtFechaInicio.ClientID%>").datepicker("destroy");
            }
            catch (e) {
            }

            try {
                $("#<%=txtFechaTermino.ClientID%>").datepicker("destroy");
            }
            catch (e) {
            }

            $("#<%=txtFechaInicio.ClientID%>").datepicker({
                dateFormat: "dd-mm-yy",
                maxDate: obtenerFechaDesdeTexto($("#<%=txtFechaTermino.ClientID%>").val()),
                firstDay: 1,
                dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
                onSelect: function (selectedDate) {
                    $("#<%=txtFechaInicio.ClientID%>").val(selectedDate);
                    $("#<%=txtFechaTermino.ClientID%>").datepicker("option", "minDate", obtenerFechaDesdeTexto(selectedDate));

                    var fechaInicio = obtenerFechaDesdeTexto(selectedDate);
                    var fechaTermino = obtenerFechaDesdeTexto($("#<%=txtFechaTermino.ClientID%>").val());

                    if (fechaInicio !== null && fechaTermino !== null && fechaInicio > fechaTermino) {
                        $("#<%=txtFechaTermino.ClientID%>").val(selectedDate);
                    }

                    sincronizarHiddenFechas();
                }
            });

            $("#<%=txtFechaTermino.ClientID%>").datepicker({
                dateFormat: "dd-mm-yy",
                minDate: obtenerFechaDesdeTexto($("#<%=txtFechaInicio.ClientID%>").val()),
                firstDay: 1,
                dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
                onSelect: function (selectedDate) {
                    $("#<%=txtFechaTermino.ClientID%>").val(selectedDate);
                    $("#<%=txtFechaInicio.ClientID%>").datepicker("option", "maxDate", obtenerFechaDesdeTexto(selectedDate));

                    var fechaInicio = obtenerFechaDesdeTexto($("#<%=txtFechaInicio.ClientID%>").val());
                    var fechaTermino = obtenerFechaDesdeTexto(selectedDate);

                    if (fechaInicio !== null && fechaTermino !== null && fechaTermino < fechaInicio) {
                        $("#<%=txtFechaInicio.ClientID%>").val(selectedDate);
                    }

                    sincronizarHiddenFechas();
                }
            });

            $("#<%=txtFechaTermino.ClientID%>").datepicker("option", "minDate", obtenerFechaDesdeTexto($("#<%=txtFechaInicio.ClientID%>").val()));
            $("#<%=txtFechaInicio.ClientID%>").datepicker("option", "maxDate", obtenerFechaDesdeTexto($("#<%=txtFechaTermino.ClientID%>").val()));

            sincronizarHiddenFechas();
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
                                                <label class="mg-b-0">Título</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtTitulo"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="100"
                                                    placeholder="título del aviso"
                                                    required="required"
                                                    aria-describedby="feedbackTitulo">
                                                </asp:TextBox>
                                                <div id="feedbackTitulo" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionAvisoPlataformaTituloVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Mensaje</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtMensaje"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="500"
                                                    TextMode="MultiLine"
                                                    Rows="4"
                                                    placeholder="mensaje visible para los usuarios"
                                                    required="required"
                                                    aria-describedby="feedbackMensaje">
                                                </asp:TextBox>
                                                <div id="feedbackMensaje" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionAvisoPlataformaMensajeVacio%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Versión</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:TextBox
                                                    ID="txtVersion"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    MaxLength="20"
                                                    placeholder="1.3.0"
                                                    required="required"
                                                    aria-describedby="feedbackVersion">
                                                </asp:TextBox>
                                                <div id="feedbackVersion" class="invalid-feedback">
                                                    <%=ConstantesModelo.ConsValidacionAvisoPlataformaVersionVacia%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Fecha inicio</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <div style="width: 136px;">
                                                    <asp:TextBox
                                                        ID="txtFechaInicio"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        MaxLength="10"
                                                        autocomplete="off"
                                                        onkeydown="return false;"
                                                        onpaste="return false;"
                                                        Style="cursor: pointer;"
                                                        required="required"
                                                        aria-describedby="feedbackFechaInicio">
                                                    </asp:TextBox>
                                                    <div id="feedbackFechaInicio" class="invalid-feedback">
                                                        <%=ConstantesModelo.ConsValidacionAvisoPlataformaFechaInicioInvalida%>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Fecha término</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <div style="width: 136px;">
                                                    <asp:TextBox
                                                        ID="txtFechaTermino"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        MaxLength="10"
                                                        autocomplete="off"
                                                        onkeydown="return false;"
                                                        onpaste="return false;"
                                                        Style="cursor: pointer;"
                                                        required="required"
                                                        aria-describedby="feedbackFechaTermino">
                                                    </asp:TextBox>
                                                    <div id="feedbackFechaTermino" class="invalid-feedback">
                                                        <%=ConstantesModelo.ConsValidacionAvisoPlataformaFechaTerminoInvalida%>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row row-xs align-items-center mg-b-20">
                                            <div class="col-md-4">
                                                <label class="mg-b-0">Estado</label>
                                            </div>
                                            <div class="col-md-8 mg-t-5 mg-md-t-0">
                                                <asp:HiddenField ID="hfEstado" runat="server" Value="0" />

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

    <asp:HiddenField ID="hdnId" runat="server" />
    <asp:HiddenField ID="hdnFechaInicio" runat="server" />
    <asp:HiddenField ID="hdnFechaTermino" runat="server" />

</asp:Content>