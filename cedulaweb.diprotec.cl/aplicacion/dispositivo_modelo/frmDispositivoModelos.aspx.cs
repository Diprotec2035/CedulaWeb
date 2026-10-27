using Modelo;
using Modelo.v1.DispositivoMarcas;
using Modelo.v1.DispositivoModelos;
using Negocio;
using Negocio.v1.DispositivoMarcas;
using Negocio.v1.DispositivoModelos;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.dispositivo_modelo
{
    public partial class frmDispositivoModelos : Page
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

            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorPermisoAccesoModulo;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            var contextoModulo = ModuloContextoHelper.ValidarModuloPagina(this);

            if (!contextoModulo.Exitoso)
            {
                var mensaje = string.IsNullOrWhiteSpace(contextoModulo.Mensaje)
                    ? ConstantesModelo.ConsValidacionModuloContextoError
                    : contextoModulo.Mensaje;

                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = mensaje;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadMarcas();

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
                }

                LoadModelos();
            }
        }

        private void LoadMarcas()
        {
            ddlMarca.Items.Clear();
            ddlMarca.Items.Add(new ListItem("Todas", "0"));

            var filtro = new DispositivoMarcasFiltroModelo
            {
                Coincidencia = string.Empty,
                Estado = 0,
                Orden = 1,
                Orientacion = 1
            };

            var respuesta = DispositivoMarcasNegocio.GetReporteDispositivoMarcas(filtro);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var item in respuesta.Datos)
            {
                ddlMarca.Items.Add(new ListItem(item.Nombre, item.Id.ToString()));
            }
        }

        private DispositivoModelosFiltroModelo ObtenerFiltro()
        {
            uint idMarca;
            uint.TryParse(ddlMarca.SelectedValue, out idMarca);

            short estado;
            short.TryParse(ddlEstado.SelectedValue, out estado);

            short orden = 1;
            short orientacion = 1;

            return new DispositivoModelosFiltroModelo
            {
                Coincidencia = txtCoincidencia.Text != null ? txtCoincidencia.Text.Trim() : string.Empty,
                IdMarca = idMarca == 0 ? (uint?)null : idMarca,
                Estado = estado == 2 ? (short?)null : estado,
                Orden = orden,
                Orientacion = orientacion
            };
        }

        private void LoadModelos()
        {
            var filtro = ObtenerFiltro();

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosDispositivoModelosTituloListadoVacio;

            var respuesta = DispositivoModelosNegocio.GetReporteDispositivoModelos(filtro);

            if (respuesta == null || !respuesta.Exitoso)
            {
                gvListado.DataSource = new List<DispositivoModelosModelo>();
                gvListado.DataBind();

                if (respuesta != null && !string.IsNullOrWhiteSpace(respuesta.Mensaje))
                {
                    MostrarAlerta(string.IsNullOrWhiteSpace(respuesta.AlertaColor) ? ConstantesModelo.ConsAlertaColorDanger : respuesta.AlertaColor, respuesta.AlertaTitulo, respuesta.Mensaje);
                }

                return;
            }

            gvListado.DataSource = respuesta.Datos;
            gvListado.DataBind();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadModelos();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlNuevo);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadModelos();
        }

        protected void gvListado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var item = e.Row.DataItem as DispositivoModelosModelo;

            if (item == null || item.Id == 0)
            {
                return;
            }

            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlEditar, "id=" + HttpUtility.UrlEncode(item.Id.ToString()));
            var urlJs = HttpUtility.JavaScriptStringEncode(url);

            e.Row.Attributes["ondblclick"] = "window.location='" + urlJs + "';";
            e.Row.Style["cursor"] = "pointer";
            e.Row.Attributes["title"] = "Doble click para editar";

            var lnkEditar = e.Row.FindControl("lnkEditar") as HyperLink;

            if (lnkEditar != null)
            {
                lnkEditar.NavigateUrl = url;
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

        private void MostrarAlerta(string color, string titulo, string mensaje)
        {
            var script = string.Format("mostrarAlerta('{0}', '{1}', '{2}');", HttpUtility.JavaScriptStringEncode(color ?? ConstantesModelo.ConsAlertaColorDanger), HttpUtility.JavaScriptStringEncode(titulo ?? string.Empty), HttpUtility.JavaScriptStringEncode(mensaje ?? string.Empty));

            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), script, true);
        }
    }
}