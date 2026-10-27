using System;

namespace Modelo.v1.Empresa
{
    public class EmpresaTokenModelo
    {
        public uint Id { get; set; }

        public string Jti { get; set; }

        public string TokenHash { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

        public DateTime EmitidoAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public sbyte VigenciaCodigo { get; set; }

        public string VigenciaNombre { get; set; }

        public int? DiasRestantes { get; set; }

        public sbyte Revocado { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string RevokedBy { get; set; }

        public string RevocationReason { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string RutEmpresa { get; set; }

        public string RutUsuario { get; set; }
    }

    public class EmpresaTokenGeneradoModelo
    {
        public string Token { get; set; }

        public string Jti { get; set; }

        public DateTime EmitidoAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string RutEmpresa { get; set; }
    }
}