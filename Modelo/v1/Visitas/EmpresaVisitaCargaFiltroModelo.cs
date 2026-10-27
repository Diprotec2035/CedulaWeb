namespace Modelo.v1.Visitas
{
    public class EmpresaVisitaCargaFiltroModelo
    {
        public string RutEmpresa { get; set; }

        public string Coincidencia { get; set; }

        public sbyte Estado { get; set; }

        public string Desde { get; set; }

        public string Hasta { get; set; }

        public short Orden { get; set; }

        public short Orientacion { get; set; }
    }
}
