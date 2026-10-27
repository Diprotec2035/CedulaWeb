using Modelo;
using Modelo.v1.DispositivoMarcas;
using Modelo.v1.DispositivoModelos;
using Negocio;
using Negocio.v1.DispositivoMarcas;
using Negocio.v1.DispositivoModelos;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.dispositivo_modelo
{
    public partial class frmDispositivoModelosNuevo : Page
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

            if (!IsPostBack)
            {
                CargarMarcas();
            }
        }

        private void CargarMarcas()
        {
            ddlMarca.Items.Clear();
            ddlMarca.Items.Add(new ListItem("Seleccione", "0"));

            var filtro = new DispositivoMarcasFiltroModelo
            {
                Coincidencia = string.Empty,
                Estado = 0,
                Orden = 1,
                Orientacion = 1
            };

            var respuesta = DispositivoMarcasNegocio.GetReporteDispositivoMarcas(filtro);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var item in respuesta.Datos)
            {
                ddlMarca.Items.Add(new ListItem(item.Nombre, item.Id.ToString()));
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

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarModelo", scriptSinPermiso, true);
                return;
            }

            uint idMarca;

            if (!uint.TryParse(ddlMarca.SelectedValue, out idMarca))
            {
                idMarca = 0;
            }

            var modelo = new DispositivoModelosModelo
            {
                MarcaId = idMarca,
                Nombre = txtNombre.Text != null ? txtNombre.Text.Trim() : string.Empty,
                Estado = 0,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = DispositivoModelosNegocio.RegistrarDispositivoModelo(modelo);

            if (respuesta == null || !respuesta.Exitoso)
            {
                MostrarAlerta(respuesta != null && !string.IsNullOrWhiteSpace(respuesta.AlertaColor) ? respuesta.AlertaColor : ConstantesModelo.ConsAlertaColorDanger, respuesta != null ? respuesta.AlertaTitulo : string.Empty, respuesta != null && !string.IsNullOrWhiteSpace(respuesta.Mensaje) ? respuesta.Mensaje : ConstantesModelo.ConsErrorGenericoRegistrar);
                return;
            }

            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void MostrarAlerta(string color, string titulo, string mensaje)
        {
            var script = string.Format("mostrarAlerta('{0}', '{1}', '{2}');", HttpUtility.JavaScriptStringEncode(color ?? ConstantesModelo.ConsAlertaColorDanger), HttpUtility.JavaScriptStringEncode(titulo ?? string.Empty), HttpUtility.JavaScriptStringEncode(mensaje ?? string.Empty));

            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), script, true);
        }
    }
}