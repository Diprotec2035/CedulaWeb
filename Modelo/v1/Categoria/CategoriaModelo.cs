using System;

namespace Modelo
{
    public class CategoriaModelo
    {
        public uint Id { get; set; }

        public string Nombre { get; set; }

        public sbyte? Orden { get; set; }

        public string Icono { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}