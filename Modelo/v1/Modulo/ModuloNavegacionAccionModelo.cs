namespace Modelo.v1.Modulo
{
    public class ModuloNavegacionAccionModelo
    {
        public uint NavegacionId { get; set; }

        public uint ModuloIdOrigen { get; set; }

        public string Accion { get; set; }

        public uint ModuloIdDestino { get; set; }

        public sbyte? NavegacionEstado { get; set; }

        public uint ModuloId { get; set; }

        public string TituloPagina { get; set; }

        public string Url { get; set; }

        public string UrlPublica { get; set; }

        public sbyte? ModuloEstado { get; set; }
    }
}