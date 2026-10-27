namespace Modelo.v1.Visitas
{
    public class EmpresaVisitaCargaDetalleModelo
    {
        public uint Id { get; set; }

        public uint IdCarga { get; set; }

        public int Fila { get; set; }

        public string Rut { get; set; }

        public string Nombre { get; set; }

        public string Empresa { get; set; }

        public sbyte Accion { get; set; }

        public string AccionNombre { get; set; }

        public string NombreAnterior { get; set; }

        public string EmpresaAnterior { get; set; }

        public sbyte Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string Mensaje { get; set; }
    }
}
