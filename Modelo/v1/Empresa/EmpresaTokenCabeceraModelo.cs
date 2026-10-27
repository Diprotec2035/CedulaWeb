using System;

namespace Modelo.v1.Empresa
{
    public class EmpresaTokenCabeceraModelo
    {
        public uint? Id { get; set; }

        public string RutEmpresa { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public sbyte? Revocado { get; set; }

        public uint TokenTotal { get; set; }

        public uint TokenTotalOperativos { get; set; }

        public sbyte TokenVigenciaCodigo { get; set; }

        public string TokenVigenciaNombre { get; set; }

        public int? TokenDiasRestantes { get; set; }

        public DateTime? TokenAnteriorExpiresAt { get; set; }

        public int? TokenAnteriorDiasRestantes { get; set; }
    }
}