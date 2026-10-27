using Modelo;
using Negocio;
using Negocio.v1.Perfil;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.usuario
{
    public partial class frmUsuariosNuevo : Page
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
                LoadPerfiles();
            }
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

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarUsuario", scriptSinPermiso, true);
                return;
            }

            uint perfilId;

            if (string.IsNullOrWhiteSpace(ddlPerfil.SelectedValue) ||
                !uint.TryParse(ddlPerfil.SelectedValue, out perfilId) ||
                perfilId == 0)
            {
                var mensajePerfilInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionUsuarioPerfilVacio);

                var scriptPerfilInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajePerfilInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "PerfilUsuarioInvalido", scriptPerfilInvalido, true);
                return;
            }

            var usuario = new EmpresaUsuarioModelo
            {
                RutUsuario = txtRut.Text.Trim(),
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                NombreUsuario = txtNombre.Text.Trim(),
                ApellidoUsuario = txtApellido.Text.Trim(),
                Password = txtPassword.Text,
                PasswordConfirmar = txtPasswordConfirmar.Text,
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                IdPerfil = perfilId,
                Estado = 0,
                RutUsuarioBitacora = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = EmpresaUsuarioNegocio.RegistrarUsuario(usuario);

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

        private void LoadPerfiles()
        {
            var operacion = 0;
            var rutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]);

            var perfiles = PerfilNegocio.GetReportePerfiles(ref operacion, rutEmpresa, string.Empty, 0, 0, 0);

            ddlPerfil.Items.Clear();

            if (operacion == 0)
            {
                ddlPerfil.DataSource = perfiles;
                ddlPerfil.DataValueField = "Id";
                ddlPerfil.DataTextField = "Nombre";
                ddlPerfil.DataBind();

                ddlPerfil.Items.Insert(0, new ListItem("Seleccione perfil", "0"));
                ddlPerfil.SelectedValue = "0";

                return;
            }

            ddlPerfil.Items.Insert(0, new ListItem("Seleccione perfil", "0"));
            ddlPerfil.SelectedValue = "0";

            var mensaje = operacion == -2
                ? ConstantesModelo.ConsErrorBaseDatos
                : ConstantesModelo.ConsErrorGenericoConsultar;

            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);

            var script =
                "mostrarAlerta('danger', '', '" + mensajeJs + "');" +
                "$('#" + btnRegistrar.ClientID + "').prop('disabled', true);";

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadPerfiles", script, true);
        }
    }
}