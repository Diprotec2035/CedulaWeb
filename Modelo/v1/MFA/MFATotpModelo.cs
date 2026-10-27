using System;

namespace Modelo.v1.MFA
{
    public class MFATotpModelo
    {
        public uint Id { get; set; }

        public byte[] SecretProtegido { get; set; }

        public string Algoritmo { get; set; }

        public byte Digitos { get; set; }

        public ushort PeriodoSegundos { get; set; }

        public sbyte Configurado { get; set; }

        public sbyte Estado { get; set; }

        public ushort IntentosFallidos { get; set; }

        public DateTime? BloqueadoHasta { get; set; }

        public long? UltimoPasoUsado { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? LastUsedAt { get; set; }

        public uint? EmpresaUsuarioId { get; set; }

        public string RutSuperAdmin { get; set; }
    }
}
