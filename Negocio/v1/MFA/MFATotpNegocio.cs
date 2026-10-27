using Datos.v1.MFA;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.MFA;
using System;
using System.Collections.Generic;

namespace Negocio.v1.MFA
{
    public class MFATotpNegocio
    {
        public static RespuestaModelo<MFATotpModelo> GetConfiguracion(uint? empresaUsuarioId, string rutSuperAdmin)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();
            var tieneEmpresaUsuario = empresaUsuarioId.HasValue && empresaUsuarioId.Value > 0;
            var tieneSuperAdmin = !string.IsNullOrWhiteSpace(rutSuperAdmin);

            if (tieneEmpresaUsuario == tieneSuperAdmin)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpPropietarioInvalido;
                return respuesta;
            }

            var rutSuperAdminNormalizado = tieneSuperAdmin ? rutSuperAdmin.Trim().ToUpperInvariant() : string.Empty;
            var operacion = 0;
            var configuracion = MFATotpDatos.GetMfaTotp(ref operacion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdminNormalizado : string.Empty);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = configuracion;
            respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
            respuesta.AlertaTitulo = string.Empty;
            return respuesta;
        }

        public static RespuestaModelo<MFATotpEnrolamientoModelo> IniciarConfiguracion(uint? empresaUsuarioId, string rutSuperAdmin, string nombreCuenta)
        {
            var respuesta = new RespuestaModelo<MFATotpEnrolamientoModelo>();
            var tieneEmpresaUsuario = empresaUsuarioId.HasValue && empresaUsuarioId.Value > 0;
            var tieneSuperAdmin = !string.IsNullOrWhiteSpace(rutSuperAdmin);

            if (tieneEmpresaUsuario == tieneSuperAdmin)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpPropietarioInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(nombreCuenta))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCuentaVacia;
                return respuesta;
            }

            var rutSuperAdminNormalizado = tieneSuperAdmin ? rutSuperAdmin.Trim().ToUpperInvariant() : string.Empty;
            var operacion = 0;
            var configuracion = MFATotpDatos.GetMfaTotp(ref operacion, empresaUsuarioId, rutSuperAdminNormalizado);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (configuracion != null && configuracion.Id > 0)
            {
                if (configuracion.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpConfiguracionNoVigente;
                    return respuesta;
                }

                if (configuracion.Configurado == ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpYaConfigurado;
                    return respuesta;
                }

                try
                {
                    respuesta.Datos = ConstruirEnrolamiento(configuracion, nombreCuenta.Trim());
                    respuesta.Exitoso = true;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoMfaTotpConfiguracionObtener;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;
                }
                catch
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpProteccion;
                    return respuesta;
                }
            }

            byte[] secreto = null;

            try
            {
                secreto = MFATotpHelperNegocio.GenerarSecreto();

                configuracion = new MFATotpModelo
                {
                    SecretProtegido = MFATotpHelperNegocio.ProtegerSecreto(secreto),
                    Algoritmo = ConstantesModelo.ConsMfaTotpAlgoritmo,
                    Digitos = ConstantesModelo.ConsMfaTotpDigitos,
                    PeriodoSegundos = ConstantesModelo.ConsMfaTotpPeriodoSegundos,
                    EmpresaUsuarioId = tieneEmpresaUsuario ? empresaUsuarioId : null,
                    RutSuperAdmin = tieneSuperAdmin ? rutSuperAdminNormalizado : string.Empty
                };

                var resultado = MFATotpDatos.InsertMfaTotp(configuracion, out uint mfaTotpId);

                if (resultado != 0 || mfaTotpId == 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
                }

                configuracion.Id = mfaTotpId;

                var secretoBase32 = MFATotpHelperNegocio.ConvertirSecretoBase32(secreto);
                var emisor = ObtenerEmisor();

                respuesta.Exitoso = true;
                respuesta.Datos = new MFATotpEnrolamientoModelo
                {
                    Id = configuracion.Id,
                    SecretBase32 = secretoBase32,
                    UriOtpAuth = MFATotpHelperNegocio.CrearUriOtpAuth(emisor, nombreCuenta.Trim(), secretoBase32, configuracion.Algoritmo, configuracion.Digitos, configuracion.PeriodoSegundos),
                    Algoritmo = configuracion.Algoritmo,
                    Digitos = configuracion.Digitos,
                    PeriodoSegundos = configuracion.PeriodoSegundos
                };
                respuesta.Mensaje = ConstantesModelo.ConsExitoMfaTotpConfiguracionIniciar;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpProteccion;
                return respuesta;
            }
            finally
            {
                if (secreto != null)
                {
                    Array.Clear(secreto, 0, secreto.Length);
                }
            }
        }

        public static RespuestaModelo<MFATotpModelo> ConfirmarConfiguracion(uint? empresaUsuarioId, string rutSuperAdmin, string codigo)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();

            rutSuperAdmin = rutSuperAdmin != null ? rutSuperAdmin.Trim().ToUpperInvariant() : string.Empty;
            codigo = codigo != null ? codigo.Trim() : string.Empty;

            var tieneEmpresaUsuario = empresaUsuarioId.HasValue && empresaUsuarioId.Value > 0;
            var tieneSuperAdmin = !string.IsNullOrWhiteSpace(rutSuperAdmin);

            if (tieneEmpresaUsuario == tieneSuperAdmin)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpPropietarioInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(codigo))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoVacio;
                return respuesta;
            }

            var operacion = 0;
            var configuracion = MFATotpDatos.GetMfaTotp(ref operacion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (configuracion == null || configuracion.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            if (configuracion.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpConfiguracionNoVigente;
                return respuesta;
            }

            if (configuracion.Configurado == ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpYaConfigurado;
                return respuesta;
            }

            if (configuracion.Configurado != ConstantesModelo.ConsMfaTotpConfiguracionPendiente)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            if (configuracion.BloqueadoHasta.HasValue && configuracion.BloqueadoHasta.Value > DateTime.Now)
            {
                respuesta.Mensaje = ObtenerMensajeBloqueo(configuracion.BloqueadoHasta.Value);
                return respuesta;
            }

            byte[] secreto = null;
            long pasoUsado;

            try
            {
                secreto = MFATotpHelperNegocio.DesprotegerSecreto(configuracion.SecretProtegido);
                var codigoValido = MFATotpHelperNegocio.ValidarCodigo(secreto, codigo, configuracion.PeriodoSegundos, configuracion.Digitos, out pasoUsado);

                if (!codigoValido)
                {
                    var resultadoIntentoFallido = MFATotpDatos.UpdateMFATotpIntentoFallido(configuracion.Id, ConstantesModelo.ConsMfaTotpIntentosMaximos, ConstantesModelo.ConsMfaTotpMinutosBloqueo);

                    if (resultadoIntentoFallido != 0)
                    {
                        respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                        return respuesta;
                    }

                    var operacionConfiguracionActualizada = 0;
                    var configuracionActualizada = MFATotpDatos.GetMfaTotp(ref operacionConfiguracionActualizada, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);

                    if (operacionConfiguracionActualizada != 0)
                    {
                        respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                        return respuesta;
                    }

                    if (configuracionActualizada == null || configuracionActualizada.Id == 0)
                    {
                        respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                        return respuesta;
                    }

                    if (configuracionActualizada.BloqueadoHasta.HasValue && configuracionActualizada.BloqueadoHasta.Value > DateTime.Now)
                    {
                        respuesta.Mensaje = ObtenerMensajeBloqueo(configuracionActualizada.BloqueadoHasta.Value);
                        return respuesta;
                    }

                    respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido;
                    return respuesta;
                }
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpProteccion;
                return respuesta;
            }
            finally
            {
                if (secreto != null)
                {
                    Array.Clear(secreto, 0, secreto.Length);
                }
            }

            if (configuracion.UltimoPasoUsado.HasValue && pasoUsado <= configuracion.UltimoPasoUsado.Value)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoReutilizado;
                return respuesta;
            }

            List<string> codigosRecuperacion;
            var codigosRecuperacionHash = new List<string>();

            try
            {
                codigosRecuperacion = MFATotpHelperNegocio.GenerarCodigosRecuperacion();

                foreach (var codigoRecuperacion in codigosRecuperacion)
                {
                    codigosRecuperacionHash.Add(MFATotpHelperNegocio.CalcularHashCodigoRecuperacion(codigoRecuperacion));
                }
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigoRecuperacionGenerar;
                return respuesta;
            }

            if (codigosRecuperacion.Count != ConstantesModelo.ConsMfaTotpCodigoRecuperacionCantidad || codigosRecuperacionHash.Count != ConstantesModelo.ConsMfaTotpCodigoRecuperacionCantidad)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigoRecuperacionGenerar;
                return respuesta;
            }

            var codigosHash = string.Join(ConstantesModelo.ConsMfaTotpCodigoRecuperacionListaSeparador, codigosRecuperacionHash);
            var resultado = MFATotpDatos.UpdateMFATotpConfirmar(configuracion.Id, pasoUsado, codigosHash);

            switch (resultado)
            {
                case 0:
                    var ahora = DateTime.Now;

                    configuracion.Configurado = ConstantesModelo.ConsMfaTotpConfiguracionConfirmada;
                    configuracion.IntentosFallidos = ConstantesModelo.ConsMfaTotpIntentosFallidosInicial;
                    configuracion.BloqueadoHasta = null;
                    configuracion.UltimoPasoUsado = pasoUsado;
                    configuracion.UpdatedAt = ahora;
                    configuracion.ConfirmedAt = ahora;
                    configuracion.LastUsedAt = ahora;

                    respuesta.Exitoso = true;
                    respuesta.Datos = ConstruirConfirmacion(configuracion, codigosRecuperacion);
                    respuesta.Mensaje = ConstantesModelo.ConsExitoMfaTotpConfiguracionConfirmar;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<MFATotpModelo> ValidarAcceso(uint? empresaUsuarioId, string rutSuperAdmin, string codigo)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();

            rutSuperAdmin = rutSuperAdmin != null ? rutSuperAdmin.Trim().ToUpperInvariant() : string.Empty;
            codigo = codigo != null ? codigo.Trim() : string.Empty;

            var tieneEmpresaUsuario = empresaUsuarioId.HasValue && empresaUsuarioId.Value > 0;
            var tieneSuperAdmin = !string.IsNullOrWhiteSpace(rutSuperAdmin);

            if (tieneEmpresaUsuario == tieneSuperAdmin)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpPropietarioInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(codigo))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoVacio;
                return respuesta;
            }

            var operacion = 0;
            var configuracion = MFATotpDatos.GetMfaTotp(ref operacion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (configuracion == null || configuracion.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            if (configuracion.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpConfiguracionNoVigente;
                return respuesta;
            }

            if (configuracion.Configurado != ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoConfirmada;
                return respuesta;
            }

            if (configuracion.BloqueadoHasta.HasValue && configuracion.BloqueadoHasta.Value > DateTime.Now)
            {
                respuesta.Mensaje = ObtenerMensajeBloqueo(configuracion.BloqueadoHasta.Value);
                return respuesta;
            }

            byte[] secreto = null;
            long pasoUsado;

            try
            {
                secreto = MFATotpHelperNegocio.DesprotegerSecreto(configuracion.SecretProtegido);
                var codigoValido = MFATotpHelperNegocio.ValidarCodigo(secreto, codigo, configuracion.PeriodoSegundos, configuracion.Digitos, out pasoUsado);

                if (!codigoValido)
                {
                    return RegistrarIntentoFallidoAcceso(configuracion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);
                }
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpProteccion;
                return respuesta;
            }
            finally
            {
                if (secreto != null)
                {
                    Array.Clear(secreto, 0, secreto.Length);
                }
            }

            if (configuracion.UltimoPasoUsado.HasValue && pasoUsado <= configuracion.UltimoPasoUsado.Value)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoReutilizado;
                return respuesta;
            }

            var resultado = MFATotpDatos.UpdateMFATotpValidacionExitosa(configuracion.Id, pasoUsado);

            switch (resultado)
            {
                case 0:
                    configuracion.IntentosFallidos = ConstantesModelo.ConsMfaTotpIntentosFallidosInicial;
                    configuracion.BloqueadoHasta = null;
                    configuracion.UltimoPasoUsado = pasoUsado;
                    configuracion.LastUsedAt = DateTime.Now;

                    respuesta.Exitoso = true;
                    respuesta.Datos = configuracion;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoMfaTotpValidacion;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpValidacion;
                    return respuesta;
            }
        }

        public static RespuestaModelo<MFATotpModelo> ValidarCodigoRecuperacion(uint? empresaUsuarioId, string rutSuperAdmin, string codigo)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();

            rutSuperAdmin = rutSuperAdmin != null ? rutSuperAdmin.Trim().ToUpperInvariant() : string.Empty;
            codigo = codigo != null ? codigo.Trim() : string.Empty;

            var tieneEmpresaUsuario = empresaUsuarioId.HasValue && empresaUsuarioId.Value > 0;
            var tieneSuperAdmin = !string.IsNullOrWhiteSpace(rutSuperAdmin);

            if (tieneEmpresaUsuario == tieneSuperAdmin)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpPropietarioInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(codigo))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoRecuperacionVacio;
                return respuesta;
            }

            var operacion = 0;
            var configuracion = MFATotpDatos.GetMfaTotp(ref operacion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (configuracion == null || configuracion.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            if (configuracion.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpConfiguracionNoVigente;
                return respuesta;
            }

            if (configuracion.Configurado != ConstantesModelo.ConsMfaTotpConfiguracionConfirmada)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoConfirmada;
                return respuesta;
            }

            if (configuracion.BloqueadoHasta.HasValue && configuracion.BloqueadoHasta.Value > DateTime.Now)
            {
                respuesta.Mensaje = ObtenerMensajeBloqueo(configuracion.BloqueadoHasta.Value);
                return respuesta;
            }

            string codigoHash;

            try
            {
                codigoHash = MFATotpHelperNegocio.CalcularHashCodigoRecuperacion(codigo);
            }
            catch (ArgumentException)
            {
                return RegistrarIntentoFallidoCodigoRecuperacion(configuracion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigoRecuperacionValidar;
                return respuesta;
            }

            var resultado = MFATotpDatos.UpdateMFACodigoRecuperacionUsar(configuracion.Id, codigoHash);

            switch (resultado)
            {
                case 0:
                    configuracion.IntentosFallidos = ConstantesModelo.ConsMfaTotpIntentosFallidosInicial;
                    configuracion.BloqueadoHasta = null;
                    configuracion.UpdatedAt = DateTime.Now;
                    configuracion.LastUsedAt = DateTime.Now;

                    respuesta.Exitoso = true;
                    respuesta.Datos = configuracion;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoMfaTotpCodigoRecuperacionValidar;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case 1:
                    return RegistrarIntentoFallidoCodigoRecuperacion(configuracion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigoRecuperacionValidar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<MFATotpModelo> RegenerarCodigosRecuperacion(uint? empresaUsuarioId, string rutSuperAdmin, string codigo, string rutUsuario)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();

            rutUsuario = rutUsuario != null ? rutUsuario.Trim().ToUpperInvariant() : string.Empty;

            if (string.IsNullOrWhiteSpace(rutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar;
                return respuesta;
            }

            var respuestaValidacion = ValidarAcceso(empresaUsuarioId, rutSuperAdmin, codigo);

            if (!respuestaValidacion.Exitoso)
            {
                return respuestaValidacion;
            }

            if (respuestaValidacion.Datos == null || respuestaValidacion.Datos.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            try
            {
                var codigosRecuperacion = MFATotpHelperNegocio.GenerarCodigosRecuperacion();

                if (codigosRecuperacion == null || codigosRecuperacion.Count != ConstantesModelo.ConsMfaTotpCodigoRecuperacionCantidad)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar;
                    return respuesta;
                }

                var codigosRecuperacionHash = new string[codigosRecuperacion.Count];

                for (var indice = 0; indice < codigosRecuperacion.Count; indice++)
                {
                    codigosRecuperacionHash[indice] = MFATotpHelperNegocio.CalcularHashCodigoRecuperacion(codigosRecuperacion[indice]);
                }

                var codigosHash = string.Join(ConstantesModelo.ConsMfaTotpCodigoRecuperacionListaSeparador, codigosRecuperacionHash);
                var resultado = MFATotpDatos.UpdateMFACodigoRecuperacionRegenerar(respuestaValidacion.Datos.Id, codigosHash, rutUsuario);

                switch (resultado)
                {
                    case 0:
                        respuesta.Exitoso = true;
                        respuesta.Datos = new MFATotpConfirmacionModelo
                        {
                            Id = respuestaValidacion.Datos.Id,
                            CodigosRecuperacion = codigosRecuperacion
                        };
                        respuesta.Mensaje = ConstantesModelo.ConsExitoMfaTotpCodigosRecuperacionRegenerar;
                        respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                        respuesta.AlertaTitulo = string.Empty;
                        return respuesta;

                    case 1:
                    case 2:
                        respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar;
                        return respuesta;

                    default:
                        respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                        return respuesta;
                }
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpCodigosRecuperacionRegenerar;
                return respuesta;
            }
        }

        private static MFATotpEnrolamientoModelo ConstruirEnrolamiento(MFATotpModelo configuracion, string nombreCuenta)
        {
            byte[] secreto = null;

            try
            {
                secreto = MFATotpHelperNegocio.DesprotegerSecreto(configuracion.SecretProtegido);

                var secretoBase32 = MFATotpHelperNegocio.ConvertirSecretoBase32(secreto);
                var emisor = ObtenerEmisor();

                return new MFATotpEnrolamientoModelo
                {
                    Id = configuracion.Id,
                    SecretBase32 = secretoBase32,
                    UriOtpAuth = MFATotpHelperNegocio.CrearUriOtpAuth(emisor, nombreCuenta, secretoBase32, configuracion.Algoritmo, configuracion.Digitos, configuracion.PeriodoSegundos),
                    Algoritmo = configuracion.Algoritmo,
                    Digitos = configuracion.Digitos,
                    PeriodoSegundos = configuracion.PeriodoSegundos
                };
            }
            finally
            {
                if (secreto != null)
                {
                    Array.Clear(secreto, 0, secreto.Length);
                }
            }
        }

        private static MFATotpConfirmacionModelo ConstruirConfirmacion(MFATotpModelo configuracion, List<string> codigosRecuperacion)
        {
            return new MFATotpConfirmacionModelo
            {
                Id = configuracion.Id,
                SecretProtegido = configuracion.SecretProtegido,
                Algoritmo = configuracion.Algoritmo,
                Digitos = configuracion.Digitos,
                PeriodoSegundos = configuracion.PeriodoSegundos,
                Configurado = configuracion.Configurado,
                Estado = configuracion.Estado,
                IntentosFallidos = configuracion.IntentosFallidos,
                BloqueadoHasta = configuracion.BloqueadoHasta,
                UltimoPasoUsado = configuracion.UltimoPasoUsado,
                CreatedAt = configuracion.CreatedAt,
                UpdatedAt = configuracion.UpdatedAt,
                ConfirmedAt = configuracion.ConfirmedAt,
                LastUsedAt = configuracion.LastUsedAt,
                EmpresaUsuarioId = configuracion.EmpresaUsuarioId,
                RutSuperAdmin = configuracion.RutSuperAdmin,
                CodigosRecuperacion = codigosRecuperacion
            };
        }

        private static string ObtenerEmisor()
        {
            if (!string.IsNullOrWhiteSpace(ConstantesModelo.ConsWebsiteNombre))
            {
                return ConstantesModelo.ConsWebsiteNombre.Trim();
            }

            return ConstantesModelo.ConsMfaTotpEmisorPredeterminado;
        }

        private static string ObtenerMensajeBloqueo(DateTime bloqueadoHasta)
        {
            var segundosRestantes = (int)Math.Ceiling((bloqueadoHasta - DateTime.Now).TotalSeconds);

            if (segundosRestantes <= 0)
            {
                return ConstantesModelo.ConsValidacionMfaTotpConfiguracionBloqueada;
            }

            var minutos = segundosRestantes / 60;
            var segundos = segundosRestantes % 60;

            return ConstantesModelo.ConsValidacionMfaTotpConfiguracionBloqueada + "<br>Tiempo restante: " + minutos.ToString("00") + ":" + segundos.ToString("00") + ".";
        }

        private static RespuestaModelo<MFATotpModelo> RegistrarIntentoFallidoAcceso(MFATotpModelo configuracion, uint? empresaUsuarioId, string rutSuperAdmin)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();
            var resultado = MFATotpDatos.UpdateMFATotpIntentoFallido(configuracion.Id, ConstantesModelo.ConsMfaTotpIntentosMaximos, ConstantesModelo.ConsMfaTotpMinutosBloqueo);

            if (resultado != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            var operacion = 0;
            var configuracionActualizada = MFATotpDatos.GetMfaTotp(ref operacion, empresaUsuarioId, rutSuperAdmin);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (configuracionActualizada == null || configuracionActualizada.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            if (configuracionActualizada.BloqueadoHasta.HasValue && configuracionActualizada.BloqueadoHasta.Value > DateTime.Now)
            {
                respuesta.Mensaje = ObtenerMensajeBloqueo(configuracionActualizada.BloqueadoHasta.Value);
                return respuesta;
            }

            respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoInvalido;
            return respuesta;
        }

        private static RespuestaModelo<MFATotpModelo> RegistrarIntentoFallidoCodigoRecuperacion(MFATotpModelo configuracion, uint? empresaUsuarioId, string rutSuperAdmin)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();
            var resultado = MFATotpDatos.UpdateMFATotpIntentoFallido(configuracion.Id, ConstantesModelo.ConsMfaTotpIntentosMaximos, ConstantesModelo.ConsMfaTotpMinutosBloqueo);

            if (resultado != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            var operacion = 0;
            var configuracionActualizada = MFATotpDatos.GetMfaTotp(ref operacion, empresaUsuarioId, rutSuperAdmin);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (configuracionActualizada == null || configuracionActualizada.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            if (configuracionActualizada.BloqueadoHasta.HasValue && configuracionActualizada.BloqueadoHasta.Value > DateTime.Now)
            {
                respuesta.Mensaje = ObtenerMensajeBloqueo(configuracionActualizada.BloqueadoHasta.Value);
                return respuesta;
            }

            respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpCodigoRecuperacionInvalido;
            return respuesta;
        }

        public static RespuestaModelo<MFATotpModelo> RestablecerConfiguracion(uint? empresaUsuarioId, string rutSuperAdmin, string rutUsuario)
        {
            var respuesta = new RespuestaModelo<MFATotpModelo>();

            rutSuperAdmin = rutSuperAdmin != null ? rutSuperAdmin.Trim().ToUpperInvariant() : string.Empty;
            rutUsuario = rutUsuario != null ? rutUsuario.Trim().ToUpperInvariant() : string.Empty;

            var tieneEmpresaUsuario = empresaUsuarioId.HasValue && empresaUsuarioId.Value > 0;
            var tieneSuperAdmin = !string.IsNullOrWhiteSpace(rutSuperAdmin);

            if (tieneEmpresaUsuario == tieneSuperAdmin)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpPropietarioInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(rutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpRestablecer;
                return respuesta;
            }

            if (tieneSuperAdmin && string.Equals(rutSuperAdmin, rutUsuario, StringComparison.OrdinalIgnoreCase))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpRestablecer;
                return respuesta;
            }

            var operacion = 0;
            var configuracion = MFATotpDatos.GetMfaTotp(ref operacion, tieneEmpresaUsuario ? empresaUsuarioId : null, tieneSuperAdmin ? rutSuperAdmin : string.Empty);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (configuracion == null || configuracion.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionMfaTotpConfiguracionNoEncontrada;
                return respuesta;
            }

            if (configuracion.Estado != ConstantesModelo.ConsMfaTotpEstadoVigente)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpConfiguracionNoVigente;
                return respuesta;
            }

            byte[] secreto = null;
            byte[] secretoProtegido = null;

            try
            {
                secreto = MFATotpHelperNegocio.GenerarSecreto();
                secretoProtegido = MFATotpHelperNegocio.ProtegerSecreto(secreto);

                if (secretoProtegido == null || secretoProtegido.Length == 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpRestablecer;
                    return respuesta;
                }

                var resultado = MFATotpDatos.UpdateMFATotpRestablecer(configuracion.Id, secretoProtegido, rutUsuario);

                switch (resultado)
                {
                    case 0:
                        configuracion.SecretProtegido = null;
                        configuracion.Configurado = ConstantesModelo.ConsMfaTotpConfiguracionPendiente;
                        configuracion.IntentosFallidos = ConstantesModelo.ConsMfaTotpIntentosFallidosInicial;
                        configuracion.BloqueadoHasta = null;
                        configuracion.UltimoPasoUsado = null;
                        configuracion.UpdatedAt = DateTime.Now;
                        configuracion.ConfirmedAt = null;
                        configuracion.LastUsedAt = null;

                        respuesta.Exitoso = true;
                        respuesta.Datos = configuracion;
                        respuesta.Mensaje = ConstantesModelo.ConsExitoMfaTotpRestablecer;
                        respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                        respuesta.AlertaTitulo = string.Empty;
                        return respuesta;

                    case 1:
                        respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpRestablecer;
                        return respuesta;

                    default:
                        respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                        return respuesta;
                }
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorMfaTotpRestablecer;
                return respuesta;
            }
            finally
            {
                if (secreto != null)
                {
                    Array.Clear(secreto, 0, secreto.Length);
                }

                if (secretoProtegido != null)
                {
                    Array.Clear(secretoProtegido, 0, secretoProtegido.Length);
                }
            }
        }
    }
}