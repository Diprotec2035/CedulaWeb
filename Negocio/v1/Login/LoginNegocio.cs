using Datos.Comunes;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Login;
using Modelo.v1.SuperAdmin;
using Negocio.v1.SuperAdmin;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Negocio.v1.Login
{
    public class LoginNegocio
    {
        public static async Task<RespuestaModelo<EmpresaUsuarioModelo>> ValidarLoginAsync(LoginModelo login)
        {
            var respuesta = new RespuestaModelo<EmpresaUsuarioModelo>();

            if (string.IsNullOrWhiteSpace(login.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaRutVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(login.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioRutVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(login.Password))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPasswordVacio;
                return respuesta;
            }

            if (FuncionesNegocio.ValidateRut(login.RutEmpresa) == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutEmpresa;
                return respuesta;
            }

            if (FuncionesNegocio.ValidateRut(login.RutUsuario) == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutUsuario;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(login.TokenCaptcha))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorCaptcha;
                return respuesta;
            }

            try
            {
                var secretKey_v3 = ConstantesModelo.ReCaptchaSecretKey_v3;
                var ipAddress = FuncionesNegocio.GetIpAddress(login.HttpXForwardedFor, login.UserHostAddress);

                using (var client = new HttpClient())
                {
                    var url = string.Format(ConstantesModelo.ReCaptchaUrl, secretKey_v3, login.TokenCaptcha, ipAddress);
                    var result = await client.GetStringAsync(url);
                    var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponseModelo>(result);

                    if (!captchaResponse.success)
                    {
                        respuesta.Mensaje = ConstantesModelo.ConsErrorCaptchaSuccess;
                        return respuesta;
                    }

                    if (captchaResponse.action != "login")
                    {
                        respuesta.Mensaje = ConstantesModelo.ConsErrorCaptcha;
                        return respuesta;
                    }

                    var scoreCaptcha = Convert.ToDecimal(captchaResponse.score, CultureInfo.InvariantCulture);
                    var scoreMinimo = Convert.ToDecimal(ConstantesModelo.ReCaptchaScore, CultureInfo.InvariantCulture);

                    if (scoreCaptcha < scoreMinimo)
                    {
                        respuesta.Mensaje = ConstantesModelo.ConsErrorCaptchaScore;
                        return respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(ValidarLoginAsync), "Validación reCAPTCHA v3 Enterprise - Acción Login");

                respuesta.Mensaje = ConstantesModelo.ConsErrorCaptchaResponse;
                return respuesta;
            }

            var usuario = new EmpresaUsuarioModelo
            {
                RutEmpresa = login.RutEmpresa,
                RutUsuario = login.RutUsuario,
                Password = login.Password,
                Estado = 0,
                EsSuperAdmin = false
            };

            var operacionEmpresa = 0;

            var empresa = EmpresaNegocio.GetEmpresa(ref operacionEmpresa, new EmpresaModelo
            {
                Rut = login.RutEmpresa,
                Estado = null
            });

            if (operacionEmpresa != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorLogin;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorLogin;
                return respuesta;
            }

            if (empresa.Estado != 0)
            {
                respuesta.CodigoAuxiliar = 1;
                respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaEstado;
                return respuesta;
            }

            var superAdminLogin = new SuperAdminModelo
            {
                RutAdmin = login.RutUsuario,
                Password = login.Password
            };

            var respuestaSuperAdmin = SuperAdminNegocio.ValidarSuperAdminLogin(superAdminLogin);

            if (respuestaSuperAdmin.Exitoso)
            {
                usuario.NombreEmpresa = empresa.Nombre;
                usuario.EstadoEmpresa = Convert.ToSByte(empresa.Estado);
                usuario.RutEmpresa = empresa.Rut;
                usuario.RutUsuario = respuestaSuperAdmin.Datos.RutAdmin;
                usuario.NombreUsuario = respuestaSuperAdmin.Datos.NombreAdmin;
                usuario.ApellidoUsuario = respuestaSuperAdmin.Datos.ApellidoAdmin;
                usuario.Email = respuestaSuperAdmin.Datos.Email;
                usuario.Telefono = respuestaSuperAdmin.Datos.Telefono;
                usuario.NombrePerfil = ConstantesModelo.ConsAdministradorPerfil;
                usuario.IdPerfil = 0;
                usuario.Estado = respuestaSuperAdmin.Datos.Estado;
                usuario.EstadoNombre = respuestaSuperAdmin.Datos.EstadoNombre;
                usuario.EsSuperAdmin = true;
                usuario.MFARequerido = respuestaSuperAdmin.Datos.MFARequerido;

                respuesta.Exitoso = true;
                respuesta.Datos = usuario;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                respuesta.AlertaTitulo = string.Empty;

                return respuesta;
            }

            var operacion = -1;
            usuario = EmpresaUsuarioNegocio.GetEmpresaUsuario(ref operacion, usuario);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorLogin;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorLogin;
                return respuesta;
            }

            if (usuario.EstadoEmpresa != 0)
            {
                respuesta.CodigoAuxiliar = 1;
                respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaEstado;
                return respuesta;
            }

            if (usuario.Estado != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioEstado;
                return respuesta;
            }

            if (!PasswordHelperNegocio.VerificarPassword(login.Password, usuario.PasswordHash, usuario.PasswordSalt, usuario.PasswordAlgoritmo))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioPassword;
                return respuesta;
            }

            usuario.EsSuperAdmin = false;

            respuesta.Exitoso = true;
            respuesta.Datos = usuario;
            respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
            respuesta.AlertaTitulo = string.Empty;

            return respuesta;
        }
    }
}