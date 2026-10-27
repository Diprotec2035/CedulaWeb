namespace Modelo.v1.Visitas
{
    public class ReporteRegistroVisitaModelo
    {
        public uint Id { get; set; }

        public string Rut { get; set; }

        public string Fecha { get; set; }

        public string Hora { get; set; }

        public string Ip { get; set; }

        public string NombreApellido { get; set; }

        public string Empresa { get; set; }

        public string Patente { get; set; }

        public string Comentario { get; set; }

        public string Fotografia { get; set; }

        public sbyte EntradaSalida { get; set; }

        public string RutEmpresa { get; set; }

        public string RutUsuario { get; set; }

        public uint IdDispositivo { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public string SerialNumber { get; set; }
    }
}
