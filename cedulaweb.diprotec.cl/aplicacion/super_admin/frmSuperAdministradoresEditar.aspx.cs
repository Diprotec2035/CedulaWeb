using cedulaweb.diprotec.cl.aplicacion.helpers;
using Modelo;
using Modelo.v1.MFA;
using Modelo.v1.SuperAdmin;
using Negocio;
using Negocio.v1.MFA;
using Negocio.v1.SuperAdmin;
using System;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.super_admin
{
    public partial class frmSuperAdministradoresEditar : Page
    {
        private DateTime UpdatedAtOriginal
        {
            get
            {
                return ViewState["UpdatedAtOriginal"] == null ? DateTime.MinValue : (DateTime)ViewState["UpdatedAtOriginal"];
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
                var mensaje = string.IsNullOrWhiteSpace(contextoModulo.Mensaje) ? ConstantesModelo.ConsValidacionModuloContextoError : contextoModulo.Mensaje;

                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = mensaje;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            tglEstado.Attributes["data-hidden-id"] = hfEstado.ClientID;
            txtCodigoMFA.Attributes["pattern"] = ConstantesModelo.ConsMfaTotpCodigoPatron;

            pnlCodigosRecuperacion.Visible = false;

            if (IsPostBack && pnlSeguridadMFA.Visible)
            {
                pnlRegenerarCodigos.Visible = true;
            }

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

                LoadSuperAdmin(rut);
                ucBitacora.Cargar(rut);
            }
        }

        private void LoadSuperAdmin(string rutSuperAdmin)
        {
            var operacion = 0;

            var superAdminFiltro = new SuperAdminModelo
            {
                RutAdmin = rutSuperAdmin,
                Estado = null
            };

            var superAdmin = SuperAdminNegocio.GetSuperAdmin(ref operacion, superAdminFiltro);

            if (operacion != 0 || superAdmin == null || string.IsNullOrWhiteSpace(superAdmin.RutAdmin))
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtRut.Text = superAdmin.RutAdmin;
            txtNombre.Text = superAdmin.NombreAdmin;
            txtApellido.Text = superAdmin.ApellidoAdmin;
            txtTelefono.Text = superAdmin.Telefono;
            txtEmail.Text = superAdmin.Email;
            hfEstado.Value = superAdmin.Estado.ToString();

            tglEstado.Attributes["class"] = superAdmin.Estado == 0 ? "main-toggle br-3 text-center on" : "main-toggle br-3 text-center";

            UpdatedAtOriginal = superAdmin.UpdatedAt;

            ConfigurarSeguridadMFA(superAdmin);
        }

        private void ConfigurarSeguridadMFA(SuperAdminModelo superAdmin)
        {
            pnlSeguridadMFA.Visible = false;
            pnlRegenerarCodigos.Visible = false;
            pnlCodigosRecuperacion.Visible = false;

            if (superAdmin == null || string.IsNullOrWhiteSpace(superAdmin.RutAdmin))
            {
                return;
            }

            var rutSesion = Convert.ToString(Session["cliente"]);

            if (string.IsNullOrWhiteSpace(rutSesion) || !string.Equals(rutSesion.Trim(), superAdmin.RutAdmin.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (superAdmin.Estado != 0 || superAdmin.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
            {
                return;
            }

            var respuesta = MFATotpNegocio.GetConfiguracion(null, superAdmin.RutAdmin);

            if (!respuesta.Exitoso || respuesta.Datos == null || respuesta.Datos.Id == 0)
            {
                return;
            }

            if (respuesta.Datos.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente || respuesta.Datos.Configurado != ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                return;
            }

            pnlSeguridadMFA.Visible = true;
            pnlRegenerarCodigos.Visible = true;
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

                ScriptManager.RegisterStartupScript(this, GetType(), "btnActualizar_Click", scriptSinPermiso, true);
                return;
            }

            var rutSuperAdmin = Convert.ToString(Request.QueryString["id"]);

            if (string.IsNullOrWhiteSpace(rutSuperAdmin))
            {
                var mensajeIdInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);

                var scriptRutInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeIdInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "btnActualizar_Click", scriptRutInvalido, true);
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

                ScriptManager.RegisterStartupScript(this, GetType(), "btnActualizar_Click", scriptEstadoInvalido, true);
                return;
            }

            var superAdmin = new SuperAdminModelo
            {
                RutAdmin = rutSuperAdmin.Trim(),
                NombreAdmin = txtNombre.Text.Trim(),
                ApellidoAdmin = txtApellido.Text.Trim(),
                Password = txtPassword.Text,
                PasswordConfirmar = txtPasswordConfirmar.Text,
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Estado = estado,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]),
                UpdatedAt = UpdatedAtOriginal
            };

            var respuesta = SuperAdminNegocio.ActualizarSuperAdmin(superAdmin);

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            var mensajeAlerta = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";

            if (respuesta.Exitoso)
            {
                ActualizarSesionSuperAdminEditado(superAdmin);

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

        protected void btnRegenerarCodigos_Click(object sender, EventArgs e)
        {
            var rutSuperAdmin = Convert.ToString(Request.QueryString["id"]);
            var rutSesion = Convert.ToString(Session["cliente"]);

            if (string.IsNullOrWhiteSpace(rutSuperAdmin) || string.IsNullOrWhiteSpace(rutSesion) || !string.Equals(rutSuperAdmin.Trim(), rutSesion.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                txtCodigoMFA.Text = string.Empty;
                MostrarResultadoRegeneracion(false, ConstantesModelo.ConsErrorPermisoModificarRegistro, string.Empty, string.Empty);
                return;
            }

            var codigo = txtCodigoMFA.Text.Trim();

            if (!EsCodigoMFAValido(codigo))
            {
                txtCodigoMFA.Text = string.Empty;
                MostrarResultadoRegeneracion(false, ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido, string.Empty, string.Empty);
                return;
            }

            var operacion = 0;

            var superAdmin = SuperAdminNegocio.GetSuperAdmin(ref operacion, new SuperAdminModelo
            {
                RutAdmin = rutSuperAdmin,
                Estado = null
            });

            if (operacion != 0 || superAdmin == null || string.IsNullOrWhiteSpace(superAdmin.RutAdmin) || superAdmin.Estado != 0 || superAdmin.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
            {
                txtCodigoMFA.Text = string.Empty;
                MostrarResultadoRegeneracion(false, ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar, string.Empty, string.Empty);
                return;
            }

            var respuesta = MFATotpNegocio.RegenerarCodigosRecuperacion(null, superAdmin.RutAdmin, codigo, rutSesion);

            if (!respuesta.Exitoso)
            {
                txtCodigoMFA.Text = string.Empty;
                MostrarResultadoRegeneracion(false, respuesta.Mensaje, respuesta.AlertaColor, respuesta.AlertaTitulo);
                return;
            }

            var confirmacion = respuesta.Datos as MFATotpConfirmacionModelo;

            if (confirmacion == null || confirmacion.CodigosRecuperacion == null || confirmacion.CodigosRecuperacion.Count != ConstantesModelo.ConsMfaTotpCodigoRecuperacionCantidad)
            {
                txtCodigoMFA.Text = string.Empty;
                MostrarResultadoRegeneracion(false, ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar, string.Empty, string.Empty);
                return;
            }

            rptCodigosRecuperacion.DataSource = confirmacion.CodigosRecuperacion;
            rptCodigosRecuperacion.DataBind();

            txtCodigoMFA.Text = string.Empty;
            pnlSeguridadMFA.Visible = true;
            pnlRegenerarCodigos.Visible = false;
            pnlCodigosRecuperacion.Visible = true;

            ucBitacora.Cargar(rutSuperAdmin);

            MostrarResultadoRegeneracion(true, respuesta.Mensaje, respuesta.AlertaColor, respuesta.AlertaTitulo);
        }

        private bool EsCodigoMFAValido(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != ConstantesModelo.ConsMfaTotpCodigoLargo)
            {
                return false;
            }

            foreach (var caracter in codigo)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        private void MostrarResultadoRegeneracion(bool exitoso, string mensaje, string alertaColor, string alertaTitulo)
        {
            pnlSeguridadMFA.Visible = true;

            if (!exitoso)
            {
                pnlRegenerarCodigos.Visible = true;
                pnlCodigosRecuperacion.Visible = false;
            }

            var mensajeResultado = string.IsNullOrWhiteSpace(mensaje) ? ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar : mensaje;
            var colorResultado = string.IsNullOrWhiteSpace(alertaColor) ? (exitoso ? ConstantesModelo.ConsAlertaColorSuccess : ConstantesModelo.ConsAlertaColorDanger) : alertaColor;
            var tituloResultado = alertaTitulo ?? string.Empty;

            var mensajeJs = HttpUtility.JavaScriptStringEncode(HttpUtility.HtmlDecode(mensajeResultado));
            var colorJs = HttpUtility.JavaScriptStringEncode(colorResultado);
            var tituloJs = HttpUtility.JavaScriptStringEncode(HttpUtility.HtmlDecode(tituloResultado));
            var script = "$('#global-loader').hide();" + "$('#" + btnRegenerarCodigos.ClientID + "').prop('disabled', false);" + "inicializarSeguridadMFA();" + "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), nameof(MostrarResultadoRegeneracion), script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ActualizarSesionSuperAdminEditado(SuperAdminModelo superAdmin)
        {
            var rutUsuarioSesion = Convert.ToString(Session["cliente"]);

            if (!string.Equals(rutUsuarioSesion, superAdmin.RutAdmin, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var nombreCompleto = (superAdmin.NombreAdmin + " " + superAdmin.ApellidoAdmin).Trim();

            if (!string.IsNullOrWhiteSpace(nombreCompleto))
            {
                Session["clienteNombre"] = nombreCompleto;
            }
        }
    }
}