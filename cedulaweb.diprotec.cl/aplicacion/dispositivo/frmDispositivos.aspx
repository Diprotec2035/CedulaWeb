<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmDispositivos.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.dispositivo.frmDispositivos" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%=txtCoincidencia.ClientID%>").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaDispositivo)%>");
        });

        function regenerarCodigoDesdeAccion(id) {
            document.getElementById('<%=hfRegenerar.ClientID%>').value = id;
            document.getElementById('<%=btnRegenerarCodigoOculto.ClientID%>').click();
        }

        function mostrarModalCodigoActivacion(codigo) {
            $("#lblCodigoActivacionGenerado").text(codigo);

            if (typeof bootstrap === "undefined") {
                setTimeout(function () {
                    mostrarModalCodigoActivacion(codigo);
                }, 100);

                return;
            }

            var modalElemento = document.getElementById("modalCodigoActivacion");
            var modal = new bootstrap.Modal(modalElemento, {
                backdrop: "static",
                keyboard: false
            });

            modal.show();
        }

        function copiarCodigoActivacion() {
            var codigo = $("#lblCodigoActivacionGenerado").text().trim();

            if (codigo.length === 0) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorWarning%>", "", "No hay código de activación para copiar.");
                return;
            }

            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(codigo).then(function () {
                    mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "", "Código de activación copiado al portapapeles.");
                }).catch(function () {
                    copiarCodigoActivacionFallback(codigo);
                });

                return;
            }

            copiarCodigoActivacionFallback(codigo);
        }

        function copiarCodigoActivacionFallback(codigo) {
            var inputTemporal = document.createElement("input");
            inputTemporal.value = codigo;
            inputTemporal.setAttribute("readonly", "");
            inputTemporal.style.position = "absolute";
            inputTemporal.style.left = "-9999px";

            document.body.appendChild(inputTemporal);
            inputTemporal.select();
            inputTemporal.setSelectionRange(0, 99999);

            try {
                document.execCommand("copy");
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorSuccess%>", "Copiado:", "Código de activación copiado al portapapeles.");
            }
            catch (e) {
                mostrarAlerta("<%=ConstantesModelo.ConsAlertaColorDanger%>", "Error:", "No fue posible copiar el código de activación.");
            }

            document.body.removeChild(inputTemporal);
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
                                                MaxLength="50"
                                                placeholder="buscar dispositivo"
                                                data-filtro-sesion="Coincidencia">
                                            </asp:TextBox>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Estado</label>
                                            <asp:DropDownList
                                                ID="ddlEstado"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="Estado">
                                                <asp:ListItem Text="Todos" Value="-1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Vigente" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Anulado" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Activado" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Ordenar por</label>
                                            <asp:DropDownList
                                                ID="ddlOrdenarPor"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="OrdenarPor">
                                                <asp:ListItem Text="Marca" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Modelo" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Part Number" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Serial Number" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Estado" Value="6"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-xl-2 col-lg-2 col-md-6 col-sm-12 mb-2">
                                            <label class="form-label mg-b-5">Dirección</label>
                                            <asp:DropDownList
                                                ID="ddlDireccion"
                                                runat="server"
                                                CssClass="form-control"
                                                data-filtro-sesion="Direccion">
                                                <asp:ListItem Text="Ascendente" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Descendente" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

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
                                                        ID="btnNuevo"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-block"
                                                        Text="Nuevo"
                                                        OnClick="btnNuevo_Click" />
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
                                <h6 class="card-title"><%:ModuloContextoHelper.ObtenerTituloContenido(this)%></h6>
                            </div>

                            <div class="card-body">
                                <div class="table-responsive p-0">

                                    <asp:HiddenField ID="hfRegenerar" runat="server" />

                                    <asp:Button
                                        ID="btnRegenerarCodigoOculto"
                                        runat="server"
                                        Text="Regenerar"
                                        Style="display:none;"
                                        OnClick="btnRegenerarCodigoOculto_Click" />

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

                                            <asp:BoundField HeaderText="Part Number" DataField="PartNumber">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle Width="20%" />
                                            </asp:BoundField>

                                            <asp:BoundField HeaderText="Serial Number" DataField="SerialNumber">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle Width="20%" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Estado">
                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />
                                                <ItemTemplate>
                                                    <span class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                        ? "badge bg-success-transparent text-success px-3 py-2 fs-12"
                                                        : Convert.ToInt16(Eval("Estado")) == 2
                                                            ? "badge bg-info-transparent text-info px-3 py-2 fs-12"
                                                            : "badge bg-danger-transparent text-danger px-3 py-2 fs-12" %>'>

                                                        <i class='<%# Convert.ToInt16(Eval("Estado")) == 0
                                                            ? "fe fe-check-circle me-1"
                                                            : Convert.ToInt16(Eval("Estado")) == 2
                                                                ? "fe fe-smartphone me-1"
                                                                : "fe fe-x-circle me-1" %>'></i>

                                                        <%# Convert.ToInt16(Eval("Estado")) == 0
                                                            ? "Vigente"
                                                            : Convert.ToInt16(Eval("Estado")) == 2
                                                                ? "Activado"
                                                                : "Anulado" %>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Acciones">
                                                <HeaderStyle CssClass="text-center" Width="10%" />
                                                <ItemStyle CssClass="text-center" Width="10%" />
                                                <ItemTemplate>
                                                    <asp:HyperLink
                                                        ID="lnkEditar"
                                                        runat="server"
                                                        CssClass="btn ripple btn-outline-info btn-sm"
                                                        ToolTip="Editar">
                                                        <i class="fe fe-edit"></i>
                                                    </asp:HyperLink>

                                                    <a href="javascript:void(0);"
                                                       class="btn ripple btn-outline-secondary btn-sm ms-1"
                                                       title="Regenerar código de activación"
                                                       onclick='regenerarCodigoDesdeAccion("<%# Eval("Id") %>"); return false;'>
                                                        <i class="bi bi-key-fill"></i>
                                                    </a>
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

    <!-- Código de Activación -->
    <div class="modal fade" id="modalCodigoActivacion" tabindex="-1" aria-labelledby="modalCodigoActivacionLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content custom-card border-0">
                <div class="modal-body text-center p-5">

                    <div class="mb-4">
                        <span class="avatar avatar-xl rounded-circle bg-primary-transparent text-primary">
                            <i class="bi bi-key-fill fs-28"></i>
                        </span>
                    </div>

                    <h5 class="mb-2 fw-semibold" id="modalCodigoActivacionLabel">
                        Código de activación regenerado
                    </h5>

                    <p class="text-muted mb-4">
                        Este código debe ser entregado al usuario para activar el dispositivo.
                    </p>

                    <div class="border rounded-3 p-3 mb-4">
                        <p class="mb-1 tx-13 text-muted">Código de activación</p>
                        <h3 class="mb-0 fw-bold text-primary" id="lblCodigoActivacionGenerado"></h3>
                    </div>

                    <div class="alert alert-warning mb-4" role="alert">
                        Este código es de un solo uso. Si el usuario lo olvida o lo pierde, deberá regenerarse uno nuevo.
                    </div>

                    <div class="d-flex justify-content-center gap-2">
                        <button type="button"
                                class="btn btn-outline-secondary"
                                style="min-width: 130px;"
                                onclick="copiarCodigoActivacion();">
                            Copiar
                        </button>

                        <button type="button"
                                class="btn btn-outline-info"
                                style="min-width: 130px;"
                                data-bs-dismiss="modal">
                            Cerrar
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Fin Código de Activación -->

</asp:Content>