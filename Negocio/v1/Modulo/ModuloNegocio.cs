using Datos.v1.Modulo;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Modulo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.v1.Modulo
{
    public class ModuloNegocio
    {
        public static RespuestaModelo<List<ModuloModelo>> GetReporteModulos(ModuloFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<ModuloModelo>>();

            var operacion = 0;

            var modulos = ModuloDatos.GetReporteModulos(
                ref operacion,
                filtro.Coincidencia,
                filtro.Estado,
                filtro.CategoriaId,
                filtro.Orden,
                filtro.Orientacion
            );

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = modulos;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static ModuloModelo GetModuloMantenedor(ref int operacion, uint moduloId)
        {
            return ModuloDatos.GetModuloMantenedor(ref operacion, moduloId);
        }

        public static RespuestaModelo<ModuloModelo> RegistrarModulo(ModuloModelo modulo)
        {
            var respuesta = new RespuestaModelo<ModuloModelo>();

            var validacion = ValidarModulo(modulo, false);

            if (!validacion.Exitoso)
            {
                return validacion;
            }

            var validacionUrls = ValidarUrlsModulo(0, modulo.Url, modulo.UrlPublica);

            if (!validacionUrls.Exitoso)
            {
                respuesta.Exitoso = false;
                respuesta.Datos = modulo;
                respuesta.Mensaje = validacionUrls.Mensaje;
                respuesta.AlertaColor = validacionUrls.AlertaColor;
                respuesta.AlertaTitulo = validacionUrls.AlertaTitulo;
                return respuesta;
            }

            var resultado = ModuloDatos.InsertModulo(modulo);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = modulo;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoModuloRegistro;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<ModuloModelo> ActualizarModulo(ModuloModelo modulo)
        {
            var respuesta = new RespuestaModelo<ModuloModelo>();

            if (modulo.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            var validacion = ValidarModulo(modulo, true);

            if (!validacion.Exitoso)
            {
                return validacion;
            }

            var validacionUrls = ValidarUrlsModulo(modulo.Id, modulo.Url, modulo.UrlPublica);

            if (!validacionUrls.Exitoso)
            {
                respuesta.Exitoso = false;
                respuesta.Datos = modulo;
                respuesta.Mensaje = validacionUrls.Mensaje;
                respuesta.AlertaColor = validacionUrls.AlertaColor;
                respuesta.AlertaTitulo = validacionUrls.AlertaTitulo;
                return respuesta;
            }

            var resultado = ModuloDatos.UpdateModulo(modulo);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = modulo;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoModuloActualizar;
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

        public static List<ModuloModelo> GetModuloUsuario(ref int operacion, EmpresaUsuarioModelo usuario)
        {
            return ModuloDatos.GetModuloUsuario(ref operacion, usuario);
        }

        public static List<ModuloModelo> GetModulosMenu(ref int operacion)
        {
            return ModuloDatos.GetModulosMenu(ref operacion);
        }

        public static List<ModuloModelo> GetModulos(ref int operacion, int? visible)
        {
            return ModuloDatos.GetModulos(ref operacion, visible);
        }

        private static RespuestaModelo<ModuloModelo> ValidarModulo(ModuloModelo modulo, bool validarEstado)
        {
            var respuesta = new RespuestaModelo<ModuloModelo>();

            if (modulo == null)
            {
                respuesta.Mensaje = validarEstado
                    ? ConstantesModelo.ConsErrorGenericoActualizar
                    : ConstantesModelo.ConsErrorGenericoRegistrar;

                return respuesta;
            }

            if (modulo.CategoriaId == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloCategoriaVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(modulo.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(modulo.TituloPagina))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloTituloPaginaVacio;
                return respuesta;
            }

            if (modulo.TituloPagina.Trim().Length > 150)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloTituloPaginaLargo;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(modulo.TituloContenido))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloTituloContenidoVacio;
                return respuesta;
            }

            if (modulo.TituloContenido.Trim().Length > 150)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloTituloContenidoLargo;
                return respuesta;
            }

            if (!modulo.Orden.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloOrdenVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(modulo.Url))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloUrlVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(modulo.UrlPublica))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloUrlPublicaVacia;
                return respuesta;
            }

            if (validarEstado && !modulo.Estado.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloEstadoInvalido;
                return respuesta;
            }

            if (!modulo.Administrador.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloAdministradorInvalido;
                return respuesta;
            }

            if (!modulo.MenuVisible.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloMenuVisibleInvalido;
                return respuesta;
            }

            if (!modulo.BitacoraEstado.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloBitacoraEstadoInvalido;
                return respuesta;
            }

            if (modulo.BitacoraEstado.Value != 0 && modulo.BitacoraEstado.Value != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloBitacoraEstadoInvalido;
                return respuesta;
            }

            if (modulo.BitacoraEstado.Value == 0)
            {
                if (string.IsNullOrWhiteSpace(modulo.BitacoraSp))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloBitacoraSpVacio;
                    return respuesta;
                }

                if (string.IsNullOrWhiteSpace(modulo.BitacoraParametro))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloBitacoraParametroVacio;
                    return respuesta;
                }
            }
            else
            {
                modulo.BitacoraSp = string.Empty;
                modulo.BitacoraParametro = string.Empty;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = modulo;

            return respuesta;
        }

        private static RespuestaModelo<bool> ValidarUrlsModulo(uint moduloId, string url, string urlPublica)
        {
            var respuesta = new RespuestaModelo<bool>
            {
                Exitoso = false,
                Datos = false,
                AlertaColor = ConstantesModelo.ConsAlertaColorError,
                AlertaTitulo = string.Empty
            };

            var operacion = 0;

            var duplicados = ModuloDatos.GetModuloUrlDuplicada(
                ref operacion,
                moduloId,
                url,
                urlPublica
            );

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloUrlDuplicadaError;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorError;
                return respuesta;
            }

            if (duplicados == null || duplicados.Count == 0)
            {
                respuesta.Exitoso = true;
                respuesta.Datos = true;
                respuesta.Mensaje = string.Empty;
                respuesta.AlertaColor = string.Empty;
                return respuesta;
            }

            foreach (var duplicado in duplicados)
            {
                if (!string.IsNullOrWhiteSpace(url) && string.Equals(duplicado.Url, url, StringComparison.OrdinalIgnoreCase))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloUrlDuplicada;
                    return respuesta;
                }

                if (!string.IsNullOrWhiteSpace(urlPublica) && string.Equals(duplicado.UrlPublica, urlPublica, StringComparison.OrdinalIgnoreCase))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloUrlPublicaDuplicada;
                    return respuesta;
                }
            }

            respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloUrlDuplicada;
            return respuesta;
        }

        public static ModuloContextoModelo GetModuloContexto(uint moduloId, string urlReal, string urlPublica)
        {
            var respuesta = new ModuloContextoModelo();

            if (moduloId == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloContextoIdInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(urlReal) && string.IsNullOrWhiteSpace(urlPublica))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloContextoNoEncontrado;
                return respuesta;
            }

            var operacion = 0;

            var modulo = ModuloDatos.GetModuloContexto(
                ref operacion,
                moduloId,
                urlReal,
                urlPublica
            );

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloContextoError;
                return respuesta;
            }

            if (modulo == null || modulo.ModuloId == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloContextoNoEncontrado;
                return respuesta;
            }

            if (!modulo.Estado.HasValue || modulo.Estado.Value != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloContextoNoVigente;
                return respuesta;
            }

            modulo.Exitoso = true;
            modulo.Mensaje = string.Empty;

            return modulo;
        }

        public static RespuestaModelo<ModuloNavegacionModelo> GetModuloNavegacion(string urlReal, string urlPublica)
        {
            var respuesta = new RespuestaModelo<ModuloNavegacionModelo>
            {
                Exitoso = false,
                Datos = null
            };

            if (string.IsNullOrWhiteSpace(urlReal) && string.IsNullOrWhiteSpace(urlPublica))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionNoEncontrado;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            var operacion = 0;

            var modulos = ModuloDatos.GetModuloNavegacion(
                ref operacion,
                urlReal,
                urlPublica
            );

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionError;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            if (modulos == null || modulos.Count == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionNoEncontrado;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            if (modulos.Count > 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionDuplicado;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            var modulo = modulos.First();

            if (!modulo.Estado.HasValue || modulo.Estado.Value != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionNoVigente;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Mensaje = string.Empty;
            respuesta.AlertaColor = string.Empty;
            respuesta.AlertaTitulo = string.Empty;
            respuesta.Datos = modulo;

            return respuesta;
        }

        public static RespuestaModelo<ModuloNavegacionAccionModelo> GetModuloNavegacionAccion(uint moduloIdOrigen, string accion)
        {
            var respuesta = new RespuestaModelo<ModuloNavegacionAccionModelo>
            {
                Exitoso = false,
                Datos = null,
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty
            };

            if (moduloIdOrigen == 0 || string.IsNullOrWhiteSpace(accion))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionAccionNoEncontrada;
                return respuesta;
            }

            var operacion = 0;

            var navegacion = ModuloDatos.GetModuloNavegacionAccion(
                ref operacion,
                moduloIdOrigen,
                accion.Trim()
            );

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionAccionError;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                return respuesta;
            }

            if (navegacion == null || navegacion.NavegacionId == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionAccionNoEncontrada;
                return respuesta;
            }

            if (!navegacion.NavegacionEstado.HasValue || navegacion.NavegacionEstado.Value != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionAccionNoVigente;
                return respuesta;
            }

            if (!navegacion.ModuloEstado.HasValue || navegacion.ModuloEstado.Value != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionAccionDestinoNoVigente;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Mensaje = string.Empty;
            respuesta.AlertaColor = string.Empty;
            respuesta.Datos = navegacion;

            return respuesta;
        }

        public static RespuestaModelo<List<ModuloNavegacionConfigModelo>> GetModuloNavegacionConfig(uint moduloIdOrigen)
        {
            var respuesta = new RespuestaModelo<List<ModuloNavegacionConfigModelo>>
            {
                Exitoso = false,
                Datos = new List<ModuloNavegacionConfigModelo>(),
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty
            };

            if (moduloIdOrigen == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionOrigenNoIdentificado;
                return respuesta;
            }

            var operacion = 0;

            var navegaciones = ModuloDatos.GetModuloNavegacionConfig(
                ref operacion,
                moduloIdOrigen
            );

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorModuloNavegacionConfig;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Mensaje = string.Empty;
            respuesta.AlertaColor = string.Empty;
            respuesta.Datos = navegaciones ?? new List<ModuloNavegacionConfigModelo>();

            return respuesta;
        }

        public static RespuestaModelo<ModuloNavegacionConfigModelo> GuardarModuloNavegacion(ModuloNavegacionConfigModelo navegacion)
        {
            var respuesta = new RespuestaModelo<ModuloNavegacionConfigModelo>
            {
                Exitoso = false,
                Datos = navegacion,
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty
            };

            var validacion = ValidarModuloNavegacion(navegacion);

            if (!validacion.Exitoso)
            {
                respuesta.Mensaje = validacion.Mensaje;
                respuesta.AlertaColor = validacion.AlertaColor;
                respuesta.AlertaTitulo = validacion.AlertaTitulo;
                return respuesta;
            }

            var operacion = navegacion.Id > 0 ? ModuloDatos.ActualizarModuloNavegacion(navegacion) : ModuloDatos.RegistrarModuloNavegacion(navegacion);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorModuloNavegacionRegistrar;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Mensaje = ConstantesModelo.ConsExitoModuloNavegacionRegistrar;
            respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
            respuesta.AlertaTitulo = string.Empty;
            respuesta.Datos = navegacion;

            return respuesta;
        }

        private static RespuestaModelo<bool> ValidarModuloNavegacion(ModuloNavegacionConfigModelo navegacion)
        {
            var respuesta = new RespuestaModelo<bool>
            {
                Exitoso = false,
                Datos = false,
                AlertaColor = ConstantesModelo.ConsAlertaColorWarning,
                AlertaTitulo = string.Empty
            };

            if (navegacion == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionDatosNoObtenidos;
                return respuesta;
            }

            if (navegacion.ModuloIdOrigen == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionOrigenNoIdentificado;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(navegacion.Accion))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionAccionRequerida;
                return respuesta;
            }

            if (navegacion.Accion.Trim().Length > 30)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionAccionLargo;
                return respuesta;
            }

            if (navegacion.ModuloIdDestino == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionDestinoRequerido;
                return respuesta;
            }

            if (!navegacion.Estado.HasValue || (navegacion.Estado.Value != 0 && navegacion.Estado.Value != 1))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionEstadoInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(navegacion.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionModuloNavegacionUsuarioNoIdentificado;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = true;
            respuesta.Mensaje = string.Empty;
            respuesta.AlertaColor = string.Empty;
            respuesta.AlertaTitulo = string.Empty;

            return respuesta;
        }
    }
}