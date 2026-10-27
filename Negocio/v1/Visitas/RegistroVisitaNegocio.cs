using Datos.v1.Visitas;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Visitas;
using System.Collections.Generic;

namespace Negocio.v1.Visitas
{
    public class RegistroVisitaNegocio
    {
        public static RespuestaModelo<List<ReporteRegistroVisitaModelo>> GetRegistroVisitas(RegistroVisitaFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<ReporteRegistroVisitaModelo>>
            {
                Exitoso = false,
                Datos = new List<ReporteRegistroVisitaModelo>(),
                Mensaje = string.Empty
            };

            if (filtro == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(filtro.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionEmpresa;
                return respuesta;
            }

            filtro.Coincidencia = filtro.Coincidencia != null ? filtro.Coincidencia.Trim() : string.Empty;
            filtro.Desde = filtro.Desde != null ? filtro.Desde.Trim() : string.Empty;
            filtro.Hasta = filtro.Hasta != null ? filtro.Hasta.Trim() : string.Empty;

            var operacion = 0;
            var listado = RegistroVisitaDatos.GetReporteRegistroVisitas(ref operacion, filtro);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = listado;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RegistroVisitaModelo GetRegistroVisita(ref int operacion, RegistroVisitaModelo registroVisita)
        {
            return RegistroVisitaDatos.GetRegistroVisita(ref operacion, registroVisita);
        }
    }
}
