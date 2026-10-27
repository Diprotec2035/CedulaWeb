using Modelo;
using Modelo.v1.SuperAdmin;
using Negocio;
using Negocio.v1.SuperAdmin;
using System;
using System.Web;
using System.Web.UI;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.super_admin
{
    public partial class frmSuperAdministradoresNuevo : Page
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
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                var mensajeSinPermisoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeSinPermisoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "btnRegistrar_Click", scriptSinPermiso, true);
                return;
            }

            var superAdmin = new SuperAdminModelo
            {
                RutAdmin = txtRut.Text.Trim(),
                NombreAdmin = txtNombre.Text.Trim(),
                ApellidoAdmin = txtApellido.Text.Trim(),
                Password = txtPassword.Text,
                PasswordConfirmar = txtPasswordConfirmar.Text,
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Estado = 0,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = SuperAdminNegocio.RegistrarSuperAdmin(superAdmin);

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