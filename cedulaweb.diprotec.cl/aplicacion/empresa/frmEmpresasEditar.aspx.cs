using Modelo;
using Negocio;
using System;
using System.Web;
using System.Web.UI;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.empresa
{
    public partial class frmEmpresasEditar : Page
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

            tglExterno.Attributes["data-hidden-id"] = hfExterno.ClientID;
            tglEstado.Attributes["data-hidden-id"] = hfEstado.ClientID;

            if (!IsPostBack)
            {
                var rut = Convert.ToString(Request.QueryString["id"]);

                if (string.IsNullOrWhiteSpace(rut))
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

                    Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadEmpresa(rut);
                ucBitacora.Cargar(rut);
            }
        }

        private void LoadEmpresa(string rut)
        {
            var operacion = 0;

            var empresaFiltro = new EmpresaModelo
            {
                Rut = rut,
                Estado = null
            };

            var empresa = EmpresaNegocio.GetEmpresa(ref operacion, empresaFiltro);

            if (operacion != 0 || empresa == null || string.IsNullOrWhiteSpace(empresa.Rut))
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtRut.Text = empresa.Rut;
            txtNombre.Text = empresa.Nombre;
            txtDireccion.Text = empresa.Direccion;
            txtCiudad.Text = empresa.Ciudad;
            txtComuna.Text = empresa.Comuna;
            txtContacto.Text = empresa.Contacto;
            txtContactoTelefono.Text = empresa.ContactoTelefono;
            txtContactoEmail.Text = empresa.ContactoEmail;

            hfExterno.Value = empresa.DispositivoExterno.HasValue
                ? empresa.DispositivoExterno.Value.ToString()
                : "1";

            tglExterno.Attributes["class"] = empresa.DispositivoExterno.HasValue && empresa.DispositivoExterno.Value == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            hfEstado.Value = empresa.Estado.HasValue
                ? empresa.Estado.Value.ToString()
                : "1";

            tglEstado.Attributes["class"] = empresa.Estado.HasValue && empresa.Estado.Value == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            UpdatedAtOriginal = empresa.UpdatedAt;
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

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarEmpresa", scriptSinPermiso, true);
                return;
            }

            var rut = Convert.ToString(Request.QueryString["id"]);

            if (string.IsNullOrWhiteSpace(rut))
            {
                var mensajeRutInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);

                var scriptRutInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeRutInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "EmpresaRutInvalido", scriptRutInvalido, true);
                return;
            }

            sbyte externo;

            if (!sbyte.TryParse(hfExterno.Value, out externo) || (externo != 0 && externo != 1))
            {
                var mensajeExternoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionEmpresaExternoInvalido);

                var scriptExternoInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeExternoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ExternoEmpresaInvalido", scriptExternoInvalido, true);
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

                ScriptManager.RegisterStartupScript(this, GetType(), "EstadoEmpresaInvalido", scriptEstadoInvalido, true);
                return;
            }

            var empresa = new EmpresaModelo
            {
                Rut = rut,
                Nombre = txtNombre.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                Ciudad = txtCiudad.Text.Trim(),
                Comuna = txtComuna.Text.Trim(),
                Contacto = txtContacto.Text.Trim(),
                ContactoTelefono = txtContactoTelefono.Text.Trim(),
                ContactoEmail = txtContactoEmail.Text.Trim(),
                DispositivoExterno = externo,
                Estado = estado,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]),
                UpdatedAt = UpdatedAtOriginal
            };

            var respuesta = EmpresaNegocio.ActualizarEmpresa(empresa);

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