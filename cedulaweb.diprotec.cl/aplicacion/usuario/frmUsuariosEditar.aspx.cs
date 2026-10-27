using Modelo;
using Modelo.v1.MFA;
using Negocio;
using Negocio.v1.MFA;
using Negocio.v1.Perfil;
using System;
using System.Web;
using System.Web.UI;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.usuario
{
    public partial class frmUsuariosEditar : Page
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

            if (!ValidarSolicitudMisDatos())
            {
                return;
            }

            if (!EsEdicionMisDatos)
            {
                var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
                var urlModulo = urlVolver.Split('?')[0];

                if (!PermisoModuloHelper.ValidarPuedeModificar(this, urlModulo, urlVolver, ConstantesModelo.ConsErrorPermisoModificarRegistro))
                {
                    return;
                }
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
                litBreadcrumbActual.Text = ModuloContextoHelper.ObtenerTituloPagina(this);

                var rutUsuario = ObtenerRutUsuarioEdicion();

                if (string.IsNullOrWhiteSpace(rutUsuario))
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

                    Response.Redirect(ObtenerUrlRetorno(), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (EsEdicionMisDatos && Convert.ToString(Session["superAdmin"]) == "1")
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsErrorUsuarioSuperAdministradorSinFichaEditable;

                    Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadPerfiles();
                LoadUsuario(rutUsuario);
                ucBitacora.Cargar(rutUsuario);
                AplicarModoEdicion();
            }
        }

        private string ObtenerRutUsuarioEdicion()
        {
            return Convert.ToString(Request.QueryString["id"]);
        }

        private bool EsSolicitudMisDatos
        {
            get
            {
                return string.Equals(Convert.ToString(Request.QueryString["origen"]), "perfil", StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool EsEdicionMisDatos
        {
            get
            {
                if (!EsSolicitudMisDatos)
                {
                    return false;
                }

                var rutUsuarioEdicion = ObtenerRutUsuarioEdicion();
                var rutUsuarioSesion = Convert.ToString(Session["cliente"]);

                if (string.IsNullOrWhiteSpace(rutUsuarioEdicion) || string.IsNullOrWhiteSpace(rutUsuarioSesion))
                {
                    return false;
                }

                return string.Equals(rutUsuarioEdicion.Trim(), rutUsuarioSesion.Trim(), StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool ValidarSolicitudMisDatos()
        {
            if (!EsSolicitudMisDatos || EsEdicionMisDatos)
            {
                return true;
            }

            Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
            Session["AlertaTitulo"] = string.Empty;
            Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

            Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        private string ObtenerUrlRetorno()
        {
            if (EsEdicionMisDatos)
            {
                return ConstantesModelo.ConsModulosDefault;
            }

            return ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
        }

        private void LoadPerfiles()
        {
            var operacion = 0;
            var rutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]);

            var perfiles = PerfilNegocio.GetReportePerfiles(ref operacion, rutEmpresa, string.Empty, 0, 0, 0);

            if (operacion == 0)
            {
                ddlPerfil.DataSource = perfiles;
                ddlPerfil.DataValueField = "Id";
                ddlPerfil.DataTextField = "Nombre";
                ddlPerfil.DataBind();
                return;
            }

            var mensaje = operacion == -2
                ? ConstantesModelo.ConsErrorBaseDatos
                : ConstantesModelo.ConsErrorGenericoConsultar;

            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);
            var script = "mostrarAlerta('danger', '', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadPerfiles", script, true);
        }

        private void LoadUsuario(string rutUsuario)
        {
            var operacion = 0;

            var usuarioFiltro = new EmpresaUsuarioModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                RutUsuario = rutUsuario,
                Estado = null
            };

            var usuario = EmpresaUsuarioNegocio.GetEmpresaUsuario(ref operacion, usuarioFiltro);

            if (operacion != 0 || usuario == null || string.IsNullOrWhiteSpace(usuario.RutUsuario))
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ObtenerUrlRetorno(), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtRut.Text = usuario.RutUsuario;
            txtNombre.Text = usuario.NombreUsuario;
            txtApellido.Text = usuario.ApellidoUsuario;
            txtTelefono.Text = usuario.Telefono;
            txtEmail.Text = usuario.Email;

            if (ddlPerfil.Items.FindByValue(usuario.IdPerfil.ToString()) != null)
            {
                ddlPerfil.SelectedValue = usuario.IdPerfil.ToString();
            }

            hfEstado.Value = usuario.Estado.ToString();

            tglEstado.Attributes["class"] = usuario.Estado == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            UpdatedAtOriginal = usuario.UpdatedAt;
            ConfigurarSeguridadMFA(usuario);
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (!ValidarSolicitudMisDatos())
            {
                return;
            }

            if (!EsEdicionMisDatos)
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

                    ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarUsuario", scriptSinPermiso, true);
                    return;
                }
            }

            var rutUsuario = ObtenerRutUsuarioEdicion();

            if (string.IsNullOrWhiteSpace(rutUsuario))
            {
                var mensajeRutInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);

                var scriptRutInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeRutInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "RutUsuarioInvalido", scriptRutInvalido, true);
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
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajePerfilInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "PerfilUsuarioInvalido", scriptPerfilInvalido, true);
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

                ScriptManager.RegisterStartupScript(this, GetType(), "EstadoUsuarioInvalido", scriptEstadoInvalido, true);
                return;
            }

            var usuario = new EmpresaUsuarioModelo
            {
                RutUsuario = rutUsuario.Trim(),
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                NombreUsuario = txtNombre.Text.Trim(),
                ApellidoUsuario = txtApellido.Text.Trim(),
                Password = txtPassword.Text,
                PasswordConfirmar = txtPasswordConfirmar.Text,
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                IdPerfil = perfilId,
                Estado = estado,
                EsEdicionMisDatos = EsEdicionMisDatos,
                UpdatedAt = UpdatedAtOriginal,
                RutUsuarioBitacora = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = EmpresaUsuarioNegocio.ActualizarUsuario(usuario);

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            var mensajeAlerta = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";

            if (respuesta.Exitoso)
            {
                ActualizarSesionUsuarioEditado(usuario);

                var urlRetorno = usuario.EsEdicionMisDatos
                    ? ConstantesModelo.ConsModulosDefault
                    : ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

                var urlRetornoJs = HttpUtility.JavaScriptStringEncode(urlRetorno);

                mensajeAlerta += "setTimeout(function(){ window.location = '" + urlRetornoJs + "'; }, 1200);";
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
            if (!ValidarSolicitudMisDatos())
            {
                return;
            }

            if (!EsEdicionMisDatos || Convert.ToString(Session["superAdmin"]) == "1")
            {
                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            var codigo = txtCodigoMFA.Text.Trim();

            if (!EsCodigoMFAValido(codigo))
            {
                txtCodigoMFA.Text = string.Empty;
                MostrarResultadoRegeneracion(false, ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido, string.Empty, string.Empty);
                return;
            }

            var rutUsuario = ObtenerRutUsuarioEdicion();
            var operacion = 0;

            var usuario = EmpresaUsuarioNegocio.GetEmpresaUsuario(ref operacion, new EmpresaUsuarioModelo
            {
                RutEmpresa = Convert.ToString(Session["empresa"]),
                RutUsuario = rutUsuario,
                Estado = null
            });

            if (operacion != 0 || usuario == null || usuario.Id == 0 || usuario.EstadoEmpresa != 0 || usuario.Estado != 0 || usuario.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
            {
                txtCodigoMFA.Text = string.Empty;
                MostrarResultadoRegeneracion(false, ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar, string.Empty, string.Empty);
                return;
            }

            var respuesta = MFATotpNegocio.RegenerarCodigosRecuperacion(usuario.Id, string.Empty, codigo, Convert.ToString(Session["cliente"]));

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

            ucBitacora.Cargar(rutUsuario);

            MostrarResultadoRegeneracion(true, respuesta.Mensaje, respuesta.AlertaColor, respuesta.AlertaTitulo);
        }

        private void ConfigurarSeguridadMFA(EmpresaUsuarioModelo usuario)
        {
            pnlSeguridadMFA.Visible = false;
            pnlRegenerarCodigos.Visible = false;
            pnlCodigosRecuperacion.Visible = false;

            if (!EsEdicionMisDatos || usuario == null || usuario.Id == 0 || usuario.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
            {
                return;
            }

            var respuesta = MFATotpNegocio.GetConfiguracion(usuario.Id, string.Empty);

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

        private void AplicarModoEdicion()
        {
            if (!EsEdicionMisDatos)
            {
                return;
            }

            txtRut.Enabled = false;
            txtNombre.Enabled = false;
            txtApellido.Enabled = false;
            ddlPerfil.Enabled = false;

            pnlEstado.Visible = false;
            btnVolver.Visible = false;

            phBreadcrumbUsuarios.Visible = false;
            litBreadcrumbActual.Text = "Mis datos";
        }

        private void ActualizarSesionUsuarioEditado(EmpresaUsuarioModelo usuario)
        {
            var rutUsuarioSesion = Convert.ToString(Session["cliente"]);

            if (!string.Equals(rutUsuarioSesion, usuario.RutUsuario, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var nombreCompleto = (usuario.NombreUsuario + " " + usuario.ApellidoUsuario).Trim();

            if (!string.IsNullOrWhiteSpace(nombreCompleto))
            {
                Session["clienteNombre"] = nombreCompleto;
            }

            if (ddlPerfil.SelectedItem != null && !string.IsNullOrWhiteSpace(ddlPerfil.SelectedItem.Text))
            {
                Session["perfil"] = ddlPerfil.SelectedItem.Text;
            }
        }
    }
}