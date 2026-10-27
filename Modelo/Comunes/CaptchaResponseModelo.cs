using System;
using System.Collections.Generic;

namespace Modelo
{
    public class CaptchaResponseModelo
    {
        public bool success { get; set; }

        public float score { get; set; }

        public string action { get; set; }

        public DateTime challenge_ts { get; set; }

        public string hostname { get; set; }

        public List<string> error_codes { get; set; }
    }
}
