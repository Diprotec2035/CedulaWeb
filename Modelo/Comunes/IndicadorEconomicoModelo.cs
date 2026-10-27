using System;

namespace Modelo.Comunes
{
    public class IndicadorEconomicoModelo
    {
        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public string UnidadMedida { get; set; }

        public decimal Valor { get; set; }

        public DateTime Fecha { get; set; }

        public string ValorFormateado { get; set; }
    }
}
