using System;

namespace Modelo.v1.Dispositivo
{
    public class DispositivoModelo
    {
        public uint Id { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public uint? MarcaId { get; set; }

        public string MarcaNombre { get; set; }

        public uint? ModeloId { get; set; }

        public string ModeloNombre { get; set; }

        public string PartNumber { get; set; }

        public string SerialNumber { get; set; }

        public string Comentario { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string ActivationCode { get; set; }

        public string ActivationCodeHash { get; set; }

        public string RutEmpresa { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}