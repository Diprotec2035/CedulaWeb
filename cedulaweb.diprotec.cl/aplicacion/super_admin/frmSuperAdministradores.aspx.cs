using Modelo;
using Modelo.v1.SuperAdmin;
using Negocio;
using Negocio.v1.MFA;
using Negocio.v1.SuperAdmin;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.super_admin
{
    public partial class frmSuperAdministradores : Page
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
                var mensaje = string.IsNullOrWhiteSpace(contextoModulo.Mensaje) ? ConstantesModelo.ConsValidacionModuloContextoError : contextoModulo.Mensaje;

                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = mensaje;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
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
                }

                LoadSuperAdministradores();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadSuperAdministradores();
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

            LoadSuperAdministradores();
        }

        protected void gvListado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var superAdmin = e.Row.DataItem as SuperAdminModelo;

            if (superAdmin == null || string.IsNullOrWhiteSpace(superAdmin.RutAdmin))
            {
                return;
            }

            var rutSuperAdmin = superAdmin.RutAdmin;
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlEditar, "id=" + HttpUtility.UrlEncode(rutSuperAdmin));
            var urlJs = HttpUtility.JavaScriptStringEncode(url);

            e.Row.Attributes["ondblclick"] = "window.location='" + urlJs + "';";
            e.Row.Style["cursor"] = "pointer";

            var lnkEditar = e.Row.FindControl("lnkEditar") as HyperLink;

            if (lnkEditar != null)
            {
                lnkEditar.NavigateUrl = url;
            }

            var lnkRestablecerMFA = e.Row.FindControl("lnkRestablecerMFA") as LinkButton;

            if (lnkRestablecerMFA != null)
            {
                var puedeRestablecerMFA = PuedeRestablecerMFA(superAdmin);

                lnkRestablecerMFA.Visible = puedeRestablecerMFA;

                if (puedeRestablecerMFA)
                {
                    var rutSuperAdminJs = HttpUtility.JavaScriptStringEncode(superAdmin.RutAdmin);
                    lnkRestablecerMFA.OnClientClick = "return abrirConfirmacionRestablecerMFA('" + rutSuperAdminJs + "');";
                }
            }
        }

        private bool PuedeRestablecerMFA(SuperAdminModelo superAdmin)
        {
            if (superAdmin == null ||
                string.IsNullOrWhiteSpace(superAdmin.RutAdmin) ||
                !superAdmin.Estado.HasValue ||
                superAdmin.Estado.Value != 0 ||
                superAdmin.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido ||
                !superAdmin.MFATotpId.HasValue ||
                superAdmin.MFATotpId.Value == 0 ||
                !superAdmin.MFAEstado.HasValue)
            {
                return false;
            }

            if (superAdmin.MFAEstado.Value != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                return false;
            }

            var rutSuperAdminSesion = Convert.ToString(Session["cliente"]);

            if (string.IsNullOrWhiteSpace(rutSuperAdminSesion))
            {
                return false;
            }

            if (string.Equals(superAdmin.RutAdmin.Trim(), rutSuperAdminSesion.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        protected void btnRestablecerMFA_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                hfRutSuperAdminMFA.Value = string.Empty;

                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorPermisoModificarRegistro, string.Empty, string.Empty);
                return;
            }

            var rutSuperAdmin = hfRutSuperAdminMFA.Value.Trim();
            var rutSuperAdminEjecutor = Convert.ToString(Session["cliente"]);

            hfRutSuperAdminMFA.Value = string.Empty;

            if (string.IsNullOrWhiteSpace(rutSuperAdmin) || string.IsNullOrWhiteSpace(rutSuperAdminEjecutor))
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            rutSuperAdmin = rutSuperAdmin.Trim();
            rutSuperAdminEjecutor = rutSuperAdminEjecutor.Trim();

            if (string.Equals(rutSuperAdmin, rutSuperAdminEjecutor, StringComparison.OrdinalIgnoreCase))
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            var operacion = 0;

            var superAdmin = SuperAdminNegocio.GetSuperAdmin(ref operacion, new SuperAdminModelo
            {
                RutAdmin = rutSuperAdmin,
                Estado = null
            });

            if (operacion != 0 ||
                superAdmin == null ||
                string.IsNullOrWhiteSpace(superAdmin.RutAdmin) ||
                !superAdmin.Estado.HasValue ||
                superAdmin.Estado.Value != 0 ||
                superAdmin.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            var respuestaConfiguracion = MFATotpNegocio.GetConfiguracion(null, superAdmin.RutAdmin);

            if (!respuestaConfiguracion.Exitoso ||
                respuestaConfiguracion.Datos == null ||
                respuestaConfiguracion.Datos.Id == 0 ||
                respuestaConfiguracion.Datos.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                MostrarResultadoRestablecimientoMFA(false, ConstantesModelo.ConsErrorMfaTotpRestablecer, string.Empty, string.Empty);
                return;
            }

            var respuesta = MFATotpNegocio.RestablecerConfiguracion(null, superAdmin.RutAdmin, rutSuperAdminEjecutor);

            if (respuesta.Exitoso)
            {
                LoadSuperAdministradores();
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

            var pagerTable = e.Row.Cells[0].Controls[0] as Table;

            if (pagerTable == null)
            {
                return;
            }

            foreach (TableRow row in pagerTable.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        var linkButton = control as LinkButton;

                        if (linkButton != null)
                        {
                            linkButton.CssClass = "btn btn-outline-primary btn-sm me-1";
                        }

                        var label = control as Label;

                        if (label != null)
                        {
                            label.CssClass = "btn btn-primary btn-sm me-1 disabled";
                        }
                    }
                }
            }
        }

        private void LoadSuperAdministradores()
        {
            var filtro = new SuperAdminFiltroModelo
            {
                Coincidencia = txtCoincidencia.Text.Trim(),
                Estado = ObtenerEstado(),
                Orden = ObtenerOrden(),
                Orientacion = ObtenerOrientacion()
            };

            var lista = SuperAdminNegocio.GetSuperAdmins(filtro);

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosSuperAdministradoresTituloListadoVacio;
            gvListado.DataSource = lista;
            gvListado.DataBind();
        }

        private short? ObtenerEstado()
        {
            if (!short.TryParse(ddlEstado.SelectedValue, out var estado))
            {
                return null;
            }

            if (estado == 2)
            {
                return null;
            }

            return estado;
        }

        private short ObtenerOrden()
        {
            if (!short.TryParse(ddlOrdenarPor.SelectedValue, out var orden))
            {
                return 1;
            }

            return orden;
        }

        private short ObtenerOrientacion()
        {
            if (!short.TryParse(ddlDireccion.SelectedValue, out var orientacion))
            {
                return 1;
            }

            return orientacion;
        }
    }
}