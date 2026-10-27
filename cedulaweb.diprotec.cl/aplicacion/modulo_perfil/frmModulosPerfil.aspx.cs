using Modelo;
using Modelo.Comunes;
using Modelo.v1.ModuloPerfil;
using Negocio;
using Negocio.v1.Categoria;
using Negocio.v1.ModuloPerfil;
using Negocio.v1.Perfil;
using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.modulo_perfil
{
    public partial class frmModulosPerfil : Page
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

            if (IsPostBack && Request["__EVENTTARGET"] == "ActualizarPermisoModulo")
            {
                ActualizarPermisoModulo();
                return;
            }

            if (!IsPostBack)
            {
                CargarPerfiles();
                CargarCategorias();
                LimpiarListado();
            }
        }

        private void CargarPerfiles()
        {
            var operacion = 0;
            var rutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]);

            var perfiles = PerfilNegocio.GetReportePerfiles(ref operacion, rutEmpresa, string.Empty, 0, 2, 1);

            ddlPerfil.Items.Clear();

            if (operacion == 0)
            {
                ddlPerfil.DataSource = perfiles;
                ddlPerfil.DataValueField = "Id";
                ddlPerfil.DataTextField = "Nombre";
                ddlPerfil.DataBind();
            }

            ddlPerfil.Items.Insert(0, new ListItem("Seleccione", "0"));
        }

        private void CargarCategorias()
        {
            var operacion = 0;

            var categorias = CategoriaNegocio.GetCategoriasModulosPerfil(ref operacion);

            ddlCategoria.Items.Clear();

            if (operacion == 0)
            {
                ddlCategoria.DataSource = categorias;
                ddlCategoria.DataValueField = "Id";
                ddlCategoria.DataTextField = "Nombre";
                ddlCategoria.DataBind();
            }

            ddlCategoria.Items.Insert(0, new ListItem("Seleccione", "0"));
        }

        private void ActualizarPermisoModulo()
        {
            if (!PermisoModuloHelper.TienePermisoModificar(this, Request.Url.AbsolutePath))
            {
                var scriptSinPermiso = PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoModificarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarPermisoModulo", scriptSinPermiso, true);
                return;
            }

            uint mpId;
            uint perId;
            uint catId;
            uint modId;
            sbyte valor;

            string script;

            if (!uint.TryParse(hfPermisoMpId.Value, out mpId))
            {
                mpId = 0;
            }

            if (!uint.TryParse(hfPermisoPerId.Value, out perId) || perId == 0)
            {
                var mensajePerfilJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionPerfilSeleccionVacio);
                script = "mostrarAlerta('warning', '', '" + mensajePerfilJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
                return;
            }

            if (!uint.TryParse(hfPermisoCatId.Value, out catId) || catId == 0)
            {
                var mensajeCategoriaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionCategoriaSeleccionVacia);
                script = "mostrarAlerta('warning', '', '" + mensajeCategoriaJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
                return;
            }

            if (!uint.TryParse(hfPermisoModId.Value, out modId) || modId == 0)
            {
                var mensajeModuloJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);
                script = "mostrarAlerta('warning', '', '" + mensajeModuloJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
                return;
            }

            if (!sbyte.TryParse(hfPermisoValor.Value, out valor) || (valor != 0 && valor != 1))
            {
                var mensajeEstadoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroEstadoInvalido);
                script = "mostrarAlerta('warning', '', '" + mensajeEstadoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
                return;
            }

            DateTime? updatedAtOriginal = null;

            if (!string.IsNullOrWhiteSpace(hfPermisoUpdatedAtOriginal.Value))
            {
                DateTime updatedAt;

                if (!DateTime.TryParseExact(hfPermisoUpdatedAtOriginal.Value, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out updatedAt))
                {
                    var mensajeVersionJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorRegistroObtener);
                    script = "mostrarAlerta('danger', '', '" + mensajeVersionJs + "');";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
                    return;
                }

                updatedAtOriginal = updatedAt;
            }

            if (mpId > 0 && !updatedAtOriginal.HasValue)
            {
                var mensajeVersionJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorRegistroObtener);
                script = "mostrarAlerta('danger', '', '" + mensajeVersionJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
                return;
            }

            var moduloPerfil = new ModuloPerfilModelo
            {
                Id = mpId,
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]),
                IdPerfil = perId,
                IdCategoria = catId,
                IdModulo = modId,
                UpdatedAt = updatedAtOriginal
            };

            RespuestaModelo<ModuloPerfilModelo> respuesta;

            switch (hfPermisoAccion.Value)
            {
                case "estado":
                    moduloPerfil.Estado = valor;
                    respuesta = ModuloPerfilNegocio.UpdateModuloPerfilEstado(moduloPerfil);
                    break;

                case "consulta":
                    moduloPerfil.Consulta = valor;
                    respuesta = ModuloPerfilNegocio.UpdateModuloPerfilConsulta(moduloPerfil);
                    break;

                case "nuevo":
                    moduloPerfil.Nuevo = valor;
                    respuesta = ModuloPerfilNegocio.UpdateModuloPerfilNuevo(moduloPerfil);
                    break;

                case "modifica":
                    moduloPerfil.Modifica = valor;
                    respuesta = ModuloPerfilNegocio.UpdateModuloPerfilModifica(moduloPerfil);
                    break;

                default:
                    var mensajeAccionJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionAccionInvalida);
                    script = "mostrarAlerta('warning', '', '" + mensajeAccionJs + "');";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
                    return;
            }

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            script = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";

            permisosModulo = PermisoModuloHelper.ObtenerPermisoModulo(this, Request.Url.AbsolutePath);

            if (permisosModulo == null || !permisosModulo.PuedeVer)
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorPermisoAccesoModulo;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            LoadModulosPerfil();

            ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarPermisoModulo", script, true);
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string script;

            if (ddlPerfil.SelectedValue == "0")
            {
                LimpiarListado();

                var mensajePerfilJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionPerfilSeleccionVacio);
                script = "mostrarAlerta('warning', '', '" + mensajePerfilJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "btnBuscar_Click", script, true);
                return;
            }

            if (ddlCategoria.SelectedValue == "0")
            {
                LimpiarListado();

                var mensajeCategoriaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionCategoriaSeleccionVacia);
                script = "mostrarAlerta('warning', '', '" + mensajeCategoriaJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "btnBuscar_Click", script, true);
                return;
            }

            gvListado.PageIndex = 0;

            LoadModulosPerfil();
        }

        private void LoadModulosPerfil()
        {
            var script = string.Empty;

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosModulosPerfilTituloListadoVacio;

            var filtro = new ModuloPerfilFiltroModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                IdPerfil = Convert.ToUInt32(ddlPerfil.SelectedValue),
                IdCategoria = Convert.ToUInt32(ddlCategoria.SelectedValue)
            };

            var respuesta = ModuloPerfilNegocio.GetModulosPerfil(filtro);

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

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadModulosPerfil", script, true);
        }

        private void LimpiarListado()
        {
            gvListado.EmptyDataText = ConstantesModelo.ConsModulosModulosPerfilTituloListadoVacio;
            gvListado.DataSource = null;
            gvListado.DataBind();
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;

            LoadModulosPerfil();
        }

        protected void gvListado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var moduloPerfil = e.Row.DataItem as ModuloPerfilModelo;

            if (moduloPerfil == null)
            {
                return;
            }

            var idModuloPerfil = moduloPerfil.Id.ToString();
            var idModulo = moduloPerfil.IdModulo.ToString();
            var updatedAtOriginal = moduloPerfil.UpdatedAt.HasValue
                ? moduloPerfil.UpdatedAt.Value.ToString("O", CultureInfo.InvariantCulture)
                : string.Empty;

            ConfigurarToggleFila(e.Row, "hfHabilitado", "tglHabilitado", idModuloPerfil, idModulo, "estado", updatedAtOriginal);
            ConfigurarToggleFila(e.Row, "hfConsulta", "tglConsulta", idModuloPerfil, idModulo, "consulta", updatedAtOriginal);
            ConfigurarToggleFila(e.Row, "hfNuevo", "tglNuevo", idModuloPerfil, idModulo, "nuevo", updatedAtOriginal);
            ConfigurarToggleFila(e.Row, "hfModificar", "tglModificar", idModuloPerfil, idModulo, "modifica", updatedAtOriginal);
        }

        private void ConfigurarToggleFila(GridViewRow fila, string hiddenId, string toggleId, string idModuloPerfil, string idModulo, string accion, string updatedAtOriginal)
        {
            var hidden = fila.FindControl(hiddenId) as HiddenField;
            var toggle = fila.FindControl(toggleId) as HtmlGenericControl;

            if (hidden == null || toggle == null)
            {
                return;
            }

            toggle.Attributes["data-hidden-id"] = hidden.ClientID;
            toggle.Attributes["data-mp-id"] = string.IsNullOrWhiteSpace(idModuloPerfil) ? "0" : idModuloPerfil;
            toggle.Attributes["data-mod-id"] = idModulo;
            toggle.Attributes["data-accion"] = accion;
            toggle.Attributes["data-updated-at"] = updatedAtOriginal ?? string.Empty;
            toggle.Attributes["data-valor-encendido"] = "0";
            toggle.Attributes["data-valor-apagado"] = "1";

            if (permisosModulo != null && permisosModulo.PuedeModificar)
            {
                toggle.Attributes["onclick"] = "return cambiarToggleModuloPerfil(this);";
                toggle.Style["cursor"] = "pointer";
                toggle.Style.Remove("opacity");
                toggle.Style.Remove("pointer-events");
                toggle.Attributes.Remove("title");
                return;
            }

            toggle.Attributes["onclick"] = "event.preventDefault(); event.stopPropagation(); return false;";
            toggle.Style["cursor"] = "not-allowed";
            toggle.Style["opacity"] = "0.45";
            toggle.Style["pointer-events"] = "auto";
            toggle.Attributes["title"] = "Sin permiso para modificar";
        }

        protected void gvListado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Bitacora")
            {
                return;
            }

            uint idModuloPerfil;

            if (!uint.TryParse(Convert.ToString(e.CommandArgument), out idModuloPerfil) || idModuloPerfil == 0)
            {
                var mensajeRelacionJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionModuloPerfilRelacionNoRegistrada);
                var scriptRelacion = "mostrarAlerta('warning', '', '" + mensajeRelacionJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "BitacoraModuloPerfilSinRegistro", scriptRelacion, true);
                return;
            }

            ucBitacora.Cargar(idModuloPerfil.ToString());

            var scriptAbrirBitacora = @"(function () {
                var abrir = function () {
                    abrirBitacora();
                };

                if (document.readyState === 'complete') {
                    abrir();
                }
                else {
                    window.addEventListener('load', abrir);
                }
            })();";

            ScriptManager.RegisterStartupScript(this, GetType(), "AbrirBitacoraModuloPerfil", scriptAbrirBitacora, true);
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