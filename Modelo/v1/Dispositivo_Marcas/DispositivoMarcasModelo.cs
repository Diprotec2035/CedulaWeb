using System;

namespace Modelo.v1.DispositivoMarcas
{
    public class DispositivoMarcasModelo
    {
        public uint Id { get; set; }

        public string Nombre { get; set; }

        public sbyte? Estado { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}