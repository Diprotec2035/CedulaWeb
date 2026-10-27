using cedulaweb.diprotec.cl.aplicacion.helpers;
using Modelo;
using Modelo.v1.Empresa;
using Negocio;
using Negocio.v1.Empresa;
using System;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.empresa
{
    public partial class frmEmpresasNueva : Page
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

            tglExterno.Attributes["data-hidden-id"] = hfExterno.ClientID;

            if (!IsPostBack)
            {
                InicializarToggles();
            }
        }

        private void InicializarToggles()
        {
            hfExterno.Value = "1";
            tglExterno.Attributes["class"] = "main-toggle br-3 text-center";
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                var mensajeSinPermisoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);
                var scriptSinPermiso = "$('#global-loader').hide();" + "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" + "$('#" + btnVolver.ClientID + "').prop('disabled', false);" + "mostrarAlerta('danger', '', '" + mensajeSinPermisoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarEmpresa", scriptSinPermiso, true);
                return;
            }

            if (!sbyte.TryParse(hfExterno.Value, out sbyte externo) || (externo != 0 && externo != 1))
            {
                var mensajeExternoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionEmpresaExternoInvalido);
                var scriptExternoInvalido = "$('#global-loader').hide();" + "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" + "$('#" + btnVolver.ClientID + "').prop('disabled', false);" + "mostrarAlerta('danger', '', '" + mensajeExternoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ExternoEmpresaInvalido", scriptExternoInvalido, true);
                return;
            }

            var empresa = new EmpresaModelo
            {
                Rut = txtRut.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                Comuna = txtComuna.Text.Trim(),
                Ciudad = txtCiudad.Text.Trim(),
                Contacto = txtContacto.Text.Trim(),
                ContactoTelefono = txtContactoTelefono.Text.Trim(),
                ContactoEmail = txtContactoEmail.Text.Trim(),
                DispositivoExterno = externo,
                Estado = 0,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuestaEmpresa = EmpresaNegocio.RegistrarEmpresa(empresa);

            if (!respuestaEmpresa.Exitoso)
            {
                var mensajeEmpresaErrorJs = HttpUtility.JavaScriptStringEncode(respuestaEmpresa.Mensaje);
                var alertaColorEmpresaErrorJs = HttpUtility.JavaScriptStringEncode(respuestaEmpresa.AlertaColor);
                var alertaTituloEmpresaErrorJs = HttpUtility.JavaScriptStringEncode(respuestaEmpresa.AlertaTitulo);
                var scriptEmpresaError = "$('#global-loader').hide();" + "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" + "$('#" + btnVolver.ClientID + "').prop('disabled', false);" + "mostrarAlerta('" + alertaColorEmpresaErrorJs + "', '" + alertaTituloEmpresaErrorJs + "', '" + mensajeEmpresaErrorJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "RegistrarEmpresaError", scriptEmpresaError, true);
                return;
            }

            var empresaToken = new EmpresaTokenModelo
            {
                RutEmpresa = empresa.Rut,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuestaToken = EmpresaTokenNegocio.GenerarEmpresaToken(empresaToken);
            var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlVolverJs = HttpUtility.JavaScriptStringEncode(urlVolver);

            LimpiarFormulario();

            if (respuestaToken == null || !respuestaToken.Exitoso || respuestaToken.Datos == null || string.IsNullOrWhiteSpace(respuestaToken.Datos.Token))
            {
                var mensajeAdvertenciaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsEmpresaTokenAdvertenciaGeneracionInicial);
                var scriptAdvertencia = "$('#global-loader').hide();" + "mostrarAlerta('warning', '', '" + mensajeAdvertenciaJs + "');" + "setTimeout(function(){ window.location = '" + urlVolverJs + "'; }, 3000);";

                ScriptManager.RegisterStartupScript(this, GetType(), "RegistrarEmpresaTokenAdvertencia", scriptAdvertencia, true);
                return;
            }

            var mensajeEmpresaJs = HttpUtility.JavaScriptStringEncode(respuestaEmpresa.Mensaje);
            var alertaColorEmpresaJs = HttpUtility.JavaScriptStringEncode(respuestaEmpresa.AlertaColor);
            var alertaTituloEmpresaJs = HttpUtility.JavaScriptStringEncode(respuestaEmpresa.AlertaTitulo);
            var tokenJs = HttpUtility.JavaScriptStringEncode(respuestaToken.Datos.Token);
            var fechaVencimientoJs = HttpUtility.JavaScriptStringEncode(respuestaToken.Datos.ExpiresAt.ToString("dd-MM-yyyy HH:mm:ss"));
            var scriptExito = "$('#global-loader').hide();" + "mostrarAlerta('" + alertaColorEmpresaJs + "', '" + alertaTituloEmpresaJs + "', '" + mensajeEmpresaJs + "');" + "mostrarModalTokenGenerado('" + tokenJs + "', '" + fechaVencimientoJs + "', '" + urlVolverJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), "RegistrarEmpresaTokenExito", scriptExito, true);
        }

        private void LimpiarFormulario()
        {
            txtRut.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            txtComuna.Text = string.Empty;
            txtCiudad.Text = string.Empty;
            txtContacto.Text = string.Empty;
            txtContactoTelefono.Text = string.Empty;
            txtContactoEmail.Text = string.Empty;

            InicializarToggles();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}