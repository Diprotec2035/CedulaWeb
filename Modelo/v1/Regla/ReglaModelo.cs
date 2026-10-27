using System;

namespace Modelo.v1.Regla
{
    public class ReglaModelo
    {
        public uint Id { get; set; }

        public string Nombre { get; set; }

        public sbyte NombreApellido { get; set; }

        public sbyte Empresa { get; set; }

        public sbyte Patente { get; set; }

        public sbyte Comentario { get; set; }

        public sbyte Fotografia { get; set; }

        public sbyte EntradaSalida { get; set; }

        public sbyte ListaBlancaNegra { get; set; }

        public sbyte EliminaEnviados { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string RutEmpresa { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}