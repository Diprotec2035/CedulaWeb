using Datos.v1.BaseDatos;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.BaseDatos;
using System.Collections.Generic;

namespace Negocio.v1.BaseDatos
{
    public class BaseDatosNegocio
    {
        public static RespuestaModelo<List<BaseDatosTablaModelo>> GetTablas()
        {
            var respuesta = new RespuestaModelo<List<BaseDatosTablaModelo>>();

            var operacion = 0;

            var tablas = BaseDatosDatos.GetTablas(ref operacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = tablas;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<List<BaseDatosProcedimientoModelo>> GetProcedimientosTabla(string tablaNombre)
        {
            var respuesta = new RespuestaModelo<List<BaseDatosProcedimientoModelo>>();

            if (string.IsNullOrWhiteSpace(tablaNombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionBaseDatosTablaVacia;
                return respuesta;
            }

            var operacion = 0;

            var procedimientos = BaseDatosDatos.GetProcedimientosTabla(ref operacion, tablaNombre);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = procedimientos;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<List<BaseDatosColumnaModelo>> GetColumnasTabla(string tablaNombre)
        {
            var respuesta = new RespuestaModelo<List<BaseDatosColumnaModelo>>();

            if (string.IsNullOrWhiteSpace(tablaNombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionBaseDatosTablaVacia;
                return respuesta;
            }

            var operacion = 0;

            var columnas = BaseDatosDatos.GetColumnasTabla(ref operacion, tablaNombre);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = columnas;
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