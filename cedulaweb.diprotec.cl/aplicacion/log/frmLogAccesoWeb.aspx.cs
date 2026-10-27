using Modelo;
using Modelo.v1.Log;
using Negocio;
using Negocio.v1.Log;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace cedulaweb.diprotec.cl.aplicacion.log
{
    public partial class frmLogAccesoWeb : Page
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

            if (!IsPostBack)
            {
                InicializarFechasFiltro();
                LoadLogAccesoWeb();
            }
        }

        private void LoadLogAccesoWeb()
        {
            var script = string.Empty;

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosLogAccesoWebTituloListadoVacio;

            AsegurarFechasFiltro();

            var filtro = new LogAccesoWebFiltroModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Coincidencia = txtCoincidencia.Text.Trim(),
                Desde = hdnFechaDesde.Value.Trim(),
                Hasta = hdnFechaHasta.Value.Trim(),
                Orden = Convert.ToInt16(ddlOrdenarPor.SelectedValue),
                Orientacion = Convert.ToInt16(ddlDireccion.SelectedValue)
            };

            var respuesta = LogAccesoWebNegocio.GetLogAccesoWeb(filtro);

            if (respuesta.Exitoso)
            {
                gvListado.DataSource = respuesta.Datos;
                gvListado.DataBind();
            }
            else
            {
                gvListado.DataSource = null;
                gvListado.DataBind();

                var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
                var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
                var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

                script = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadLogAccesoWeb", script, true);
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;
            LoadLogAccesoWeb();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;
            LoadLogAccesoWeb();
        }

        protected void gvListado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
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
    }
}