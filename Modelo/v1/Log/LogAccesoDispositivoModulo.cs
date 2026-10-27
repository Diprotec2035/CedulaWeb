namespace Modelo.v1.Log
{
    public class LogAccesoDispositivoModulo
    {
        public uint Id { get; set; }

        public string Ip { get; set; }

        public string EndPoint { get; set; }

        public string Metodo { get; set; }

        public short StatusCode { get; set; }

        public string Resultado { get; set; }

        public string Agente { get; set; }

        public string Token { get; set; }

        public uint Duracion { get; set; }

        public string CorrelationId { get; set; }

        public string Fecha { get; set; }

        public string Hora { get; set; }

        public string RutEmpresa { get; set; }

        public uint DispositivoId { get; set; }

        public string SerialNumber { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }
    }
}