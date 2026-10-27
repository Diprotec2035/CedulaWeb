using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Negocio
{
    public static class FuncionesNegocio
    {
        public static string ValidateRut(string rut)
        {
            rut = rut.Replace("-", "");
            var run = rut.Substring(0, rut.Length - 1);
            var dv = rut.Substring(rut.Length - 1, 1);

            var suma = 0;
            var multiplicador = 2;

            for (var i = run.Length - 1; i >= 0; i--)
            {
                var digito = Convert.ToInt16(run.Substring(i, 1));
                suma += digito * multiplicador;
                if (multiplicador == 7)
                    multiplicador = 2;
                else
                    multiplicador++;
            }

            string dvRut;
            var resto = suma % 11;
            switch (resto)
            {
                case 1:
                    dvRut = "k";
                    break;
                case 0:
                    dvRut = "0";
                    break;
                default:
                    var dvi = 11 - resto;
                    dvRut = dvi + "";
                    break;
            }

            return dvRut == dv.ToLower() ? (run + "-" + dv).ToUpper() : "0";
        }

        public static bool TryNormalizarRut(string rut, out string rutNormalizado)
        {
            rutNormalizado = string.Empty;

            if (string.IsNullOrWhiteSpace(rut))
            {
                return false;
            }

            var limpio = new StringBuilder(rut.Length);

            foreach (var caracter in rut)
            {
                if (char.IsLetterOrDigit(caracter))
                {
                    limpio.Append(caracter);
                }
            }

            var texto = limpio.ToString().ToUpperInvariant();

            if (texto.Length < 2)
            {
                return false;
            }

            var dv = texto[texto.Length - 1];
            var cuerpo = texto.Substring(0, texto.Length - 1);

            foreach (var caracter in cuerpo)
            {
                if (caracter < '0' || caracter > '9')
                {
                    return false;
                }
            }

            var suma = 0;
            var multiplicador = 2;

            for (var i = cuerpo.Length - 1; i >= 0; i--)
            {
                suma += (cuerpo[i] - '0') * multiplicador;
                multiplicador++;

                if (multiplicador > 7)
                {
                    multiplicador = 2;
                }
            }

            var resto = 11 - (suma % 11);
            char dvCalculado;

            if (resto == 11)
            {
                dvCalculado = '0';
            }
            else if (resto == 10)
            {
                dvCalculado = 'K';
            }
            else
            {
                dvCalculado = (char)('0' + resto);
            }

            if (dv != dvCalculado)
            {
                return false;
            }

            long numero;

            if (!long.TryParse(cuerpo, out numero) || numero <= 0)
            {
                return false;
            }

            rutNormalizado = numero + "-" + dv;

            return rutNormalizado.Length <= 10;
        }

        public static bool ValidateEmail(string email)
        {
            const string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

            return Regex.IsMatch(email, pattern);
        }

        public static bool TryObtenerGtinCanonico(string codigo, out string codigoCanonico)
        {
            codigoCanonico = null;

            if (string.IsNullOrWhiteSpace(codigo))
            {
                return false;
            }

            codigo = codigo.Trim();

            if (
                codigo.Length != 8 &&
                codigo.Length != 12 &&
                codigo.Length != 13 &&
                codigo.Length != 14
            )
            {
                return false;
            }

            for (var i = 0; i < codigo.Length; i++)
            {
                if (codigo[i] < '0' || codigo[i] > '9')
                {
                    return false;
                }
            }

            var suma = 0;
            var multiplicador = 3;

            for (var i = codigo.Length - 2; i >= 0; i--)
            {
                var digito = codigo[i] - '0';

                suma += digito * multiplicador;
                multiplicador = multiplicador == 3 ? 1 : 3;
            }

            var digitoVerificador = (10 - (suma % 10)) % 10;
            var digitoIngresado = codigo[codigo.Length - 1] - '0';

            if (digitoVerificador != digitoIngresado)
            {
                return false;
            }

            codigoCanonico = codigo.PadLeft(14, '0');

            return true;
        }

        public static string GetIpAddress(string httpXForwardedFor, string userHostAddress)
        {
            var ipAddress = string.Empty;

            if (httpXForwardedFor != null)
            {
                ipAddress = httpXForwardedFor;
            }
            else if (!string.IsNullOrEmpty(userHostAddress))
            {
                ipAddress = userHostAddress;
            }
            return ipAddress;
        }

        public static string CapitalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            texto = texto.ToLower();

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto);
        }

        public static int GetSessionTimeoutMinutos()
        {
            return HttpContext.Current.Session.Timeout;
        }
    }
}