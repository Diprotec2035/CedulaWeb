namespace Modelo.v1.MFA
{
    public class MFATotpEnrolamientoModelo
    {
        public uint Id { get; set; }

        public string SecretBase32 { get; set; }

        public string UriOtpAuth { get; set; }

        public string Algoritmo { get; set; }

        public byte Digitos { get; set; }

        public ushort PeriodoSegundos { get; set; }
    }
}