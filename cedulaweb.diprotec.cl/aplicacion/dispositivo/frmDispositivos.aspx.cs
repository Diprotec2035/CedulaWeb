using Modelo;
using Modelo.v1.Dispositivo;
using Modelo.v1.ModuloPerfil;
using Negocio;
using Negocio.v1.Dispositivo;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.dispositivo
{
    public partial class frmDispositivos : Page
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
                }

                LoadDispositivos();
            }
        }

        private void AplicarPermisosVisuales()
        {
            btnNuevo.Visible = permisosModulo.PuedeNuevo;
            AplicarPermisoModificarGrid();
        }

        private void AplicarPermisoModificarGrid()
        {
            foreach (DataControlField columna in gvListado.Columns)
            {
                if (string.Equals(columna.HeaderText, "Acciones", StringComparison.OrdinalIgnoreCase))
                {
                    columna.Visible = permisosModulo.PuedeModificar;
                    return;
                }
            }
        }

        private void LoadDispositivos()
        {
            var script = string.Empty;

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosDispositivosTituloListadoVacio;

            sbyte estadoSeleccionado;
            sbyte? estado = null;

            if (sbyte.TryParse(ddlEstado.SelectedValue, out estadoSeleccionado) &&
                (estadoSeleccionado == 0 || estadoSeleccionado == 1 || estadoSeleccionado == 2))
            {
                estado = estadoSeleccionado;
            }

            var filtro = new DispositivoFiltroModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Coincidencia = txtCoincidencia.Text.Trim(),
                Estado = estado,
                Orden = Convert.ToInt16(ddlOrdenarPor.SelectedValue),
                Orientacion = Convert.ToInt16(ddlDireccion.SelectedValue)
            };

            var respuesta = DispositivoNegocio.GetDispositivos(filtro);

            if (respuesta.Exitoso)
            {
                gvListado.DataSource = respuesta.Datos;
                gvListado.DataBind();
            }
            else
            {
                var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
                var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
                var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

                script = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadDispositivos", script, true);
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadDispositivos();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadDispositivos();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            if (!PermisoModuloHelper.TienePermisoNuevo(this, Request.Url.AbsolutePath))
            {
                var script = PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoNuevoDispositivo", script, true);
                return;
            }

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlNuevo);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void gvListado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            if (permisosModulo == null || !permisosModulo.PuedeModificar)
            {
                e.Row.Style["cursor"] = "default";
                return;
            }

            var id = gvListado.DataKeys[e.Row.RowIndex].Value.ToString();
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlEditar, "id=" + HttpUtility.UrlEncode(id));
            var urlJs = HttpUtility.JavaScriptStringEncode(url);

            e.Row.Attributes["ondblclick"] = "window.location='" + urlJs + "';";
            e.Row.Style["cursor"] = "pointer";

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

        protected void btnRegenerarCodigoOculto_Click(object sender, EventArgs e)
        {
            if (!PermisoModuloHelper.TienePermisoModificar(this, Request.Url.AbsolutePath))
            {
                var scriptSinPermiso = PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoModificarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegenerarCodigoDispositivo", scriptSinPermiso, true);
                return;
            }

            uint dispositivoId;

            if (!uint.TryParse(hfRegenerar.Value, out dispositivoId) || dispositivoId == 0)
            {
                var mensajeIdInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);
                var scriptErrorId = "mostrarAlerta('danger', '', '" + mensajeIdInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "DispositivoRegenerarIdInvalido", scriptErrorId, true);
                return;
            }

            var dispositivo = new DispositivoModelo
            {
                Id = dispositivoId,
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"])
            };

            var respuesta = DispositivoNegocio.RegenerarCodigoActivacion(dispositivo);

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            var script = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";

            if (respuesta.Exitoso)
            {
                var activationCode = respuesta.Datos != null
                    ? respuesta.Datos.ActivationCode
                    : string.Empty;

                var activationCodeJs = HttpUtility.JavaScriptStringEncode(activationCode);

                script += "mostrarModalCodigoActivacion('" + activationCodeJs + "');";

                LoadDispositivos();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "btnRegenerarCodigoOculto_Click", script, true);
        }
    }
}