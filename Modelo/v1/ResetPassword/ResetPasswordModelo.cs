namespace Modelo.v1.ResetPassword
{
    public class ResetPasswordModelo
    {
        public string RutEmpresa { get; set; }
        public string RutUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public string TokenCaptcha { get; set; }
        public string HttpXForwardedFor { get; set; }
        public string UserHostAddress { get; set; }
    }
}
