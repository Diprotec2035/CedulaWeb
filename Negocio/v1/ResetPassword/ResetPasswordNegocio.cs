using Datos.Comunes;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.ResetPassword;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using static Negocio.Comunes.ActivationCodeNegocio;

namespace Negocio.v1.ResetPassword
{
    public class ResetPasswordNegocio
    {
        public static async Task<RespuestaModelo<EmpresaUsuarioModelo>> ResetearPasswordAsync(ResetPasswordModelo reset)
        {
            var respuesta = new RespuestaModelo<EmpresaUsuarioModelo>();

            if (string.IsNullOrWhiteSpace(reset.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaRutVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(reset.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioRutVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(reset.EmailUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioEmailVacio;
                return respuesta;
            }

            if (FuncionesNegocio.ValidateRut(reset.RutEmpresa) == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutEmpresa;
                return respuesta;
            }

            if (FuncionesNegocio.ValidateRut(reset.RutUsuario) == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutUsuario;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(reset.TokenCaptcha))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorCaptcha;
                return respuesta;
            }

            try
            {
                var secretKey_v3 = ConstantesModelo.ReCaptchaSecretKey_v3;
                var ipAddress = FuncionesNegocio.GetIpAddress(reset.HttpXForwardedFor, reset.UserHostAddress);

                using (var client = new HttpClient())
                {
                    var url = string.Format(ConstantesModelo.ReCaptchaUrl, secretKey_v3, reset.TokenCaptcha, ipAddress);
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

                    var scoreMinimo = Convert.ToDouble(ConstantesModelo.ReCaptchaScore, CultureInfo.InvariantCulture);

                    if (captchaResponse.score < scoreMinimo)
                    {
                        respuesta.Mensaje = ConstantesModelo.ConsErrorCaptchaScore;
                        return respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(ResetearPasswordAsync), "Validación reCAPTCHA v3 Enterprise - Acción Reset Password");

                respuesta.Mensaje = ConstantesModelo.ConsErrorCaptchaResponse;
                return respuesta;
            }

            var usuario = new EmpresaUsuarioModelo
            {
                RutUsuario = reset.RutUsuario,
                RutEmpresa = reset.RutEmpresa,
                Email = reset.EmailUsuario
            };

            var operacion = -1;
            usuario = EmpresaUsuarioNegocio.GetEmpresaUsuario(ref operacion, usuario);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRecuperar;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRecuperar;
                return respuesta;
            }

            if (!string.Equals(usuario.Email?.Trim(), reset.EmailUsuario.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRecuperar;
                return respuesta;
            }

            if (usuario.EstadoEmpresa != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaEstado;
                return respuesta;
            }

            if (usuario.Estado != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioEstado;
                return respuesta;
            }

            var passwordTemporal = ActivationCodeService.Generate(12, 4);
            usuario.Password = passwordTemporal.Code;

            if (EmpresaUsuarioNegocio.SendUsuarioCredenciales(usuario) != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRecuperarEnvioMail;
                return respuesta;
            }

            if (EmpresaUsuarioNegocio.UpdateUsuarioPassword(usuario) != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = usuario;
            respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
            respuesta.AlertaTitulo = string.Empty;
            respuesta.Mensaje = ConstantesModelo.ConsExitoRecuperarEnvioMail;

            return respuesta;
        }
    }
}