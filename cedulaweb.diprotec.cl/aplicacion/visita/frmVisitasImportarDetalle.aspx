<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmVisitasImportarDetalle.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.visita.frmVisitasImportarDetalle" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%=txtCoincidencia.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaDetalleCargaVisita)%>");
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

                <!-- Resumen carga -->
                <div class="row">
                    <div class="col-lg-12 col-md-12">
                        <div class="card custom-card">
                            <div class="card-body py-2">
                                <div class="d-flex flex-wrap align-items-center">

                                    <span class="me-4">
                                        <strong>ID carga:</strong>
                                        <asp:Literal ID="litIdCarga" runat="server"></asp:Literal>
                                    </span>

                                    <span class="me-4">
                                        <strong>Fecha:</strong>
                                        <asp:Literal ID="litFecha" runat="server"></asp:Literal>
                                    </span>

                                    <span class="me-4">
                                        <strong>Hora:</strong>
                                        <asp:Literal ID="litHora" runat="server"></asp:Literal>
                                    </span>

                                    <span class="me-4">
                                        <strong>Responsable:</strong>
                                        <asp:Literal ID="litResponsable" runat="server"></asp:Literal>
                                    </span>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Resumen carga -->

                <!-- Filtros -->
                <asp:Panel ID="pnlFiltros" runat="server" DefaultButton="btnBuscar">
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <div class="card custom-card">
                                <div class="card-body py-3">

                                    <div class="row row-sm align-items-end">

                                        <div class="col-xl-5 col-lg-4 col-md-12 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Coincidencia</label>
                                            <asp:TextBox
                                                ID="txtCoincidencia"
                                                runat="server"
                                                CssClass="form-control"
                                                MaxLength="100"
                                                placeholder="rut, nombre, empresa o mensaje">
                                            </asp:TextBox>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Resultado</label>
                                            <asp:DropDownList
                                                ID="ddlResultado"
                                                runat="server"
                                                CssClass="form-control">
                                                <asp:ListItem Text="Todos" Value="9" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Insertada" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Actualizada" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Sin cambios" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Error" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 d-none d-xl-block mb-2"></div>

                                        <div class="col-xl-3 col-lg-4 col-md-12 col-sm-12 mb-2">
                                            <div class="row row-sm">
                                                <div class="col-6">
                                                    <asp:Button
                                                        ID="btnBuscar"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-block"
                                                        Text="Buscar"
                                                        OnClick="btnBuscar_Click" />
                                                </div>

                                                <div class="col-6">
                                                    <asp:Button
                                                        ID="btnVolver"
                                                        runat="server"
                                                        CssClass="btn btn-outline-secondary btn-block"
                                                        Text="Volver"
                                                        CausesValidation="false"
                                                        OnClick="btnVolver_Click" />
                                                </div>
                                            </div>
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
                                <h6 class="card-title mb-0"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>
                            </div>

                            <div class="card-body">
                                <div class="table-responsive p-0">

                                    <asp:GridView
                                        ID="gvListado"
                                        runat="server"
                                        AutoGenerateColumns="False"
                                        AllowPaging="True"
                                        PageSize="7"
                                        OnRowCreated="gvListado_RowCreated"
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
                                            <asp:BoundField HeaderText="Fila" DataField="Fila">
                                                <HeaderStyle CssClass="text-center" Width="7%" />
                                                <ItemStyle CssClass="text-center" Width="7%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="RUT" DataField="Rut">
                                                <HeaderStyle Width="11%" />
                                                <ItemStyle Width="11%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Nombre" DataField="Nombre">
                                                <HeaderStyle Width="22%" />
                                                <ItemStyle Width="22%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Empresa" DataField="Empresa">
                                                <HeaderStyle Width="15%" />
                                                <ItemStyle Width="15%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Resultado">
                                                <HeaderStyle CssClass="text-center" Width="12%" />
                                                <ItemStyle CssClass="text-center" Width="12%" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Estado")) != 0
                                                        ? "badge bg-danger-transparent text-danger px-3 py-2 fs-12"
                                                        : Convert.ToInt16(Eval("Accion")) == 1
                                                            ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                            : Convert.ToInt16(Eval("Accion")) == 2
                                                                ? "badge bg-info-transparent text-info px-3 py-2 fs-12"
                                                                : "badge bg-secondary-transparent text-secondary px-3 py-2 fs-12" %>'>

                                                        <i class='<%# Convert.ToInt16(Eval("Estado")) != 0
                                                            ? "fe fe-x-circle me-1"
                                                            : Convert.ToInt16(Eval("Accion")) == 1
                                                                ? "fe fe-plus-circle me-1"
                                                                : Convert.ToInt16(Eval("Accion")) == 2
                                                                    ? "fe fe-edit me-1"
                                                                    : "fe fe-minus-circle me-1" %>'></i>

                                                        <%# Convert.ToInt16(Eval("Estado")) != 0 ? Eval("EstadoNombre") : Eval("AccionNombre") %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField HeaderText="Mensaje" DataField="Mensaje">
                                                <HeaderStyle Width="33%" />
                                                <ItemStyle Width="33%" />
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

</asp:Content>