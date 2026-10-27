namespace Modelo.v1.Modulo
{
    public class ModuloContextoModelo
    {
        public bool Exitoso { get; set; }

        public string Mensaje { get; set; }

        public uint ModuloId { get; set; }

        public string Nombre { get; set; }

        public string TituloPagina { get; set; }

        public string TituloContenido { get; set; }

        public string UrlActual { get; set; }

        public sbyte? Estado { get; set; }

        public sbyte? BitacoraEstado { get; set; }

        public string BitacoraSp { get; set; }

        public string BitacoraParametro { get; set; }
    }
}