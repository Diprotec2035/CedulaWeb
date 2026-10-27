<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmRegistroVisitas.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.visita.frmRegistroVisitas" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

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

        function verImagen(base64) {
            $("#imgEvidencia").attr("src", base64);
            $("#modalEvidencia").modal("show");

            return false;
        }

        $(document).ready(function () {
            inicializarFiltroFechas();
            $("#<%=txtCoincidencia.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaRegistroVisita)%>");
        });

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
                <asp:Panel ID="pnlFiltros" runat="server" DefaultButton="btnBuscar">
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <div class="card custom-card">
                                <div class="card-body py-3">
                                    <div class="row row-sm align-items-end">

                                        <div class="col-xl-3 col-lg-4 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Coincidencia</label>
                                            <asp:TextBox
                                                ID="txtCoincidencia"
                                                runat="server"
                                                CssClass="form-control"
                                                MaxLength="100"
                                                placeholder="rut, nombre o empresa"
                                                data-filtro-sesion="Coincidencia">
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
                                                Style="cursor: pointer;"
                                                data-filtro-sesion="FechaDesde">
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
                                                Style="cursor: pointer;"
                                                data-filtro-sesion="FechaHasta">
                                            </asp:TextBox>
                                        </div>

                                        <div class="col-xl col-lg-3 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Operación</label>
                                            <asp:DropDownList
                                                ID="ddlOperacion"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="Operacion">
                                                <asp:ListItem Text="Todos" Value="9" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Entrada" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Salida" Value="1"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl col-lg-3 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Ordenar por</label>
                                            <asp:DropDownList
                                                ID="ddlOrdenarPor"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="OrdenarPor">
                                                <asp:ListItem Text="RUT" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Nombre" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Empresa" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Fecha" Value="4" Selected="True"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl col-lg-3 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Dirección</label>
                                            <asp:DropDownList
                                                ID="ddlDireccion"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="Direccion">
                                                <asp:ListItem Text="Descendente" Value="2" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Ascendente" Value="1"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl col-lg-3 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5 d-block">&nbsp;</label>
                                            <asp:Button
                                                ID="btnBuscar"
                                                runat="server"
                                                CssClass="btn ripple btn-outline-info btn-block w-100"
                                                Text="Buscar"
                                                OnClick="btnBuscar_Click" />
                                        </div>

                                        <div class="col-xl col-lg-3 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5 d-block">&nbsp;</label>
                                            <asp:LinkButton
                                                ID="btnExcel"
                                                runat="server"
                                                CssClass="btn ripple btn-outline-success btn-block w-100"
                                                OnClick="btnExcel_Click"
                                                ToolTip="Descargar Excel">
                                                <i class="fe fe-download me-1"></i>Excel
                                            </asp:LinkButton>
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
                                            <asp:BoundField HeaderText="RUT" DataField="Rut">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Nombre" DataField="NombreApellido">
                                                <HeaderStyle Width="23%" />
                                                <ItemStyle Width="23%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Empresa" DataField="Empresa">
                                                <HeaderStyle Width="19%" />
                                                <ItemStyle Width="19%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Patente" DataField="Patente">
                                                <HeaderStyle Width="9%" />
                                                <ItemStyle Width="9%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Operación">
                                                <HeaderStyle CssClass="text-center" Width="11%" />
                                                <ItemStyle CssClass="text-center" Width="11%" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("EntradaSalida")) == 0
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : "badge bg-info-transparent text-info px-3 py-2 fs-12" %>'>

                                                        <i class='<%# Convert.ToInt16(Eval("EntradaSalida")) == 0
                                                            ? "fe fe-log-in me-1"
                                                            : "fe fe-log-out me-1" %>'></i>

                                                        <%# Convert.ToInt16(Eval("EntradaSalida")) == 0 ? "Entrada" : "Salida" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField HeaderText="Fecha" DataField="Fecha">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Hora" DataField="Hora">
                                                <HeaderStyle Width="8%" />
                                                <ItemStyle Width="8%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Evidencia">
                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />
                                                <ItemTemplate>
                                                    <asp:LinkButton
                                                        ID="lnkEvidencia"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-sm"
                                                        ToolTip="Ver evidencia"
                                                        Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("Fotografia"))) %>'
                                                        OnClientClick='<%# "return verImagen(\"data:image/jpeg;base64," + Convert.ToString(Eval("Fotografia")) + "\");" %>'>
                                                        <i class="fe fe-image"></i>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
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

    <div class="modal fade" id="modalEvidencia" tabindex="-1" aria-labelledby="tituloEvidencia" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h6 class="modal-title" id="tituloEvidencia">Evidencia de visita</h6>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <div class="modal-body text-center">
                    <img id="imgEvidencia" src="" class="img-fluid" alt="Evidencia de visita" style="max-height: 70vh;" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
