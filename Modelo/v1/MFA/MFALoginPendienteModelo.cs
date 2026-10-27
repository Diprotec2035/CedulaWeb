using System;

namespace Modelo.v1.MFA
{
    public class MFALoginPendienteModelo
    {
        public uint? EmpresaUsuarioId { get; set; }

        public string RutSuperAdmin { get; set; }

        public string RutEmpresa { get; set; }

        public string NombreEmpresa { get; set; }

        public string RutUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public string ApellidoUsuario { get; set; }

        public string NombrePerfil { get; set; }

        public bool EsSuperAdmin { get; set; }

        public DateTime Expira { get; set; }
    }
}