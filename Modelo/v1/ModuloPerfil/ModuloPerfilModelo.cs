using System;

namespace Modelo.v1.ModuloPerfil
{
    public class ModuloPerfilModelo
    {
        public uint Id { get; set; }

        public sbyte Consulta { get; set; }

        public sbyte Nuevo { get; set; }

        public sbyte Modifica { get; set; }

        public sbyte Estado { get; set; }

        public uint IdModulo { get; set; }

        public uint IdPerfil { get; set; }

        public uint IdCategoria { get; set; }

        public string NombreModulo { get; set; }

        public string RutEmpresa { get; set; }

        public string RutUsuario { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}