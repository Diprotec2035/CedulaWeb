using System;

namespace Modelo.v1.Perfil
{
    public class PerfilModelo
    {
        public uint Id { get; set; }

        public string Nombre { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string RutEmpresa { get; set; }

        public uint IdRegla { get; set; }

        public string NomRegla { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}