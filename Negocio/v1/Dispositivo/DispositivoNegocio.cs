using Datos.v1.Dispositivo;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Dispositivo;
using Negocio.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Negocio.Comunes.ActivationCodeNegocio;

namespace Negocio.v1.Dispositivo
{
    public class DispositivoNegocio
    {
        public static List<DispositivoModelo> GetReporteDispositivos(ref int operacion, string emprut, string buscador, short estado, short orden, short orientacion)
        {
            return DispositivoDatos.GetReporteDispositivos(ref operacion, emprut, buscador, estado, orden, orientacion);
        }

        public static RespuestaModelo<List<DispositivoModelo>> GetDispositivos(DispositivoFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<DispositivoModelo>>();

            var operacion = 0;
            var dispositivos = DispositivoDatos.GetReporteDispositivos(
                ref operacion,
                filtro.RutEmpresa,
                filtro.Coincidencia,
                filtro.Estado,
                filtro.Orden,
                filtro.Orientacion
            );

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = dispositivos;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static DispositivoModelo GetDispositivo(ref int operacion, DispositivoModelo dispositivo)
        {
            return DispositivoDatos.GetDispositivo(ref operacion, dispositivo);
        }

        public static int UpdateDispositivo(DispositivoModelo dispositivo)
        {
            return DispositivoDatos.UpdateDispositivo(dispositivo);
        }

        public static int InsertDispositivo(DispositivoModelo dispositivo)
        {
            return DispositivoDatos.InsertDispositivo(dispositivo);
        }

        public static async Task<RespuestaModelo<DispositivoModelo>> RegistrarDispositivoAsync(DispositivoModelo dispositivo)
        {
            var respuesta = new RespuestaModelo<DispositivoModelo>();

            if (!dispositivo.ModeloId.HasValue || dispositivo.ModeloId.Value == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModeloVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.PartNumber))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoPartnumberVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.SerialNumber))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoSerialnumberVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            var operacion = 0;

            var existeSerial = DispositivoDatos.GetDispositivoPorSerie(ref operacion, dispositivo);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                return respuesta;
            }

            if (existeSerial)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoSerialNumberExiste;
                return respuesta;
            }

            operacion = 0;

            var empresaConsulta = new EmpresaModelo
            {
                Rut = dispositivo.RutEmpresa,
                Estado = 0
            };

            var empresa = EmpresaNegocio.GetEmpresa(ref operacion, empresaConsulta);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                return respuesta;
            }

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaExternoInvalido;
                return respuesta;
            }

            if (empresa.DispositivoExterno != 0 && empresa.DispositivoExterno != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaExternoInvalido;
                return respuesta;
            }

            if (empresa.DispositivoExterno == 1)
            {
                var validacionCommerce = await CommerceDispositivoServicio.ValidarDispositivoAsync(dispositivo.PartNumber, dispositivo.SerialNumber);

                if (!validacionCommerce.ConsultaProcesada)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoCommerceNoDisponible;
                    return respuesta;
                }

                if (!validacionCommerce.DispositivoValido)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoNoVendidoDiprotec;
                    return respuesta;
                }
            }

            var activation = ActivationCodeService.Generate(12, 4);
            dispositivo.ActivationCode = activation.Code;
            dispositivo.ActivationCodeHash = activation.Hash;

            var resultado = DispositivoDatos.InsertDispositivo(dispositivo);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = dispositivo;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoDispositivoRegistro;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case -1:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloNoVigente;
                    return respuesta;

                case -2:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<DispositivoModelo> ActualizarDispositivo(DispositivoModelo dispositivo)
        {
            var respuesta = new RespuestaModelo<DispositivoModelo>();

            if (dispositivo.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoIdVacio;
                return respuesta;
            }

            if (!dispositivo.ModeloId.HasValue || dispositivo.ModeloId.Value == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoModeloVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.PartNumber))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoPartnumberVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.SerialNumber))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoSerialnumberVacio;
                return respuesta;
            }

            if (!dispositivo.Estado.HasValue || (dispositivo.Estado.Value != 0 && dispositivo.Estado.Value != 1 && dispositivo.Estado.Value != 2))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoEstadoInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoRutEmpresaVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            var operacion = 0;

            var dispositivoFiltro = new DispositivoModelo
            {
                Id = dispositivo.Id,
                RutEmpresa = dispositivo.RutEmpresa,
                Estado = null
            };

            var dispositivoActual = DispositivoDatos.GetDispositivo(ref operacion, dispositivoFiltro);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                return respuesta;
            }

            if (dispositivoActual == null || dispositivoActual.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRegistroObtener;
                return respuesta;
            }

            if (dispositivoActual.UpdatedAt != dispositivo.UpdatedAt)
            {
                respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            var partNumberModificado = !string.Equals(dispositivo.PartNumber.Trim(), dispositivoActual.PartNumber.Trim(), StringComparison.OrdinalIgnoreCase);
            var serialNumberModificado = !string.Equals(dispositivo.SerialNumber.Trim(), dispositivoActual.SerialNumber.Trim(), StringComparison.OrdinalIgnoreCase);

            if (partNumberModificado || serialNumberModificado)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoIdentidadNoModificable;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            dispositivo.PartNumber = dispositivoActual.PartNumber;
            dispositivo.SerialNumber = dispositivoActual.SerialNumber;

            var resultado = DispositivoDatos.UpdateDispositivo(dispositivo);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = dispositivo;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoDispositivoActualizar;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case 3:
                    respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case -1:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorDispositivoModeloNoVigente;
                    return respuesta;

                case -2:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<DispositivoModelo> RegenerarCodigoActivacion(DispositivoModelo dispositivo)
        {
            var respuesta = new RespuestaModelo<DispositivoModelo>();

            if (dispositivo.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoIdVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(dispositivo.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoRutEmpresaVacio;
                return respuesta;
            }

            var activation = ActivationCodeService.Generate(12, 4);
            dispositivo.ActivationCode = activation.Code;
            dispositivo.ActivationCodeHash = activation.Hash;

            var resultado = DispositivoDatos.UpdateDispositivoActivationCodeHash(dispositivo);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = dispositivo;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoDispositivoActualizarActivationCode;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case -1:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;

                case -2:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
            }
        }
    }
}