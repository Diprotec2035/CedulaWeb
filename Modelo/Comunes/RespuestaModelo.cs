namespace Modelo.Comunes
{
    public class RespuestaModelo<T>
    {
        public bool Exitoso { get; set; }

        public string Mensaje { get; set; }

        public string AlertaColor { get; set; }

        public string AlertaTitulo { get; set; }

        public int CodigoAuxiliar { get; set; }

        public T Datos { get; set; }

        public RespuestaModelo()
        {
            Exitoso = false;
            Mensaje = string.Empty;
            AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
            AlertaTitulo = string.Empty;
            CodigoAuxiliar = 0;
            Datos = default(T);
        }
    }
}
