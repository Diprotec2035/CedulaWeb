namespace Modelo.v1.Visitas
{
    public class EmpresaVisitaCargaDetalleFiltroModelo
    {
        public uint IdCarga { get; set; }

        public string RutEmpresa { get; set; }

        public sbyte Estado { get; set; }

        public sbyte Accion { get; set; }

        public string Coincidencia { get; set; }
    }
}
