using System;
using System.Collections.Generic;

namespace Modelo.Comunes
{
    [Serializable]
    public class ListadoFiltroSesionModelo
    {
        public uint ModuloId { get; set; }
        public int PageIndex { get; set; }

        public Dictionary<string, string> Filtros { get; set; }

        public ListadoFiltroSesionModelo()
        {
            Filtros = new Dictionary<string, string>();
        }
    }
}
