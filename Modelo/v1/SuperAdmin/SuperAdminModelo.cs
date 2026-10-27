using System;

namespace Modelo.v1.SuperAdmin
{
    public class SuperAdminModelo
    {
        public string RutAdmin { get; set; }

        public string RutUsuario { get; set; }

        public string NombreAdmin { get; set; }

        public string ApellidoAdmin { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmar { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public string PasswordAlgoritmo { get; set; }

        public sbyte? Estado { get; set; }

        public string EstadoNombre { get; set; }

        public DateTime UpdatedAt { get; set; }

        public sbyte MFARequerido { get; set; }

        public uint? MFATotpId { get; set; }

        public sbyte? MFAConfigurado { get; set; }

        public sbyte? MFAEstado { get; set; }
    }
}