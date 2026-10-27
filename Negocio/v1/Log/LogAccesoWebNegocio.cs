using Datos.v1.Log;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Log;
using System.Collections.Generic;

namespace Negocio.v1.Log
{
    public class LogAccesoWebNegocio
    {
        public static RespuestaModelo<List<LogAccesoWebModelo>> GetLogAccesoWeb(LogAccesoWebFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<LogAccesoWebModelo>>();

            var operacion = 0;
            var logAccesoWeb = LogAccesoWebDatos.GetReporteLogAccesoWeb(
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
                    respuesta.Datos = logAccesoWeb;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<LogAccesoWebModelo> RegistrarLogAccesoWeb(LogAccesoWebModelo logAccesoWeb)
        {
            var respuesta = new RespuestaModelo<LogAccesoWebModelo>();

            var resultado = LogAccesoWebDatos.InsertLogAccesoWeb(logAccesoWeb);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = logAccesoWeb;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
            }
        }
    }
}