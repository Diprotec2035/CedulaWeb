using Datos.v1.DispositivoModelos;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.DispositivoModelos;
using System;
using System.Collections.Generic;

namespace Negocio.v1.DispositivoModelos
{
    public class DispositivoModelosNegocio
    {
        public static RespuestaModelo<List<DispositivoModelosModelo>> GetReporteDispositivoModelos(DispositivoModelosFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<DispositivoModelosModelo>>
            {
                Exitoso = false,
                Datos = new List<DispositivoModelosModelo>(),
                Mensaje = string.Empty,
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty,
                CodigoAuxiliar = 0
            };

            try
            {
                if (filtro == null)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloFiltrosObtener;
                    return respuesta;
                }

                if (filtro.Orden == 0)
                {
                    filtro.Orden = 1;
                }

                if (filtro.Orientacion == 0)
                {
                    filtro.Orientacion = 1;
                }

                var operacion = 0;

                respuesta.Datos = DispositivoModelosDatos.GetReporteDispositivoModelos(
                    ref operacion,
                    filtro.Coincidencia ?? string.Empty,
                    filtro.IdMarca,
                    filtro.Estado,
                    filtro.Orden,
                    filtro.Orientacion
                );

                if (operacion < 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloListadoObtener;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                    return respuesta;
                }

                respuesta.Exitoso = true;
                 respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;

                return respuesta;
            }
            catch (Exception)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloListadoObtener;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                return respuesta;
            }
        }

        public static DispositivoModelosModelo GetDispositivoModelo(ref int operacion, DispositivoModelosModelo modelo)
        {
            return DispositivoModelosDatos.GetDispositivoModelo(ref operacion, modelo);
        }

        public static RespuestaModelo<bool> RegistrarDispositivoModelo(DispositivoModelosModelo modelo)
        {
            var respuesta = new RespuestaModelo<bool>
            {
                Exitoso = false,
                Datos = false,
                Mensaje = string.Empty,
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty,
                CodigoAuxiliar = 0
            };

            try
            {
                var validacion = ValidarDispositivoModelo(modelo, false);

                if (!validacion.Exitoso)
                {
                    respuesta.Mensaje = validacion.Mensaje;
                    return respuesta;
                }

                modelo.Nombre = modelo.Nombre.Trim();
                modelo.Estado = modelo.Estado.HasValue ? modelo.Estado.Value : Convert.ToSByte(0);

                var retorno = DispositivoModelosDatos.InsertDispositivoModelo(modelo);

                if (retorno < 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModeloExiste;
                    return respuesta;
                }

                respuesta.Exitoso = true;
                respuesta.Datos = true;
                 respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                respuesta.Mensaje = ConstantesModelo.ConsExitoDispositivoModeloRegistro;

                return respuesta;
            }
            catch (Exception)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloRegistro;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                return respuesta;
            }
        }

        public static RespuestaModelo<bool> ActualizarDispositivoModelo(DispositivoModelosModelo modelo)
        {
            var respuesta = new RespuestaModelo<bool>
            {
                Exitoso = false,
                Datos = false,
                Mensaje = string.Empty,
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty,
                CodigoAuxiliar = 0
            };

            try
            {
                var validacion = ValidarDispositivoModelo(modelo, true);

                if (!validacion.Exitoso)
                {
                    respuesta.Mensaje = validacion.Mensaje;
                    return respuesta;
                }

                modelo.Nombre = modelo.Nombre.Trim();

                var retorno = DispositivoModelosDatos.UpdateDispositivoModelo(modelo);

                if (retorno == 3)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;
                }

                if (retorno < 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModeloExiste;
                    return respuesta;
                }

                respuesta.Exitoso = true;
                respuesta.Datos = true;
                 respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                respuesta.Mensaje = ConstantesModelo.ConsExitoDispositivoModeloActualizar;

                return respuesta;
            }
            catch (Exception)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloActualizar;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                return respuesta;
            }
        }

        private static RespuestaModelo<bool> ValidarDispositivoModelo(DispositivoModelosModelo modelo, bool esEdicion)
        {
            var respuesta = new RespuestaModelo<bool>
            {
                Exitoso = false,
                Datos = false,
                Mensaje = string.Empty,
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty,
                CodigoAuxiliar = 0
            };

            if (modelo == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloObtener;
                return respuesta;
            }

            if (esEdicion && modelo.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModeloIdInvalido;
                return respuesta;
            }

            if (modelo.MarcaId == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModelosMarcaVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(modelo.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModelosNombreVacio;
                return respuesta;
            }

            if (modelo.Nombre.Trim().Length > 100)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModeloNombreLargo;
                return respuesta;
            }

            if (!modelo.Estado.HasValue)
            {
                modelo.Estado = 0;
            }

            if (modelo.Estado.Value != 0 && modelo.Estado.Value != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModeloEstadoInvalido;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = true;
             respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;

            return respuesta;
        }
    }
}