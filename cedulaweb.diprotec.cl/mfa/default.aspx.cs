using Modelo;
using Modelo.Comunes;
using Modelo.v1.Log;
using Modelo.v1.MFA;
using Modelo.v1.SuperAdmin;
using Negocio;
using Negocio.v1.Log;
using Negocio.v1.MFA;
using Negocio.v1.SuperAdmin;
using QRCoder;
using System;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.mfa
{
    public partial class _default : Page
    {
        protected int SegundosRestantesLoginPendiente { get; private set; }
        protected bool MostrarTemporizadorLoginPendiente { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            ConfigurarControles();
            ConfigurarTemporizadorLoginPendiente();

            if (IsPostBack)
            {
                return;
            }

            CargarPagina();
        }

        protected void btnValidar_Click(object sender, EventArgs e)
        {
            var loginPendiente = ObtenerLoginPendienteValido();

            if (loginPendiente == null)
            {
                RedirigirLogin();
                return;
            }

            if (!ObtenerUsuarioActual(loginPendiente, out EmpresaUsuarioModelo usuario))
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpIdentidadNoVigente);
                return;
            }

            var codigo = txtCodigo.Text.Trim();

            if (!EsCodigoValido(codigo))
            {
                MostrarAlerta(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido);
                return;
            }

            var respuestaConfiguracion = MFATotpNegocio.GetConfiguracion(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin);

            if (!respuestaConfiguracion.Exitoso)
            {
                MostrarAlerta(respuestaConfiguracion.AlertaColor, respuestaConfiguracion.AlertaTitulo, respuestaConfiguracion.Mensaje);
                return;
            }

            if (respuestaConfiguracion.Datos == null || respuestaConfiguracion.Datos.Id == 0)
            {
                MostrarErrorPagina(ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada);
                return;
            }

            RespuestaModelo<MFATotpModelo> respuesta;

            if (respuestaConfiguracion.Datos.Configurado == ConstantesModelo.ConsMfaTotpConfiguracionPendiente)
            {
                respuesta = MFATotpNegocio.ConfirmarConfiguracion(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin, codigo);

                if (!respuesta.Exitoso)
                {
                    txtCodigo.Text = string.Empty;
                    MostrarAlerta(respuesta.AlertaColor, respuesta.AlertaTitulo, respuesta.Mensaje);
                    return;
                }

                var confirmacion = respuesta.Datos as MFATotpConfirmacionModelo;

                if (confirmacion == null || confirmacion.CodigosRecuperacion == null || confirmacion.CodigosRecuperacion.Count != ConstantesModelo.ConsMfaTotpCodigoRecuperacionCantidad)
                {
                    Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                    MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpCodigoRecuperacionGenerar);
                    return;
                }

                MostrarCodigosRecuperacion(confirmacion);
                return;
            }

            if (respuestaConfiguracion.Datos.Configurado == ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                respuesta = MFATotpNegocio.ValidarAcceso(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin, codigo);
            }
            else
            {
                MostrarErrorPagina(ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada);
                return;
            }

            if (!respuesta.Exitoso)
            {
                txtCodigo.Text = string.Empty;
                MostrarAlerta(respuesta.AlertaColor, respuesta.AlertaTitulo, respuesta.Mensaje);
                return;
            }

            CompletarInicioSesion(loginPendiente);
        }

        protected void btnValidarRecuperacion_Click(object sender, EventArgs e)
        {
            var loginPendiente = ObtenerLoginPendienteValido();

            if (loginPendiente == null)
            {
                RedirigirLogin();
                return;
            }

            if (!ObtenerUsuarioActual(loginPendiente, out EmpresaUsuarioModelo usuario))
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpIdentidadNoVigente);
                return;
            }

            var respuestaConfiguracion = MFATotpNegocio.GetConfiguracion(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin);

            if (!respuestaConfiguracion.Exitoso)
            {
                MostrarAlertaRecuperacion(respuestaConfiguracion.AlertaColor, respuestaConfiguracion.AlertaTitulo, respuestaConfiguracion.Mensaje);
                return;
            }

            if (respuestaConfiguracion.Datos == null || respuestaConfiguracion.Datos.Id == 0 || respuestaConfiguracion.Datos.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente || respuestaConfiguracion.Datos.Configurado != ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                MostrarErrorPagina(ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoConfirmada);
                return;
            }

            var respuesta = MFATotpNegocio.ValidarCodigoRecuperacion(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin, txtCodigoRecuperacion.Text);

            if (!respuesta.Exitoso)
            {
                txtCodigoRecuperacion.Text = string.Empty;
                MostrarAlertaRecuperacion(respuesta.AlertaColor, respuesta.AlertaTitulo, respuesta.Mensaje);
                return;
            }

            CompletarInicioSesion(loginPendiente);
        }

        protected void btnUsarCodigoRecuperacion_Click(object sender, EventArgs e)
        {
            var loginPendiente = ObtenerLoginPendienteValido();

            if (loginPendiente == null)
            {
                RedirigirLogin();
                return;
            }

            MostrarRecuperacion();
        }

        protected void btnUsarCodigoTotp_Click(object sender, EventArgs e)
        {
            var loginPendiente = ObtenerLoginPendienteValido();

            if (loginPendiente == null)
            {
                RedirigirLogin();
                return;
            }

            MostrarValidacion();
        }

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            var loginPendiente = ObtenerLoginPendienteValido();

            if (loginPendiente == null)
            {
                RedirigirLogin();
                return;
            }

            if (!ObtenerUsuarioActual(loginPendiente, out EmpresaUsuarioModelo usuario))
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpIdentidadNoVigente);
                return;
            }

            var respuestaConfiguracion = MFATotpNegocio.GetConfiguracion(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin);

            if (!respuestaConfiguracion.Exitoso)
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                MostrarErrorPagina(respuestaConfiguracion.Mensaje);
                return;
            }

            if (respuestaConfiguracion.Datos == null || respuestaConfiguracion.Datos.Id == 0 || respuestaConfiguracion.Datos.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente || respuestaConfiguracion.Datos.Configurado != ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                MostrarErrorPagina(ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoConfirmada);
                return;
            }

            CompletarInicioSesion(loginPendiente);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);

            Response.Redirect(ConstantesModelo.ConsHttpInicio, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void CargarPagina()
        {
            var loginPendiente = ObtenerLoginPendienteValido();

            if (loginPendiente == null)
            {
                RedirigirLogin();
                return;
            }

            if (!ObtenerUsuarioActual(loginPendiente, out EmpresaUsuarioModelo usuario))
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpIdentidadNoVigente);
                return;
            }

            var respuestaConfiguracion = MFATotpNegocio.GetConfiguracion(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin);

            if (!respuestaConfiguracion.Exitoso)
            {
                MostrarErrorPagina(respuestaConfiguracion.Mensaje);
                return;
            }

            if (respuestaConfiguracion.Datos == null)
            {
                CargarEnrolamiento(loginPendiente);
                return;
            }

            if (respuestaConfiguracion.Datos.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpConfiguracionNoVigente);
                return;
            }

            if (respuestaConfiguracion.Datos.Configurado == ConstantesModelo.ConsMfaTotpConfiguracionPendiente)
            {
                CargarEnrolamiento(loginPendiente);
                return;
            }

            if (respuestaConfiguracion.Datos.Configurado == ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                MostrarValidacion();
                return;
            }

            MostrarErrorPagina(ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada);
        }

        private void CargarEnrolamiento(MFALoginPendienteModelo loginPendiente)
        {
            var nombreCuenta = loginPendiente.EsSuperAdmin ? loginPendiente.RutSuperAdmin + ConstantesModelo.ConsMfaTotpCuentaSeparador + ConstantesModelo.ConsMfaTotpCuentaTipoSuperAdmin : loginPendiente.RutUsuario + ConstantesModelo.ConsMfaTotpCuentaSeparador + loginPendiente.RutEmpresa + ConstantesModelo.ConsMfaTotpCuentaSeparador + ConstantesModelo.ConsMfaTotpCuentaTipoUsuario;
            var respuesta = MFATotpNegocio.IniciarConfiguracion(loginPendiente.EmpresaUsuarioId, loginPendiente.RutSuperAdmin, nombreCuenta);

            if (!respuesta.Exitoso || respuesta.Datos == null)
            {
                MostrarErrorPagina(respuesta.Mensaje);
                return;
            }

            try
            {
                imgCodigoQR.ImageUrl = GenerarCodigoQrDataUri(respuesta.Datos.UriOtpAuth);
                txtClaveManual.Text = respuesta.Datos.SecretBase32;

                pnlFormulario.Visible = true;
                pnlEnrolamiento.Visible = true;
                pnlEnrolamientoDetalle.Visible = true;
                pnlValidacion.Visible = false;
                pnlCodigoTotp.Visible = true;
                pnlRecuperacion.Visible = false;
                pnlCodigosRecuperacion.Visible = false;
                pnlError.Visible = false;

                btnUsarCodigoRecuperacion.Visible = false;
                btnValidar.Text = "Confirmar configuración";
            }
            catch
            {
                MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpProteccion);
            }
        }

        private void MostrarValidacion()
        {
            pnlFormulario.Visible = true;
            pnlEnrolamiento.Visible = false;
            pnlEnrolamientoDetalle.Visible = false;
            pnlValidacion.Visible = true;
            pnlCodigoTotp.Visible = true;
            pnlRecuperacion.Visible = false;
            pnlCodigosRecuperacion.Visible = false;
            pnlError.Visible = false;

            txtClaveManual.Text = string.Empty;
            txtCodigoRecuperacion.Text = string.Empty;

            btnUsarCodigoRecuperacion.Visible = true;
            btnValidar.Text = "Validar código";
        }

        private void MostrarRecuperacion()
        {
            pnlFormulario.Visible = true;
            pnlEnrolamiento.Visible = false;
            pnlEnrolamientoDetalle.Visible = false;
            pnlValidacion.Visible = false;
            pnlCodigoTotp.Visible = false;
            pnlRecuperacion.Visible = true;
            pnlCodigosRecuperacion.Visible = false;
            pnlError.Visible = false;

            txtCodigo.Text = string.Empty;
            txtCodigoRecuperacion.Text = string.Empty;
        }

        private void MostrarCodigosRecuperacion(MFATotpConfirmacionModelo confirmacion)
        {
            pnlFormulario.Visible = false;
            pnlCodigosRecuperacion.Visible = true;
            pnlError.Visible = false;

            rptCodigosRecuperacion.DataSource = confirmacion.CodigosRecuperacion;
            rptCodigosRecuperacion.DataBind();
        }

        private void MostrarErrorPagina(string mensaje)
        {
            MostrarTemporizadorLoginPendiente = false;

            pnlFormulario.Visible = false;
            pnlEnrolamiento.Visible = false;
            pnlEnrolamientoDetalle.Visible = false;
            pnlValidacion.Visible = false;
            pnlCodigoTotp.Visible = false;
            pnlRecuperacion.Visible = false;
            pnlCodigosRecuperacion.Visible = false;
            pnlError.Visible = true;

            litError.Text = HttpUtility.HtmlEncode(mensaje);
        }

        private void ConfigurarControles()
        {
            txtCodigo.Attributes["pattern"] = ConstantesModelo.ConsMfaTotpCodigoPatron;

            txtCodigoRecuperacion.MaxLength = ConstantesModelo.ConsMfaTotpCodigoRecuperacionLargo + ConstantesModelo.ConsMfaTotpCodigoRecuperacionSeparador.Length;
            txtCodigoRecuperacion.Attributes["pattern"] = ConstantesModelo.ConsMfaTotpCodigoRecuperacionPatron;
        }

        private void ConfigurarTemporizadorLoginPendiente()
        {
            SegundosRestantesLoginPendiente = 0;
            MostrarTemporizadorLoginPendiente = false;

            var loginPendiente = Session[ConstantesModelo.ConsMfaTotpSessionLoginPendiente] as MFALoginPendienteModelo;

            if (loginPendiente == null)
            {
                return;
            }

            var segundosRestantes = (int)Math.Ceiling((loginPendiente.Expira - DateTime.Now).TotalSeconds);

            if (segundosRestantes <= 0)
            {
                return;
            }

            SegundosRestantesLoginPendiente = segundosRestantes;
            MostrarTemporizadorLoginPendiente = true;
        }

        private MFALoginPendienteModelo ObtenerLoginPendienteValido()
        {
            var loginPendiente = Session[ConstantesModelo.ConsMfaTotpSessionLoginPendiente] as MFALoginPendienteModelo;

            if (loginPendiente == null)
            {
                return null;
            }

            if (loginPendiente.Expira <= DateTime.Now)
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                return null;
            }

            var tieneEmpresaUsuario = loginPendiente.EmpresaUsuarioId.HasValue && loginPendiente.EmpresaUsuarioId.Value > 0;
            var tieneSuperAdmin = !string.IsNullOrWhiteSpace(loginPendiente.RutSuperAdmin);

            if (tieneEmpresaUsuario == tieneSuperAdmin)
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                return null;
            }

            if (string.IsNullOrWhiteSpace(loginPendiente.RutEmpresa) || string.IsNullOrWhiteSpace(loginPendiente.RutUsuario))
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                return null;
            }

            return loginPendiente;
        }

        private bool ObtenerUsuarioActual(MFALoginPendienteModelo loginPendiente, out EmpresaUsuarioModelo usuario)
        {
            usuario = null;

            var operacionEmpresa = 0;
            var empresa = EmpresaNegocio.GetEmpresa(ref operacionEmpresa, new EmpresaModelo { Rut = loginPendiente.RutEmpresa, Estado = null });

            if (operacionEmpresa != 0 || empresa == null || string.IsNullOrWhiteSpace(empresa.Rut) || empresa.Estado != 0)
            {
                return false;
            }

            if (loginPendiente.EsSuperAdmin)
            {
                var operacionSuperAdmin = 0;
                var superAdmin = SuperAdminNegocio.GetSuperAdmin(ref operacionSuperAdmin, new SuperAdminModelo { RutAdmin = loginPendiente.RutSuperAdmin });

                if (operacionSuperAdmin != 0 || superAdmin == null || string.IsNullOrWhiteSpace(superAdmin.RutAdmin) || superAdmin.Estado != 0 || superAdmin.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
                {
                    return false;
                }

                usuario = new EmpresaUsuarioModelo
                {
                    RutEmpresa = empresa.Rut,
                    NombreEmpresa = empresa.Nombre,
                    EstadoEmpresa = Convert.ToSByte(empresa.Estado),
                    RutUsuario = superAdmin.RutAdmin,
                    NombreUsuario = superAdmin.NombreAdmin,
                    ApellidoUsuario = superAdmin.ApellidoAdmin,
                    NombrePerfil = ConstantesModelo.ConsAdministradorPerfil,
                    IdPerfil = 0,
                    Estado = superAdmin.Estado,
                    EsSuperAdmin = true,
                    MFARequerido = superAdmin.MFARequerido
                };

                return true;
            }

            var operacionUsuario = 0;
            var empresaUsuario = EmpresaUsuarioNegocio.GetEmpresaUsuario(ref operacionUsuario, new EmpresaUsuarioModelo { RutEmpresa = loginPendiente.RutEmpresa, RutUsuario = loginPendiente.RutUsuario });

            if (operacionUsuario != 0 || empresaUsuario == null || empresaUsuario.Id == 0 || !loginPendiente.EmpresaUsuarioId.HasValue || empresaUsuario.Id != loginPendiente.EmpresaUsuarioId.Value || empresaUsuario.EstadoEmpresa != 0 || empresaUsuario.Estado != 0 || empresaUsuario.MFARequerido != ConstantesModelo.ConsMfaTotpRequerido)
            {
                return false;
            }

            empresaUsuario.EsSuperAdmin = false;
            usuario = empresaUsuario;

            return true;
        }

        private void CompletarInicioSesion(MFALoginPendienteModelo loginPendiente)
        {
            if (!ObtenerUsuarioActual(loginPendiente, out EmpresaUsuarioModelo usuario))
            {
                Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);
                MostrarErrorPagina(ConstantesModelo.ConsErrorMfaTotpIdentidadNoVigente);
                return;
            }

            IniciarSesion(usuario);
            RegistrarAcceso(usuario);

            Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);

            Response.Redirect(ConstantesModelo.ConsHttpAplicacion, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void IniciarSesion(EmpresaUsuarioModelo usuario)
        {
            Session["empresa"] = usuario.RutEmpresa;
            Session["empresaNombre"] = usuario.NombreEmpresa;
            Session["perfil"] = usuario.NombrePerfil;
            Session["cliente"] = usuario.RutUsuario;
            Session["clienteNombre"] = (usuario.NombreUsuario + " " + usuario.ApellidoUsuario).Trim();
            Session["superAdmin"] = usuario.EsSuperAdmin ? "1" : "0";

            var minutosSesion = FuncionesNegocio.GetSessionTimeoutMinutos();

            Session.Timeout = minutosSesion;
            Session["SesionExpira"] = DateTime.Now.AddMinutes(minutosSesion);
        }

        private void RegistrarAcceso(EmpresaUsuarioModelo usuario)
        {
            var acceso = new LogAccesoWebModelo
            {
                Ip = GetIpCliente(),
                RutEmpresa = usuario.RutEmpresa,
                RutUsuario = usuario.RutUsuario
            };

            LogAccesoWebNegocio.RegistrarLogAccesoWeb(acceso);
        }

        private string GetIpCliente()
        {
            var ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrWhiteSpace(ip))
            {
                var direcciones = ip.Split(',');

                foreach (var direccion in direcciones)
                {
                    var candidata = direccion.Trim();

                    if (EsIpValida(candidata))
                    {
                        return NormalizarIp(candidata);
                    }
                }
            }

            ip = Request.ServerVariables["REMOTE_ADDR"];

            if (!string.IsNullOrWhiteSpace(ip) && EsIpValida(ip))
            {
                return NormalizarIp(ip);
            }

            ip = Request.UserHostAddress;

            if (!string.IsNullOrWhiteSpace(ip) && EsIpValida(ip))
            {
                return NormalizarIp(ip);
            }

            return string.Empty;
        }

        private bool EsIpValida(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            return IPAddress.TryParse(ip, out IPAddress direccionIp);
        }

        private string NormalizarIp(string ip)
        {
            if (!IPAddress.TryParse(ip, out IPAddress direccionIp))
            {
                return ip;
            }

            if (direccionIp.ToString() == "::1")
            {
                return "127.0.0.1";
            }

            if (direccionIp.AddressFamily == AddressFamily.InterNetwork)
            {
                return direccionIp.ToString();
            }

            if (direccionIp.AddressFamily == AddressFamily.InterNetworkV6 && direccionIp.IsIPv4MappedToIPv6)
            {
                return direccionIp.MapToIPv4().ToString();
            }

            return direccionIp.ToString();
        }

        private string GenerarCodigoQrDataUri(string contenido)
        {
            using (var generador = new QRCodeGenerator())
            {
                using (var datos = generador.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var codigoQr = new PngByteQRCode(datos))
                    {
                        var imagen = codigoQr.GetGraphic(ConstantesModelo.ConsMfaTotpQrPixelesPorModulo);
                        return ConstantesModelo.ConsMfaTotpQrDataUriPrefijo + Convert.ToBase64String(imagen);
                    }
                }
            }
        }

        private bool EsCodigoValido(string codigo)
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

        private void MostrarAlerta(string color, string titulo, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                color = ConstantesModelo.ConsAlertaColorDanger;
            }

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido;
            }

            var colorJs = HttpUtility.JavaScriptStringEncode(color);
            var tituloJs = HttpUtility.JavaScriptStringEncode(titulo);
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);
            var script = "enviandoMFA=false;" + "$('#" + btnValidar.ClientID + "').css({'pointer-events':'auto','opacity':'1'});" + "$('#" + btnVolver.ClientID + "').css({'pointer-events':'auto','opacity':'1'});" + "inicializarMFA();" + "window.setTimeout(function(){" + "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');" + "$('#" + txtCodigo.ClientID + "').focus();" + "},0);";

            ScriptManager.RegisterStartupScript(this, GetType(), nameof(MostrarAlerta), script, true);
        }

        private void MostrarAlertaRecuperacion(string color, string titulo, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                color = ConstantesModelo.ConsAlertaColorDanger;
            }

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoRecuperacionInvalido;
            }

            MostrarRecuperacion();

            var colorJs = HttpUtility.JavaScriptStringEncode(color);
            var tituloJs = HttpUtility.JavaScriptStringEncode(titulo);
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);
            var script = "enviandoMFA=false;" + "$('#" + btnValidarRecuperacion.ClientID + "').css({'pointer-events':'auto','opacity':'1'});" + "$('#" + btnUsarCodigoTotp.ClientID + "').css({'pointer-events':'auto','opacity':'1'});" + "$('#" + btnVolver.ClientID + "').css({'pointer-events':'auto','opacity':'1'});" + "inicializarCodigoRecuperacion();" + "window.setTimeout(function(){" + "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');" + "$('#" + txtCodigoRecuperacion.ClientID + "').focus();" + "},0);";

            ScriptManager.RegisterStartupScript(this, GetType(), nameof(MostrarAlertaRecuperacion), script, true);
        }

        private void RedirigirLogin()
        {
            Response.Redirect(ConstantesModelo.ConsHttpInicio, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}