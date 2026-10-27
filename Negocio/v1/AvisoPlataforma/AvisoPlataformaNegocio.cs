using Datos.v1.AvisoPlataforma;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.AvisoPlataforma;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Negocio.v1.AvisoPlataforma
{
    public class AvisoPlataformaNegocio
    {
        private const int MaximoDiasVigencia = 20;

        public static RespuestaModelo<List<AvisoPlataformaModelo>> GetReporteAvisosPlataforma(AvisoPlataformaFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<AvisoPlataformaModelo>>();

            var operacion = 0;

            var avisos = AvisoPlataformaDatos.GetReporteAvisosPlataforma(
                ref operacion,
                filtro.Coincidencia,
                filtro.Estado,
                filtro.Orden,
                filtro.Orientacion
            );

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = avisos;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static AvisoPlataformaModelo GetAvisoPlataforma(ref int operacion, AvisoPlataformaModelo aviso)
        {
            return AvisoPlataformaDatos.GetAvisoPlataforma(ref operacion, aviso);
        }

        public static AvisoPlataformaModelo GetAvisoPlataformaVigente(ref int operacion)
        {
            return AvisoPlataformaDatos.GetAvisoPlataformaVigente(ref operacion);
        }

        public static RespuestaModelo<AvisoPlataformaModelo> RegistrarAvisoPlataforma(AvisoPlataformaModelo aviso)
        {
            var respuesta = new RespuestaModelo<AvisoPlataformaModelo>();

            var validacion = ValidarAviso(aviso, false);

            if (!validacion.Exitoso)
            {
                return validacion;
            }

            var resultado = AvisoPlataformaDatos.InsertAvisoPlataforma(aviso);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = aviso;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoAvisoPlataformaRegistro;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<AvisoPlataformaModelo> ActualizarAvisoPlataforma(AvisoPlataformaModelo aviso)
        {
            var respuesta = new RespuestaModelo<AvisoPlataformaModelo>();

            if (aviso.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            var validacion = ValidarAviso(aviso, true);

            if (!validacion.Exitoso)
            {
                return validacion;
            }

            var resultado = AvisoPlataformaDatos.UpdateAvisoPlataforma(aviso);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = aviso;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoAvisoPlataformaActualizar;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case 3:
                    respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;
            }
        }

        private static RespuestaModelo<AvisoPlataformaModelo> ValidarAviso(AvisoPlataformaModelo aviso, bool validarEstado)
        {
            var respuesta = new RespuestaModelo<AvisoPlataformaModelo>();

            if (string.IsNullOrWhiteSpace(aviso.Titulo))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaTituloVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(aviso.Mensaje))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaMensajeVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(aviso.Version))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaVersionVacia;
                return respuesta;
            }

            DateTime fechaInicio;
            DateTime fechaTermino;

            if (!DateTime.TryParseExact(aviso.FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicio))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaFechaInicioInvalida;
                return respuesta;
            }

            if (!DateTime.TryParseExact(aviso.FechaTermino, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaTermino))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaFechaTerminoInvalida;
                return respuesta;
            }

            if (fechaTermino < fechaInicio)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaRangoFechasInvalido;
                return respuesta;
            }

            if ((fechaTermino - fechaInicio).TotalDays > MaximoDiasVigencia)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaMaximoDias;
                return respuesta;
            }

            if (validarEstado && !aviso.Estado.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaEstadoInvalido;
                return respuesta;
            }

            if (validarEstado && aviso.Estado.Value != 0 && aviso.Estado.Value != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionAvisoPlataformaEstadoInvalido;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = aviso;

            return respuesta;
        }
    }
}