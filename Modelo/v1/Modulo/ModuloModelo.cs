using System;

namespace Modelo.v1.Modulo
{
    public class ModuloModelo
    {
        public uint Id { get; set; }

        public string Nombre { get; set; }

        public string TituloPagina { get; set; }

        public string TituloContenido { get; set; }

        public sbyte? Orden { get; set; }

        public string Url { get; set; }

        public string UrlPublica { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public sbyte? Administrador { get; set; }

        public string AdministradorNombre { get; set; }

        public sbyte? MenuVisible { get; set; }

        public string MenuVisibleNombre { get; set; }

        public sbyte? BitacoraEstado { get; set; }

        public string BitacoraEstadoNombre { get; set; }

        public string BitacoraSp { get; set; }

        public string BitacoraParametro { get; set; }

        public uint CategoriaId { get; set; }

        public string CategoriaNombre { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string RutUsuario { get; set; }
    }
}