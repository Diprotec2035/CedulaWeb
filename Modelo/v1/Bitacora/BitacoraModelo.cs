namespace Modelo.v1.Bitacora
{
    public class BitacoraModelo
    {
        public uint Id { get; set; }

        public string Fecha { get; set; }

        public string Hora { get; set; }

        public string RutUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public string Accion { get; set; }

        public string Comentarios { get; set; }

        public string Detalle { get; set; }

        public string IdRegistro { get; set; }
    }
}
