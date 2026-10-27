using Modelo;
using Modelo.v1.Perfil;
using Negocio;
using Negocio.v1.Perfil;
using Negocio.v1.Regla;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.perfil
{
    public partial class frmPerfilesNuevo : Page
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

            if (!IsPostBack)
            {
                LoadReglas();
            }
        }

        private void LoadReglas()
        {
            var operacion = 0;
            var rutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]);

            var reglas = ReglaNegocio.GetReporteReglas(ref operacion, rutEmpresa, string.Empty, 0, 0, 0);

            ddlRegla.Items.Clear();

            if (operacion == 0)
            {
                ddlRegla.DataSource = reglas;
                ddlRegla.DataValueField = "Id";
                ddlRegla.DataTextField = "Nombre";
                ddlRegla.DataBind();

                ddlRegla.Items.Insert(0, new ListItem("Seleccione", "0"));
                ddlRegla.SelectedValue = "0";

                return;
            }

            ddlRegla.Items.Insert(0, new ListItem("Seleccione", "0"));
            ddlRegla.SelectedValue = "0";

            var mensaje = operacion == -2
                ? ConstantesModelo.ConsErrorBaseDatos
                : ConstantesModelo.ConsErrorGenericoConsultar;

            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);

            var script =
                "mostrarAlerta('danger', '', '" + mensajeJs + "');" +
                "$('#" + btnRegistrar.ClientID + "').prop('disabled', true);";

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadReglas", script, true);
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

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarPerfil", scriptSinPermiso, true);
                return;
            }

            uint reglaId;

            if (string.IsNullOrWhiteSpace(ddlRegla.SelectedValue) ||
                !uint.TryParse(ddlRegla.SelectedValue, out reglaId) ||
                reglaId == 0)
            {
                var mensajeReglaInvalidaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionPerfilReglaVacia);

                var scriptReglaInvalida =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeReglaInvalidaJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ReglaPerfilInvalida", scriptReglaInvalida, true);
                return;
            }

            var perfil = new PerfilModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Nombre = txtNombre.Text.Trim(),
                IdRegla = reglaId,
                Estado = 0,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = PerfilNegocio.RegistrarPerfil(perfil);

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