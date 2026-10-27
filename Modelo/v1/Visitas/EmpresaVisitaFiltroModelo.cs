namespace Modelo.v1.Visitas
{
    public class EmpresaVisitaFiltroModelo
    {
        public string RutEmpresa { get; set; }

        public string Coincidencia { get; set; }

        public short? Vigente { get; set; }

        public short Orden { get; set; }

        public short Orientacion { get; set; }
    }
}
