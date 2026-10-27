using Datos.v1.Log;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Log;
using System.Collections.Generic;

namespace Negocio.v1.Log
{
    public class LogAccesoDispositivoNegocio
    {
        public static RespuestaModelo<List<LogAccesoDispositivoModulo>> GetLogAccesoDispositivo(LogAccesoDispositivoFiltroModulo filtro)
        {
            var respuesta = new RespuestaModelo<List<LogAccesoDispositivoModulo>>();

            var operacion = 0;
            var logAccesoDispositivo = LogAccesoDispositivoDatos.GetReporteLogAccesoDispositivo(
                ref operacion,
                filtro.RutEmpresa,
                filtro.Coincidencia,
                filtro.Desde,
                filtro.Hasta,
                filtro.Orden,
                filtro.Orientacion
            );

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = logAccesoDispositivo;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;
                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }
    }
}
