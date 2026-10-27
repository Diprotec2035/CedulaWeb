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
    public partial class frmReglasNueva : Page
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

            var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModulo = urlVolver.Split('?')[0];

            if (!PermisoModuloHelper.ValidarPuedeNuevo(this, urlModulo, urlVolver, ConstantesModelo.ConsErrorPermisoRegistrarRegistro))
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

            if (!IsPostBack)
            {
                InicializarIndicadores();
            }
        }

        private void ConfigurarToggleIndicador(HtmlGenericControl toggle, HiddenField hidden)
        {
            toggle.Attributes["data-hidden-id"] = hidden.ClientID;
            toggle.Attributes["data-on-value"] = "1";
            toggle.Attributes["data-off-value"] = "0";
        }

        private void InicializarIndicadores()
        {
            EstablecerToggleIndicador(tglNombreApellido, hfNombreApellido, 0);
            EstablecerToggleIndicador(tglEmpresa, hfEmpresa, 0);
            EstablecerToggleIndicador(tglPatente, hfPatente, 0);
            EstablecerToggleIndicador(tglComentario, hfComentario, 0);
            EstablecerToggleIndicador(tglFotografia, hfFotografia, 0);
            EstablecerToggleIndicador(tglEntradaSalida, hfEntradaSalida, 0);
            EstablecerToggleIndicador(tglEliminaEnviados, hfEliminaEnviados, 0);
            ddlListaBlancaNegra.SelectedValue = "0";
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

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            var urlVolverPermiso = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModuloPermiso = urlVolverPermiso.Split('?')[0];

            if (!PermisoModuloHelper.TienePermisoNuevo(this, urlModuloPermiso))
            {
                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarRegla", scriptSinPermiso, true);
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
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeEstadoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "EliminaEnviadosInvalido", scriptEliminaEnviadosInvalido, true);
                return;
            }

            var regla = new ReglaModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Nombre = txtNombre.Text.Trim(),
                NombreApellido = nombreApellido,
                Empresa = empresa,
                Patente = patente,
                Comentario = comentario,
                Fotografia = fotografia,
                EntradaSalida = entradaSalida,
                ListaBlancaNegra = listaBlancaNegra,
                EliminaEnviados = eliminaEnviados,
                Estado = 0,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = ReglaNegocio.RegistrarRegla(regla);

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
                    "$('#" + btnRegistrar.ClientID + "').val('Registrado').prop('disabled', true);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', true);";
            }
            else
            {
                script +=
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);";
            }

            script += mensajeAlerta;

            ScriptManager.RegisterStartupScript(this, GetType(), "btnRegistrar_Click", script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
