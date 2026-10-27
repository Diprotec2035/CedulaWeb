using System;

namespace Modelo.v1.Visitas
{
    public class EmpresaVisitaModelo
    {
        public string Rut { get; set; }

        public string Nombre { get; set; }

        public string Empresa { get; set; }

        public sbyte Vigente { get; set; }

        public string RutEmpresa { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
