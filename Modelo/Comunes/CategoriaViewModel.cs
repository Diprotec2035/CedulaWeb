using Modelo.v1.Modulo;
using System.Collections.Generic;

namespace Modelo.Comunes
{
    public class CategoriaViewModel
    {
        public uint Id { get; set; }
        public string Texto { get; set; }
        public string Icono { get; set; }
        public List<ModuloModelo> Modulos { get; set; } = new List<ModuloModelo>();
    }
}
