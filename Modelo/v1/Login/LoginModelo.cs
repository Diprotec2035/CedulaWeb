namespace Modelo.v1.Login
{
    public class LoginModelo
    {
        public string RutEmpresa { get; set; }
        public string RutUsuario { get; set; }
        public string Password { get; set; }
        public string TokenCaptcha { get; set; }
        public string HttpXForwardedFor { get; set; }
        public string UserHostAddress { get; set; }
    }
}
