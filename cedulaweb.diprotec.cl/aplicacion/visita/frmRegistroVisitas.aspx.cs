using Modelo;
using Modelo.v1.Visitas;
using Negocio;
using Negocio.v1.Visitas;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.visita
{
    public partial class frmRegistroVisitas : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (Session["empresa"] == null || Session["cliente"] == null)
            {
                Response.Redirect(ConstantesModelo.ConsHttpInicio, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Session["SesionExpira"] == null)
            {
                var minutosSesion = FuncionesNegocio.GetSessionTimeoutMinutos();

                HttpContext.Current.Session.Timeout = minutosSesion;
                HttpContext.Current.Session["SesionExpira"] = DateTime.Now.AddMinutes(minutosSesion);
            }

            var urlModulo = Request.Url.AbsolutePath;

            if (!PermisoModuloHelper.ValidarPuedeVer(this, urlModulo, ConstantesModelo.ConsModulosDefault))
            {
                return;
            }

            if (!IsPostBack)
            {
                if (ListadoFiltroSesionHelper.DebeMantenerFiltros(this))
                {
                    ListadoFiltroSesionHelper.Restaurar(this, pnlFiltros);

                    var filtrosSesion = ListadoFiltroSesionHelper.Obtener(this);

                    if (filtrosSesion != null)
                    {
                        gvListado.PageIndex = filtrosSesion.PageIndex;
                    }
                }
                else
                {
                    ListadoFiltroSesionHelper.Limpiar(this);
                    InicializarFechasFiltro();
                }

                LoadRegistroVisitas();
            }
        }

        private RegistroVisitaFiltroModelo ObtenerFiltro()
        {
            AsegurarFechasFiltro();

            return new RegistroVisitaFiltroModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Coincidencia = txtCoincidencia.Text.Trim(),
                Operacion = Convert.ToInt16(ddlOperacion.SelectedValue),
                Desde = hdnFechaDesde.Value.Trim(),
                Hasta = hdnFechaHasta.Value.Trim(),
                Orden = Convert.ToInt16(ddlOrdenarPor.SelectedValue),
                Orientacion = Convert.ToInt16(ddlDireccion.SelectedValue)
            };
        }

        private void LoadRegistroVisitas()
        {
            var script = string.Empty;

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosRegistroVisitasTituloListadoVacio;

            var filtro = ObtenerFiltro();

            var respuesta = RegistroVisitaNegocio.GetRegistroVisitas(filtro);

            if (respuesta.Exitoso)
            {
                gvListado.DataSource = respuesta.Datos;
                gvListado.DataBind();
            }
            else
            {
                gvListado.DataSource = null;
                gvListado.DataBind();

                var mensaje = string.IsNullOrWhiteSpace(respuesta.Mensaje) ? ConstantesModelo.ConsErrorGenericoConsultar : respuesta.Mensaje;
                var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);

                script = "mostrarAlerta('danger', '', '" + mensajeJs + "');";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadRegistroVisitas", script, true);
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadRegistroVisitas();
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            var filtro = ObtenerFiltro();

            var respuesta = RegistroVisitaNegocio.GetRegistroVisitas(filtro);

            if (respuesta == null || !respuesta.Exitoso)
            {
                if (respuesta != null)
                {
                    MostrarAlerta(
                        string.IsNullOrWhiteSpace(respuesta.AlertaColor) ? ConstantesModelo.ConsAlertaColorWarning : respuesta.AlertaColor,
                        respuesta.AlertaTitulo,
                        string.IsNullOrWhiteSpace(respuesta.Mensaje)
                            ? ConstantesModelo.ConsErrorReporteExportar
                            : respuesta.Mensaje
                    );
                }
                else
                {
                    MostrarAlerta(
                        ConstantesModelo.ConsAlertaColorDanger,
                        string.Empty,
                        ConstantesModelo.ConsErrorReporteExportar
                    );
                }

                return;
            }

            ExportarExcelRegistroVisitas(respuesta.Datos);
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadRegistroVisitas();
        }

        protected void gvListado_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                return;
            }

            if (e.Row.Cells.Count == 0 || e.Row.Cells[0].Controls.Count == 0)
            {
                return;
            }

            var tablaPaginador = e.Row.Cells[0].Controls[0] as Table;

            if (tablaPaginador == null || tablaPaginador.Rows.Count == 0)
            {
                return;
            }

            tablaPaginador.CssClass = "pagination justify-content-center mb-0";

            foreach (TableCell celda in tablaPaginador.Rows[0].Cells)
            {
                celda.CssClass = "page-item";

                if (celda.Controls.Count == 0)
                {
                    continue;
                }

                var control = celda.Controls[0];
                var link = control as LinkButton;

                if (link != null)
                {
                    link.CssClass = "page-link";
                    continue;
                }

                var label = control as Label;

                if (label != null)
                {
                    label.CssClass = "page-link";
                    celda.CssClass = "page-item active";
                    continue;
                }

                var literal = control as LiteralControl;

                if (literal != null && !string.IsNullOrWhiteSpace(literal.Text))
                {
                    var texto = literal.Text.Trim();

                    celda.Controls.Clear();
                    celda.Controls.Add(new Label
                    {
                        Text = texto,
                        CssClass = "page-link"
                    });

                    celda.CssClass = "page-item active";
                }
            }
        }

        private void InicializarFechasFiltro()
        {
            var fechaHasta = DateTime.Now.Date;
            var fechaDesde = fechaHasta.AddYears(-1);

            txtFechaDesde.Text = fechaDesde.ToString("dd-MM-yyyy");
            txtFechaHasta.Text = fechaHasta.ToString("dd-MM-yyyy");

            hdnFechaDesde.Value = txtFechaDesde.Text;
            hdnFechaHasta.Value = txtFechaHasta.Text;
        }

        private void AsegurarFechasFiltro()
        {
            var fechaDesde = hdnFechaDesde.Value.Trim();
            var fechaHasta = hdnFechaHasta.Value.Trim();

            if (string.IsNullOrWhiteSpace(fechaDesde))
            {
                fechaDesde = txtFechaDesde.Text.Trim();
            }

            if (string.IsNullOrWhiteSpace(fechaHasta))
            {
                fechaHasta = txtFechaHasta.Text.Trim();
            }

            if (string.IsNullOrWhiteSpace(fechaDesde) ||
                string.IsNullOrWhiteSpace(fechaHasta))
            {
                InicializarFechasFiltro();
                return;
            }

            hdnFechaDesde.Value = fechaDesde;
            hdnFechaHasta.Value = fechaHasta;

            txtFechaDesde.Text = fechaDesde;
            txtFechaHasta.Text = fechaHasta;
        }

        private void ExportarExcelRegistroVisitas(List<ReporteRegistroVisitaModelo> registros)
        {
            if (registros == null)
            {
                registros = new List<ReporteRegistroVisitaModelo>();
            }

            var nombreArchivo = "REGISTRO_VISITAS_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            try
            {
                ExcelPackage.License.SetNonCommercialOrganization("Diprotec");

                using (var paquete = new ExcelPackage())
                {
                    var hoja = paquete.Workbook.Worksheets.Add("Visitas");

                    var encabezados = new[]
                    {
                        "RUT", "NOMBRE", "EMPRESA", "PATENTE", "FECHA",
                        "HORA", "COMENTARIO", "MARCA", "MODELO", "SERIE"
                    };

                    for (var columna = 0; columna < encabezados.Length; columna++)
                    {
                        hoja.Cells[1, columna + 1].Value = encabezados[columna];
                    }

                    using (var rango = hoja.Cells[1, 1, 1, encabezados.Length])
                    {
                        rango.Style.Font.Bold = true;
                        rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rango.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }

                    var fila = 2;

                    foreach (var registro in registros)
                    {
                        hoja.Cells[fila, 1].Value = registro.Rut;
                        hoja.Cells[fila, 2].Value = registro.NombreApellido;
                        hoja.Cells[fila, 3].Value = registro.Empresa;
                        hoja.Cells[fila, 4].Value = registro.Patente;
                        hoja.Cells[fila, 5].Value = registro.Fecha;
                        hoja.Cells[fila, 6].Value = registro.Hora;
                        hoja.Cells[fila, 7].Value = registro.Comentario;
                        hoja.Cells[fila, 8].Value = registro.Marca;
                        hoja.Cells[fila, 9].Value = registro.Modelo;
                        hoja.Cells[fila, 10].Value = registro.SerialNumber;

                        fila++;
                    }

                    if (hoja.Dimension != null)
                    {
                        hoja.Cells[hoja.Dimension.Address].AutoFitColumns();
                    }

                    var archivo = paquete.GetAsByteArray();

                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + nombreArchivo);
                    Response.BinaryWrite(archivo);
                    Response.Flush();
                    Response.SuppressContent = true;

                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception)
            {
                MostrarAlerta(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsErrorReporteExportar);
            }
        }

        private void MostrarAlerta(string color, string titulo, string mensaje)
        {
            var script = string.Format(
                "mostrarAlerta('{0}', '{1}', '{2}');",
                HttpUtility.JavaScriptStringEncode(color ?? ConstantesModelo.ConsAlertaColorDanger),
                HttpUtility.JavaScriptStringEncode(titulo ?? string.Empty),
                HttpUtility.JavaScriptStringEncode(mensaje ?? string.Empty)
            );

            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), script, true);
        }
    }
}
