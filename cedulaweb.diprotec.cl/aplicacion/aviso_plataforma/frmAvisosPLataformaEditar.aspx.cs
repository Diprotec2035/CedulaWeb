using Modelo;
using Modelo.v1.AvisoPlataforma;
using Negocio;
using Negocio.v1.AvisoPlataforma;
using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.aviso_plataforma
{
    public partial class frmAvisosPlataformaEditar : Page
    {
        private DateTime UpdatedAtOriginal
        {
            get
            {
                return ViewState["UpdatedAtOriginal"] == null
                    ? DateTime.MinValue
                    : (DateTime)ViewState["UpdatedAtOriginal"];
            }
            set
            {
                ViewState["UpdatedAtOriginal"] = value;
            }
        }

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

            tglEstado.Attributes["data-hidden-id"] = hfEstado.ClientID;

            if (!IsPostBack)
            {
                var id = Convert.ToString(Request.QueryString["id"]);
                uint avisoId;

                if (string.IsNullOrWhiteSpace(id) || !uint.TryParse(id, out avisoId))
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

                    Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                hdnId.Value = avisoId.ToString();

                LoadAviso(avisoId);
                ucBitacora.Cargar(avisoId.ToString());
            }
        }

        private void LoadAviso(uint avisoId)
        {
            var operacion = 0;

            var avisoFiltro = new AvisoPlataformaModelo
            {
                Id = avisoId,
                Estado = null
            };

            var aviso = AvisoPlataformaNegocio.GetAvisoPlataforma(ref operacion, avisoFiltro);

            if (operacion != 0 || aviso == null || aviso.Id == 0)
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtTitulo.Text = aviso.Titulo;
            txtMensaje.Text = aviso.Mensaje;
            txtVersion.Text = aviso.Version;
            txtFechaInicio.Text = aviso.FechaInicio;
            txtFechaTermino.Text = aviso.FechaTermino;

            hdnFechaInicio.Value = aviso.FechaInicio;
            hdnFechaTermino.Value = aviso.FechaTermino;

            hfEstado.Value = aviso.Estado.HasValue
                ? aviso.Estado.Value.ToString()
                : "0";

            tglEstado.Attributes["class"] = hfEstado.Value == "0"
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            UpdatedAtOriginal = aviso.UpdatedAt;

            if (!FechaTerminoPermiteActualizar())
            {
                BloquearFormulario();
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                var mensajeSinPermisoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorPermisoModificarRegistro);

                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeSinPermisoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarAviso", scriptSinPermiso, true);
                return;
            }

            var id = Convert.ToString(Request.QueryString["id"]);
            uint avisoId;

            if (string.IsNullOrWhiteSpace(id) || !uint.TryParse(id, out avisoId))
            {
                var mensajeIdInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);

                var scriptIdInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeIdInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "AvisoIdInvalido", scriptIdInvalido, true);
                return;
            }

            if (!FechaTerminoPermiteActualizar())
            {
                var mensajeFechaVencidaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorAvisoPlataformaFechaTerminoVencida);

                var scriptFechaVencida =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('warning', '', '" + mensajeFechaVencidaJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "AvisoFechaVencida", scriptFechaVencida, true);
                return;
            }

            sbyte estado;

            if (!sbyte.TryParse(hfEstado.Value, out estado) || (estado != 0 && estado != 1))
            {
                var mensajeEstadoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroEstadoInvalido);

                var scriptEstadoInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeEstadoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "EstadoAvisoInvalido", scriptEstadoInvalido, true);
                return;
            }

            var aviso = new AvisoPlataformaModelo
            {
                Id = avisoId,
                Titulo = txtTitulo.Text.Trim(),
                Mensaje = txtMensaje.Text.Trim(),
                Version = txtVersion.Text.Trim(),
                FechaInicio = ObtenerValorFechaFormulario(txtFechaInicio.Text, hdnFechaInicio.Value),
                FechaTermino = ObtenerValorFechaFormulario(txtFechaTermino.Text, hdnFechaTermino.Value),
                Estado = estado,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]),
                UpdatedAt = UpdatedAtOriginal
            };

            var respuesta = AvisoPlataformaNegocio.ActualizarAvisoPlataforma(aviso);

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            var mensajeAlerta = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";

            if (respuesta.Exitoso)
            {
                var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
                var urlVolverJs = HttpUtility.JavaScriptStringEncode(urlVolver);

                mensajeAlerta += "setTimeout(function(){ window.location = '" + urlVolverJs + "'; }, 1200);";
            }

            var script = "$('#global-loader').hide();";

            if (respuesta.Exitoso)
            {
                script +=
                    "$('#" + btnActualizar.ClientID + "').val('Actualizado').prop('disabled', true);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', true);";
            }
            else
            {
                script +=
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);";
            }

            script += mensajeAlerta;

            ScriptManager.RegisterStartupScript(this, GetType(), "btnActualizar_Click", script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private bool FechaTerminoPermiteActualizar()
        {
            DateTime fechaTermino;

            if (!DateTime.TryParseExact(hdnFechaTermino.Value.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaTermino))
            {
                return false;
            }

            return fechaTermino.Date >= DateTime.Today;
        }

        private string ObtenerValorFechaFormulario(string valorTexto, string valorHidden)
        {
            if (!string.IsNullOrWhiteSpace(valorTexto))
            {
                return valorTexto.Trim();
            }

            return Convert.ToString(valorHidden).Trim();
        }

        private void BloquearFormulario()
        {
            txtTitulo.Enabled = false;
            txtMensaje.Enabled = false;
            txtVersion.Enabled = false;
            txtFechaInicio.Enabled = false;
            txtFechaTermino.Enabled = false;

            tglEstado.Attributes["class"] = ObtenerClaseToggleBloqueado(tglEstado.Attributes["class"]);
            tglEstado.Attributes["style"] = "pointer-events: none; opacity: 0.6;";

            btnActualizar.Enabled = false;
        }

        private string ObtenerClaseToggleBloqueado(string claseActual)
        {
            if (string.IsNullOrWhiteSpace(claseActual))
            {
                return "main-toggle br-3 text-center disabled";
            }

            if (claseActual.Contains("disabled"))
            {
                return claseActual;
            }

            return claseActual + " disabled";
        }
    }
}