<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucBitacora.ascx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.control.ucBitacora" %>
<%@ Import Namespace="Modelo" %>

<!-- Modal Bitácora -->
<div class="modal fade" id="<%= ModalClientId %>" tabindex="-1" role="dialog" aria-labelledby="<%= ModalLabelClientId %>" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
    <div class="modal-dialog modal-xl modal-dialog-centered" role="document" style="max-width: 88vw;">
        <div class="modal-content">

            <div class="modal-header">
                <div>
                    <h6 class="modal-title" id="<%= ModalLabelClientId %>">
                        <%=ConstantesModelo.ConsModulosBitacoraTitulo%>
                    </h6>
                </div>

                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
            </div>

            <div class="modal-body">

                <!-- Cabecera fija -->
                <div class="table-responsive p-0 border-start border-end border-top">
                    <table class="table text-start mg-b-0" style="min-width: 1050px; width: 100%;">
                        <thead>
                            <tr>
                                <th class="py-2" style="height: 35px; width: 7%; text-transform: none; font-size: 14px;">Fecha</th>
                                <th class="py-2" style="height: 35px; width: 7%; text-transform: none; font-size: 14px;">Hora</th>
                                <th class="py-2" style="height: 35px; width: 7%; text-transform: none; font-size: 14px;">Usuario</th>
                                <th class="py-2" style="height: 35px; width: 20%; text-transform: none; font-size: 14px;">Nombre</th>
                                <th class="py-2" style="height: 35px;width: 59%; text-transform: none; font-size: 14px;">Detalle</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <!-- End Cabecera fija -->

                <!-- Cuerpo con scroll -->
                <div class="table-responsive p-0 border-start border-end border-bottom" style="max-height: 60vh; overflow: auto;">

                    <asp:GridView
                        ID="gvBitacora"
                        runat="server"
                        AutoGenerateColumns="False"
                        ShowHeader="False"
                        AllowPaging="False"
                        CssClass="table table-hover text-start mg-b-0"
                        Style="min-width: 1050px; width: 100%;"
                        GridLines="None"
                        EmptyDataText="No existen registros de bitácora.">

                        <EmptyDataRowStyle CssClass="text-center tx-14 text-muted" />

                        <Columns>
                            <asp:BoundField HeaderText="Fecha" DataField="Fecha">
                                <ItemStyle Width="7%" CssClass="text-nowrap tx-14 py-2" />
                            </asp:BoundField>

                            <asp:BoundField HeaderText="Hora" DataField="Hora">
                                <ItemStyle Width="7%" CssClass="text-nowrap tx-14 py-2" />
                            </asp:BoundField>

                            <asp:BoundField HeaderText="Usuario" DataField="RutUsuario">
                                <ItemStyle Width="7%" CssClass="text-nowrap tx-14 py-2" />
                            </asp:BoundField>

                            <asp:BoundField HeaderText="Nombre" DataField="NombreUsuario">
                                <ItemStyle Width="20%" CssClass="text-nowrap tx-14 py-2" />
                            </asp:BoundField>

                            <asp:BoundField HeaderText="Detalle" DataField="Detalle">
                                <ItemStyle Width="59%" CssClass="tx-14 py-2" />
                            </asp:BoundField>
                        </Columns>

                    </asp:GridView>

                </div>
                <!-- End Cuerpo con scroll -->

            </div>

            <div class="modal-footer justify-content-center">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cerrar</button>
            </div>

        </div>
    </div>
</div>
<!-- End Modal Bitácora -->