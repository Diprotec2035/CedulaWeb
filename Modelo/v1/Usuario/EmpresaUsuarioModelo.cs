using System;

namespace Modelo
{
    public class EmpresaUsuarioModelo
    {
        public uint Id { get; set; }

        public string RutEmpresa { get; set; }

        public string NombreEmpresa { get; set; }

        public sbyte EstadoEmpresa { get; set; }

        public string RutUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public string ApellidoUsuario { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public uint IdPerfil { get; set; }

        public string NombrePerfil { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmar { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public string PasswordAlgoritmo { get; set; }

        public bool EsEdicionMisDatos { get; set; }

        public bool EsSuperAdmin { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string RutUsuarioBitacora { get; set; }

        public sbyte MFARequerido { get; set; }

        public uint? MFATotpId { get; set; }

        public sbyte? MFAConfigurado { get; set; }

        public sbyte? MFAEstado { get; set; }
    }
}