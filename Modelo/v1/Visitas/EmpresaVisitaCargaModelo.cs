namespace Modelo.v1.Visitas
{
    public class EmpresaVisitaCargaModelo
    {
        public uint Id { get; set; }

        public string RutEmpresa { get; set; }

        public string ArchivoNombre { get; set; }

        public string ArchivoRuta { get; set; }

        public int TotalFilas { get; set; }

        public int TotalOk { get; set; }

        public int TotalError { get; set; }

        public sbyte Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string CreatedAt { get; set; }

        public string CreatedFecha { get; set; }

        public string CreatedHora { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedByNombre { get; set; }
    }
}
