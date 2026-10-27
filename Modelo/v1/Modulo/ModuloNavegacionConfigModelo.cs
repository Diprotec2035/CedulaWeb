namespace Modelo.v1.Modulo
{
    public class ModuloNavegacionConfigModelo
    {
        public uint Id { get; set; }

        public uint ModuloIdOrigen { get; set; }

        public string ModuloNombreOrigen { get; set; }

        public string Accion { get; set; }

        public uint ModuloIdDestino { get; set; }

        public string ModuloNombreDestino { get; set; }

        public string ModuloUrlDestino { get; set; }

        public string ModuloUrlPublicaDestino { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string RutUsuario { get; set; }
    }
}
