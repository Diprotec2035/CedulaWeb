using System;

namespace Modelo.v1.DispositivoModelos
{
    public class DispositivoModelosModelo
    {
        public uint Id { get; set; }

        public uint MarcaId { get; set; }

        public string MarcaNombre { get; set; }

        public string Nombre { get; set; }

        public sbyte? Estado { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}