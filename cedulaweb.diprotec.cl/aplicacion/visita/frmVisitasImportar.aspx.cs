using Modelo;
using Modelo.v1.ModuloPerfil;
using Modelo.v1.Visitas;
using Negocio;
using Negocio.v1.Visitas;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.visita
{
    public partial class frmVisitasImportar : Page
    {
        private PermisoModuloUsuarioModelo permisosModulo;

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

            permisosModulo = PermisoModuloHelper.ObtenerPermisoModulo(this, urlModulo);

            if (!PermisoModuloHelper.ValidarPuedeVer(this, urlModulo, ConstantesModelo.ConsModulosDefault))
            {
                return;
            }

            AplicarPermisosVisuales();

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

                LoadEmpresaVisitaCarga();
            }
        }

        private void AplicarPermisosVisuales()
        {
            btnNuevo.Visible = permisosModulo != null && permisosModulo.PuedeNuevo;
        }

        private void LoadEmpresaVisitaCarga()
        {
            var script = string.Empty;

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosVisitasCargaTituloListadoVacio;

            AsegurarFechasFiltro();

            var filtro = new EmpresaVisitaCargaFiltroModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Coincidencia = txtCoincidencia.Text.Trim(),
                Estado = Convert.ToSByte(ddlEstado.SelectedValue),
                Desde = hdnFechaDesde.Value.Trim(),
                Hasta = hdnFechaHasta.Value.Trim(),
                Orden = Convert.ToInt16(ddlOrdenarPor.SelectedValue),
                Orientacion = Convert.ToInt16(ddlDireccion.SelectedValue)
            };

            var respuesta = EmpresaVisitaCargaNegocio.GetEmpresaVisitaCarga(filtro);

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

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadEmpresaVisitaCarga", script, true);
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadEmpresaVisitaCarga();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            if (!PermisoModuloHelper.TienePermisoNuevo(this, Request.Url.AbsolutePath))
            {
                var script = PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoImportarVisita", script, true);
                return;
            }

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlNuevo);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadEmpresaVisitaCarga();
        }

        protected void gvListado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var id = gvListado.DataKeys[e.Row.RowIndex].Value.ToString();
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlDetalle, "id=" + HttpUtility.UrlEncode(id));
            var urlJs = HttpUtility.JavaScriptStringEncode(url);

            e.Row.Attributes["ondblclick"] = "window.location='" + urlJs + "';";
            e.Row.Style["cursor"] = "pointer";

            var lnkDetalle = e.Row.FindControl("lnkDetalle") as HyperLink;

            if (lnkDetalle != null)
            {
                lnkDetalle.NavigateUrl = url;
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
