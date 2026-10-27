using System.Collections.Generic;

namespace Modelo.v1.MFA
{
    public class MFATotpConfirmacionModelo : MFATotpModelo
    {
        public List<string> CodigosRecuperacion { get; set; }
    }
}