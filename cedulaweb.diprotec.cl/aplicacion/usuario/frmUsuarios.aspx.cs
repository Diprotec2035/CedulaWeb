using Modelo;
using Modelo.v1.ModuloPerfil;
using Modelo.v1.Usuario;
using Negocio;
using Negocio.v1.MFA;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.usuario
{
    public partial class frmUsuarios : Page
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

                LoadUsuarios();
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

        private void LoadUsuarios()
        {
            var script = string.Empty;

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosUsuariosTituloListadoVacio;

            sbyte estadoSeleccionado;
            sbyte? estado = null;

            if (sbyte.TryParse(ddlEstado.SelectedValue, out estadoSeleccionado) &&
                (estadoSeleccionado == 0 || estadoSeleccionado == 1))
            {
                estado = estadoSeleccionado;
            }

            var filtro = new EmpresaUsuarioFiltroModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Coincidencia = txtCoincidencia.Text.Trim(),
                Estado = estado,
                Orden = Convert.ToInt16(ddlOrdenarPor.SelectedValue),
                Orientacion = Convert.ToInt16(ddlDireccion.SelectedValue)
            };

            var respuesta = EmpresaUsuarioNegocio.GetUsuarios(filtro);

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

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadUsuarios", script, true);
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadUsuarios();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadUsuarios();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            if (!PermisoModuloHelper.TienePermisoNuevo(this, Request.Url.AbsolutePath))
            {
                var script = PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoNuevoUsuario", script, true);
                return;
            }

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlNuevo), false);
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

            var usuario = e.Row.DataItem as EmpresaUsuarioModelo;

            if (usuario == null || string.IsNullOrWhiteSpace(usuario.RutUsuario))
            {
                return;
            }

            var rutUsuario = usuario.RutUsuario;
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlEditar, "id=" + HttpUtility.UrlEncode(rutUsuario));

            e.Row.Attributes["ondblclick"] = "window.location='" + url + "';";
            e.Row.Style["cursor"] = "pointer";

            var lnkEditar = e.Row.FindControl("lnkEditar") as HyperLink;

            if (lnkEditar != null)
            {
                lnkEditar.NavigateUrl = url;
            }

            var lnkRestablecerMFA = e.Row.FindControl("lnkRestablecerMFA") as LinkButton;

            if (lnkRestablecerMFA != null)
            {
                var puedeRestablecerMFA = PuedeRestablecerMFA(usuario);

                lnkRestablecerMFA.Visible = puedeRestablecerMFA;

                if (puedeRestablecerMFA)
                {
                    var rutUsuarioJs = HttpUtility.JavaScriptStringEncode(usuario.RutUsuario);
                    lnkRestablecerMFA.OnClientClick = "return abrirConfirmacionRestablecerMFA('" + rutUsuarioJs + "');";
                }
            }
        }

        private bool PuedeRestablecerMFA(EmpresaUsuarioModelo usuario)
        {
            if (usuario == null ||
                usuario.Id == 0 ||
                string.IsNullOrWhiteSpace(usuario.RutUsuario) ||
                usuario.Estado != 0 ||
                usuario.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido ||
                !usuario.MFATotpId.HasValue ||
                usuario.MFATotpId.Value == 0 ||
                !usuario.MFAEstado.HasValue)
            {
                return false;
            }

            if (usuario.MFAEstado.Value != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                return false;
            }

            var esSuperAdminSesion = Convert.ToString(Session["superAdmin"]) == "1";
            var rutUsuarioSesion = Convert.ToString(Session["cliente"]);

            if (!esSuperAdminSesion &&
                !string.IsNullOrWhiteSpace(rutUsuarioSesion) &&
                string.Equals(usuario.RutUsuario.Trim(), rutUsuarioSesion.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        protected void btnRestablecerMFA_Click(object sender, EventArgs e)
        {
            var urlModulo = Request.Url.AbsolutePath;

            if (!PermisoModuloHelper.TienePermisoModificar(this, urlModulo))
            {
                hfRutUsuarioMFA.Value = string.Empty;

                var scriptSinPermiso = PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoModificarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRestablecerMFA", scriptSinPermiso, true);
                return;
            }

            var rutUsuario = hfRutUsuarioMFA.Value.Trim();
            var rutUsuarioEjecutor = Convert.ToString(Session["cliente"]);
            var esSuperAdminSesion = Convert.ToString(Session["superAdmin"]) == "1";

            hfRutUsuarioMFA.Value = string.Empty;

            if (string.IsNullOrWhiteSpace(rutUsuario) || string.IsNullOrWhiteSpace(rutUsuarioEjecutor))
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            rutUsuarioEjecutor = rutUsuarioEjecutor.Trim();

            if (!esSuperAdminSesion && string.Equals(rutUsuario, rutUsuarioEjecutor, StringComparison.OrdinalIgnoreCase))
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            var operacion = 0;

            var usuario = EmpresaUsuarioNegocio.GetEmpresaUsuario(ref operacion, new EmpresaUsuarioModelo
            {
                RutEmpresa = Convert.ToString(Session["empresa"]),
                RutUsuario = rutUsuario,
                Estado = null
            });

            if (operacion != 0 ||
                usuario == null ||
                usuario.Id == 0 ||
                usuario.EstadoEmpresa != 0 ||
                usuario.Estado != 0 ||
                usuario.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            var respuestaConfiguracion = MFATotpNegocio.GetConfiguracion(usuario.Id, string.Empty);

            if (!respuestaConfiguracion.Exitoso ||
                respuestaConfiguracion.Datos == null ||
                respuestaConfiguracion.Datos.Id == 0 ||
                respuestaConfiguracion.Datos.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            var respuesta = MFATotpNegocio.RestablecerConfiguracion(usuario.Id, string.Empty, rutUsuarioEjecutor);

            if (respuesta.Exitoso)
            {
                LoadUsuarios();
            }

            MostrarResultadoRestablecimientoMFA(respuesta.Exitoso, respuesta.Mensaje, respuesta.AlertaColor, respuesta.AlertaTitulo);
        }

        private void MostrarResultadoRestablecimientoMFA(bool exitoso, string mensaje, string alertaColor, string alertaTitulo)
        {
            var mensajeResultado = string.IsNullOrWhiteSpace(mensaje) ? ConstantesModelo.ConsErrorMfaTotpRestablecer : mensaje;
            var colorResultado = string.IsNullOrWhiteSpace(alertaColor) ? (exitoso ? ConstantesModelo.ConsAlertaColorSuccess : ConstantesModelo.ConsAlertaColorDanger) : alertaColor;
            var tituloResultado = alertaTitulo ?? string.Empty;

            var mensajeJs = HttpUtility.JavaScriptStringEncode(HttpUtility.HtmlDecode(mensajeResultado));
            var colorJs = HttpUtility.JavaScriptStringEncode(colorResultado);
            var tituloJs = HttpUtility.JavaScriptStringEncode(HttpUtility.HtmlDecode(tituloResultado));
            var script = "$('#global-loader').hide();" + "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), "RestablecerMFA", script, true);
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
    }
}