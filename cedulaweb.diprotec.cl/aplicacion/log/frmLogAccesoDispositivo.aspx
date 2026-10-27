<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmLogAccesoDispositivo.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.log.frmLogAccesoDispositivo" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        $(document).ready(function () {
            inicializarFiltroFechas();

            $("#<%=txtCoincidencia.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaLogAcceso)%>");
        });

        function obtenerFechaActual() {
            var fecha = new Date();

            fecha.setHours(0, 0, 0, 0);

            return fecha;
        }

        function obtenerFechaDesdeDefecto() {
            var fecha = obtenerFechaActual();

            fecha.setFullYear(fecha.getFullYear() - 1);

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
            $("#<%=hdnFechaDesde.ClientID%>").val($("#<%=txtFechaDesde.ClientID%>").val());
            $("#<%=hdnFechaHasta.ClientID%>").val($("#<%=txtFechaHasta.ClientID%>").val());
        }

        function inicializarValorFechas() {
            var fechaDesde = $("#<%=txtFechaDesde.ClientID%>").val();
            var fechaHasta = $("#<%=txtFechaHasta.ClientID%>").val();

            var hiddenDesde = $("#<%=hdnFechaDesde.ClientID%>").val();
            var hiddenHasta = $("#<%=hdnFechaHasta.ClientID%>").val();

            if (fechaDesde === "" && hiddenDesde !== "") {
                fechaDesde = hiddenDesde;
                $("#<%=txtFechaDesde.ClientID%>").val(fechaDesde);
            }

            if (fechaHasta === "" && hiddenHasta !== "") {
                fechaHasta = hiddenHasta;
                $("#<%=txtFechaHasta.ClientID%>").val(fechaHasta);
            }

            if (fechaDesde === "") {
                fechaDesde = formatearFecha(obtenerFechaDesdeDefecto());
                $("#<%=txtFechaDesde.ClientID%>").val(fechaDesde);
            }

            if (fechaHasta === "") {
                fechaHasta = formatearFecha(obtenerFechaActual());
                $("#<%=txtFechaHasta.ClientID%>").val(fechaHasta);
            }

            $("#<%=hdnFechaDesde.ClientID%>").val(fechaDesde);
            $("#<%=hdnFechaHasta.ClientID%>").val(fechaHasta);
        }

        function inicializarFiltroFechas() {
            inicializarValorFechas();

            if (typeof $.fn.datepicker !== "function") {
                console.error("No se encontró datepicker.");
                sincronizarHiddenFechas();
                return;
            }

            try {
                $("#<%=txtFechaDesde.ClientID%>").datepicker("destroy");
            }
            catch (e) {
            }

            try {
                $("#<%=txtFechaHasta.ClientID%>").datepicker("destroy");
            }
            catch (e) {
            }

            $("#<%=txtFechaDesde.ClientID%>").datepicker({
                dateFormat: "dd-mm-yy",
                maxDate: obtenerFechaDesdeTexto($("#<%=txtFechaHasta.ClientID%>").val()),
                firstDay: 1,
                dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
                onSelect: function (selectedDate) {
                    $("#<%=txtFechaDesde.ClientID%>").val(selectedDate);
                    $("#<%=txtFechaHasta.ClientID%>").datepicker("option", "minDate", obtenerFechaDesdeTexto(selectedDate));

                    var fechaDesde = obtenerFechaDesdeTexto(selectedDate);
                    var fechaHasta = obtenerFechaDesdeTexto($("#<%=txtFechaHasta.ClientID%>").val());

                    if (fechaDesde !== null && fechaHasta !== null && fechaDesde > fechaHasta) {
                        $("#<%=txtFechaHasta.ClientID%>").val(selectedDate);
                    }

                    sincronizarHiddenFechas();
                }
            });

            $("#<%=txtFechaHasta.ClientID%>").datepicker({
                dateFormat: "dd-mm-yy",
                minDate: obtenerFechaDesdeTexto($("#<%=txtFechaDesde.ClientID%>").val()),
                maxDate: obtenerFechaActual(),
                firstDay: 1,
                dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
                onSelect: function (selectedDate) {
                    $("#<%=txtFechaHasta.ClientID%>").val(selectedDate);
                    $("#<%=txtFechaDesde.ClientID%>").datepicker("option", "maxDate", obtenerFechaDesdeTexto(selectedDate));

                    var fechaDesde = obtenerFechaDesdeTexto($("#<%=txtFechaDesde.ClientID%>").val());
                    var fechaHasta = obtenerFechaDesdeTexto(selectedDate);

                    if (fechaDesde !== null && fechaHasta !== null && fechaHasta < fechaDesde) {
                        $("#<%=txtFechaDesde.ClientID%>").val(selectedDate);
                    }

                    sincronizarHiddenFechas();
                }
            });

            $("#<%=txtFechaHasta.ClientID%>").datepicker("option", "minDate", obtenerFechaDesdeTexto($("#<%=txtFechaDesde.ClientID%>").val()));
            $("#<%=txtFechaDesde.ClientID%>").datepicker("option", "maxDate", obtenerFechaDesdeTexto($("#<%=txtFechaHasta.ClientID%>").val()));

            sincronizarHiddenFechas();
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
                            <li class="breadcrumb-item active text-primary" aria-current="page">
                                <%:ModuloContextoHelper.ObtenerTituloPagina(this)%>
                            </li>
                        </ol>
                    </div>
                </div>
                <!-- End Page Header -->

                <!-- Filtros -->
                <asp:Panel ID="Panel" runat="server" DefaultButton="btnBuscar">
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <div class="card custom-card">
                                <div class="card-body py-3">
                                    <div class="row row-sm align-items-end">

                                        <div class="col-xl-4 col-lg-4 col-md-12 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Coincidencia</label>

                                            <asp:TextBox
                                                ID="txtCoincidencia"
                                                runat="server"
                                                CssClass="form-control"
                                                MaxLength="100"
                                                placeholder="b&uacute;squeda de accesos">
                                            </asp:TextBox>
                                        </div>

                                        <div class="col-xl-1 col-lg-1 col-md-3 col-sm-6 mb-2">
                                            <label class="form-label mg-b-5">Desde</label>

                                            <asp:TextBox
                                                ID="txtFechaDesde"
                                                runat="server"
                                                CssClass="form-control"
                                                MaxLength="10"
                                                autocomplete="off"
                                                onkeydown="return false;"
                                                onpaste="return false;"
                                                Style="cursor: pointer;">
                                            </asp:TextBox>
                                        </div>

                                        <div class="col-xl-1 col-lg-1 col-md-3 col-sm-6 mb-2">
                                            <label class="form-label mg-b-5">Hasta</label>

                                            <asp:TextBox
                                                ID="txtFechaHasta"
                                                runat="server"
                                                CssClass="form-control"
                                                MaxLength="10"
                                                autocomplete="off"
                                                onkeydown="return false;"
                                                onpaste="return false;"
                                                Style="cursor: pointer;">
                                            </asp:TextBox>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-3 col-sm-6 mb-2">
                                            <label class="form-label mg-b-5">Ordenar por</label>

                                            <asp:DropDownList
                                                ID="ddlOrdenarPor"
                                                runat="server"
                                                CssClass="form-control">
                                                <asp:ListItem Value="1">Marca</asp:ListItem>
                                                <asp:ListItem Value="2">Modelo</asp:ListItem>
                                                <asp:ListItem Value="3">Serie</asp:ListItem>
                                                <asp:ListItem Value="4" Selected="True">Fecha</asp:ListItem>
                                                <asp:ListItem Value="5">IP</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-3 col-sm-6 mb-2">
                                            <label class="form-label mg-b-5">Dirección</label>

                                            <asp:DropDownList
                                                ID="ddlDireccion"
                                                runat="server"
                                                CssClass="form-control">
                                                <asp:ListItem Value="1">Ascendente</asp:ListItem>
                                                <asp:ListItem Value="2" Selected="True">Descendente</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-12 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5 d-block">&nbsp;</label>

                                            <asp:Button
                                                ID="btnBuscar"
                                                runat="server"
                                                CssClass="btn ripple btn-outline-info btn-block w-100"
                                                Text="Buscar"
                                                OnClick="btnBuscar_Click" />
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <!-- End Filtros -->

                <!-- Tabla -->
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card custom-card">
                            <div class="card-header">
                                <h6 class="card-title"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>
                            </div>

                            <div class="card-body">
                                <div class="table-responsive p-0">

                                    <asp:GridView
                                        ID="gvListado"
                                        runat="server"
                                        AutoGenerateColumns="False"
                                        DataKeyNames="Id"
                                        OnRowDataBound="gvListado_RowDataBound"
                                        OnRowCreated="gvListado_RowCreated"
                                        AllowPaging="True"
                                        PageSize="8"
                                        OnPageIndexChanging="gvListado_PageIndexChanging"
                                        CssClass="table table-hover text-start mg-b-0 border"
                                        Style="table-layout: fixed; width: 100%;"
                                        GridLines="None"
                                        PagerSettings-Mode="NumericFirstLast"
                                        PagerSettings-FirstPageText="Primera"
                                        PagerSettings-LastPageText="Última"
                                        PagerStyle-HorizontalAlign="Center">

                                        <EmptyDataRowStyle CssClass="text-center tx-14 text-muted" />

                                        <Columns>
                                            <asp:BoundField HeaderText="Marca" DataField="Marca">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle Width="20%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Modelo" DataField="Modelo">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle Width="20%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Serie Number" DataField="Serialnumber">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle Width="20%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Fecha" DataField="Fecha">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Hora" DataField="Hora">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="IP" DataField="IP">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>
                                        </Columns>

                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Tabla -->

            </div>
        </div>
    </div>
    <!-- End Main Content-->

    <asp:HiddenField ID="hdnFechaDesde" runat="server" />
    <asp:HiddenField ID="hdnFechaHasta" runat="server" />

</asp:Content>