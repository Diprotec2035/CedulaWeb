using Modelo;
using Modelo.v1.Regla;
using Negocio;
using Negocio.v1.Regla;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.regla
{
    public partial class frmReglasEditar : Page
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

            var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModulo = urlVolver.Split('?')[0];

            if (!PermisoModuloHelper.ValidarPuedeModificar(this, urlModulo, urlVolver, ConstantesModelo.ConsErrorPermisoModificarRegistro))
            {
                return;
            }

            ConfigurarToggleIndicador(tglNombreApellido, hfNombreApellido);
            ConfigurarToggleIndicador(tglEmpresa, hfEmpresa);
            ConfigurarToggleIndicador(tglPatente, hfPatente);
            ConfigurarToggleIndicador(tglComentario, hfComentario);
            ConfigurarToggleIndicador(tglFotografia, hfFotografia);
            ConfigurarToggleIndicador(tglEntradaSalida, hfEntradaSalida);
            ConfigurarToggleIndicador(tglEliminaEnviados, hfEliminaEnviados);
            tglEstado.Attributes["data-hidden-id"] = hfEstado.ClientID;

            if (!IsPostBack)
            {
                var id = Convert.ToString(Request.QueryString["id"]);
                uint reglaId;

                if (string.IsNullOrWhiteSpace(id) || !uint.TryParse(id, out reglaId))
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

                    Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadRegla(reglaId);
                ucBitacora.Cargar(reglaId.ToString());
            }
        }

        private void ConfigurarToggleIndicador(HtmlGenericControl toggle, HiddenField hidden)
        {
            toggle.Attributes["data-hidden-id"] = hidden.ClientID;
            toggle.Attributes["data-on-value"] = "1";
            toggle.Attributes["data-off-value"] = "0";
        }

        private static void EstablecerToggleIndicador(HtmlGenericControl toggle, HiddenField hidden, sbyte valor)
        {
            hidden.Value = valor.ToString();
            toggle.Attributes["class"] = valor == 1
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";
        }

        private static bool TryObtenerBinario(string valor, out sbyte resultado)
        {
            return sbyte.TryParse(valor, out resultado) && (resultado == 0 || resultado == 1);
        }

        private void LoadRegla(uint reglaId)
        {
            var operacion = 0;

            var reglaFiltro = new ReglaModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Id = reglaId,
                Estado = null
            };

            var regla = ReglaNegocio.GetRegla(ref operacion, reglaFiltro);

            if (operacion != 0 || regla == null || regla.Id == 0)
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtNombre.Text = regla.Nombre;

            EstablecerToggleIndicador(tglNombreApellido, hfNombreApellido, regla.NombreApellido);
            EstablecerToggleIndicador(tglEmpresa, hfEmpresa, regla.Empresa);
            EstablecerToggleIndicador(tglPatente, hfPatente, regla.Patente);
            EstablecerToggleIndicador(tglComentario, hfComentario, regla.Comentario);
            EstablecerToggleIndicador(tglFotografia, hfFotografia, regla.Fotografia);
            EstablecerToggleIndicador(tglEntradaSalida, hfEntradaSalida, regla.EntradaSalida);
            EstablecerToggleIndicador(tglEliminaEnviados, hfEliminaEnviados, regla.EliminaEnviados);
            ddlListaBlancaNegra.SelectedValue = regla.ListaBlancaNegra.ToString();

            hfEstado.Value = regla.Estado.ToString();

            tglEstado.Attributes["class"] = regla.Estado == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            UpdatedAtOriginal = regla.UpdatedAt;
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            var urlVolverPermiso = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModuloPermiso = urlVolverPermiso.Split('?')[0];

            if (!PermisoModuloHelper.TienePermisoModificar(this, urlModuloPermiso))
            {
                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoModificarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarRegla", scriptSinPermiso, true);
                return;
            }

            var id = Convert.ToString(Request.QueryString["id"]);
            uint reglaId;

            if (string.IsNullOrWhiteSpace(id) || !uint.TryParse(id, out reglaId))
            {
                var mensajeIdInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);

                var scriptIdInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeIdInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ReglaIdInvalido", scriptIdInvalido, true);
                return;
            }

            sbyte nombreApellido;
            sbyte empresa;
            sbyte patente;
            sbyte comentario;
            sbyte fotografia;
            sbyte entradaSalida;
            sbyte eliminaEnviados;
            sbyte listaBlancaNegra;

            if (!TryObtenerBinario(hfNombreApellido.Value, out nombreApellido) ||
                !TryObtenerBinario(hfEmpresa.Value, out empresa) ||
                !TryObtenerBinario(hfPatente.Value, out patente) ||
                !TryObtenerBinario(hfComentario.Value, out comentario) ||
                !TryObtenerBinario(hfFotografia.Value, out fotografia) ||
                !TryObtenerBinario(hfEntradaSalida.Value, out entradaSalida) ||
                !TryObtenerBinario(hfEliminaEnviados.Value, out eliminaEnviados) ||
                !sbyte.TryParse(ddlListaBlancaNegra.SelectedValue, out listaBlancaNegra) ||
                listaBlancaNegra < 0 || listaBlancaNegra > 2)
            {
                var mensajeEstadoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroEstadoInvalido);

                var scriptEliminaEnviadosInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeEstadoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "EliminaEnviadosInvalido", scriptEliminaEnviadosInvalido, true);
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

                ScriptManager.RegisterStartupScript(this, GetType(), "EstadoReglaInvalido", scriptEstadoInvalido, true);
                return;
            }

            var regla = new ReglaModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Id = reglaId,
                Nombre = txtNombre.Text.Trim(),
                NombreApellido = nombreApellido,
                Empresa = empresa,
                Patente = patente,
                Comentario = comentario,
                Fotografia = fotografia,
                EntradaSalida = entradaSalida,
                ListaBlancaNegra = listaBlancaNegra,
                EliminaEnviados = eliminaEnviados,
                Estado = estado,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]),
                UpdatedAt = UpdatedAtOriginal
            };

            var respuesta = ReglaNegocio.ActualizarRegla(regla);

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
    }
}
