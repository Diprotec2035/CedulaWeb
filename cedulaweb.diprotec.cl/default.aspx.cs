using Modelo;
using Modelo.v1.Log;
using Modelo.v1.Login;
using Modelo.v1.MFA;
using Negocio;
using Negocio.v1.Log;
using Negocio.v1.Login;
using System;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (Page.IsPostBack)
            {
                return;
            }

            OcultarCuentaSuspendida();

            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
        }

        protected async void btnIngresar_Click(object sender, EventArgs e)
        {
            OcultarCuentaSuspendida();

            var login = new LoginModelo
            {
                RutEmpresa = txtRutEmpresa.Text.Trim(),
                RutUsuario = txtRutUsuario.Text.Trim(),
                Password = txtPassword.Text,
                TokenCaptcha = gRecaptchaResponse.Value,
                HttpXForwardedFor = Request.ServerVariables["HTTP_X_FORWARDED_FOR"],
                UserHostAddress = Request.UserHostAddress
            };

            var respuesta = await LoginNegocio.ValidarLoginAsync(login);

            string script;

            if (respuesta.Exitoso)
            {
                string urlDestino;

                if (respuesta.Datos.MFARequerido == ConstantesModelo.ConsMfaTotpRequerido)
                {
                    GuardarLoginPendienteMFA(respuesta.Datos);
                    urlDestino = ConstantesModelo.ConsHttpMFA;
                }
                else
                {
                    IniciarSesion(respuesta.Datos);
                    RegistrarAcceso(respuesta.Datos);
                    urlDestino = ConstantesModelo.ConsHttpAplicacion;
                }

                var urlDestinoJs = HttpUtility.JavaScriptStringEncode(urlDestino);
                var textoIngresandoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsLoginBotonIngresando);

                script = "$('#" + btnIngresar.ClientID + "').val('" + textoIngresandoJs + "').prop('disabled', true);" + "$('#lnkRecuperar').css({'pointer-events':'none','opacity':'0.5'});" + "window.location = '" + urlDestinoJs + "';";
            }
            else
            {
                var mensajeAlerta = string.Empty;

                if (respuesta.CodigoAuxiliar == 1)
                {
                    MostrarCuentaSuspendida();
                }
                else
                {
                    var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
                    var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
                    var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

                    mensajeAlerta = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";
                }

                var textoIniciarSesionJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsLoginBotonIniciarSesion);

                script = "$('#" + btnIngresar.ClientID + "').val('" + textoIniciarSesionJs + "').prop('disabled', false);" + "$('#lnkRecuperar').css({'pointer-events':'auto','opacity':'1'});" + mensajeAlerta + "inicializarLogin();";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), nameof(btnIngresar_Click), script, true);
        }

        protected void btnRecuperar_Click(object sender, EventArgs e)
        {
            Response.Redirect(ConstantesModelo.ConsHttpRecuperar);
        }

        private void GuardarLoginPendienteMFA(EmpresaUsuarioModelo usuario)
        {
            var loginPendiente = new MFALoginPendienteModelo
            {
                EmpresaUsuarioId = usuario.EsSuperAdmin ? (uint?)null : usuario.Id,
                RutSuperAdmin = usuario.EsSuperAdmin ? usuario.RutUsuario : string.Empty,
                RutEmpresa = usuario.RutEmpresa,
                NombreEmpresa = usuario.NombreEmpresa,
                RutUsuario = usuario.RutUsuario,
                NombreUsuario = usuario.NombreUsuario,
                ApellidoUsuario = usuario.ApellidoUsuario,
                NombrePerfil = usuario.NombrePerfil,
                EsSuperAdmin = usuario.EsSuperAdmin,
                Expira = DateTime.Now.AddMinutes(ConstantesModelo.ConsMfaTotpMinutosLoginPendiente)
            };

            Session.Remove("empresa");
            Session.Remove("empresaNombre");
            Session.Remove("perfil");
            Session.Remove("cliente");
            Session.Remove("clienteNombre");
            Session.Remove("superAdmin");
            Session.Remove("SesionExpira");

            Session[ConstantesModelo.ConsMfaTotpSessionLoginPendiente] = loginPendiente;
        }

        private void IniciarSesion(EmpresaUsuarioModelo usuario)
        {
            Session.Remove(ConstantesModelo.ConsMfaTotpSessionLoginPendiente);

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
                RutUsuario = usuario.RutUsuario,
                RutEmpresa = usuario.RutEmpresa
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

        private void MostrarCuentaSuspendida()
        {
            pnlFormularioLogin.Visible = false;
            pnlCuentaSuspendida.Visible = true;

            litCuentaSuspendida.Text = ConstantesModelo.ConsErrorEmpresaSuspendida;

            if (UpdatePanelLogin != null)
            {
                UpdatePanelLogin.Update();
            }
        }

        private void OcultarCuentaSuspendida()
        {
            pnlFormularioLogin.Visible = true;
            pnlCuentaSuspendida.Visible = false;
            litCuentaSuspendida.Text = string.Empty;
        }
    }
}