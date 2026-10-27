using Modelo;
using NLog;
using System;
using System.Net.Mail;
using System.Web;

namespace Datos.Comunes
{
    public class FuncionesDatos
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static string UsuarioSesion()
        {
            return HttpContext.Current?.Session?["clienteNombre"]?.ToString() ?? string.Empty;
        }

        public static string TextoSql(object valor)
        {
            if (valor == null || valor == DBNull.Value)
            {
                return "NULL";
            }

            return "'" + valor.ToString().Replace("'", "''") + "'";
        }

        public static int SendEmail(string para, string asunto, string body)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    using (var smtpServer = new SmtpClient(ConstantesModelo.ConsEmailSmtp))
                    {
                        mail.From = new MailAddress(ConstantesModelo.ConsEmailEnvio);
                        mail.To.Add(para);
                        mail.Subject = asunto;
                        mail.IsBodyHtml = true;
                        mail.Body = body;

                        smtpServer.Port = Convert.ToInt32(ConstantesModelo.ConsEmailPort);
                        smtpServer.Credentials = new System.Net.NetworkCredential(
                            ConstantesModelo.ConsEmailUsuario,
                            ConstantesModelo.ConsEmailPassword
                        );
                        smtpServer.EnableSsl = true;

                        smtpServer.Send(mail);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error en '{0}': '{1}'",
                    nameof(SendEmail),
                    ex.Message
                );

                return -1;
            }
        }

        public static void SendErrorSistema(Exception ex, string metodo, string consulta, string usuario)
        {
            try
            {
                var host = string.Empty;
                var url = string.Empty;

                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    host = HttpContext.Current.Request.Url.Authority;
                    url = HttpContext.Current.Request.Url.ToString();
                }

                var body = ConstantesModelo.ConsCuerpoError
                    .Replace("##host##", HttpUtility.HtmlEncode(host))
                    .Replace("##url##", HttpUtility.HtmlEncode(url))
                    .Replace("##metodo##", HttpUtility.HtmlEncode(metodo))
                    .Replace("##consulta##", HttpUtility.HtmlEncode(consulta))
                    .Replace("##error##", HttpUtility.HtmlEncode(ex.ToString()))
                    .Replace("##usuario##", HttpUtility.HtmlEncode(usuario))
                    .Replace("##fecha##", DateTime.Now.ToString("dd-MM-yyyy"))
                    .Replace("##hora##", DateTime.Now.ToString("HH:mm:ss"));

                var asunto = ConstantesModelo.ConsTituloWebPage + " | Error Website";

                SendEmail(ConstantesModelo.ConsEmailDestino, asunto, body);
            }
            catch
            {
                // No lanzar excepción desde el correo de error.
            }
        }

        public static void RegistrarErrorDatos(Exception ex, string metodo, string consulta)
        {
            Logger.Error(ex, "Error en '{0}': '{1}'", metodo, ex.Message);

            SendErrorSistema(ex, metodo, consulta, UsuarioSesion());
        }
    }
}
