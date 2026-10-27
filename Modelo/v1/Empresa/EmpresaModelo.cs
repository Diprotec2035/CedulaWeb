using System;

namespace Modelo
{
    public class EmpresaModelo
    {
        public string Rut { get; set; }

        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Comuna { get; set; }

        public string Ciudad { get; set; }

        public string Contacto { get; set; }

        public string ContactoTelefono { get; set; }

        public string ContactoEmail { get; set; }

        public sbyte? DispositivoExterno { get; set; }

        public sbyte? Estado { get; set; }

        public string RutUsuario { get; set; }

        public DateTime UpdatedAt { get; set; }

        public uint? TokenId { get; set; }

        public DateTime? TokenCreatedAt { get; set; }

        public DateTime? TokenExpiresAt { get; set; }

        public int TokenTotal { get; set; }

        public int TokenTotalOperativos { get; set; }

        public sbyte TokenVigenciaCodigo { get; set; }

        public string TokenVigenciaNombre { get; set; }

        public int? TokenDiasRestantes { get; set; }
    }
}