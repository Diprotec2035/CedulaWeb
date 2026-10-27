using System;

namespace Modelo.v1.AvisoPlataforma
{
    public class AvisoPlataformaModelo
    {
        public uint Id { get; set; }

        public string Titulo { get; set; }

        public string Mensaje { get; set; }

        public string Version { get; set; }

        public string FechaInicio { get; set; }

        public string FechaTermino { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}