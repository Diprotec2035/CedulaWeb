using System;

namespace cedulaweb.diprotec.cl.aplicacion.helpers
{
    public class TextoPresentacionHelper
    {
        public static string ObtenerTextoAbreviado(object valor, int cantidadCaracteres, bool conservarFinal)
        {
            if (valor == null || valor == DBNull.Value)
            {
                return string.Empty;
            }

            var texto = Convert.ToString(valor).Trim();

            if (string.IsNullOrWhiteSpace(texto) || texto.Length <= cantidadCaracteres)
            {
                return texto;
            }

            if (conservarFinal)
            {
                return "..." + texto.Substring(texto.Length - cantidadCaracteres, cantidadCaracteres);
            }

            return texto.Substring(0, cantidadCaracteres) + "...";
        }
    }
}