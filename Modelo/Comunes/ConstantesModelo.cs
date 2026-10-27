using System;

namespace Modelo
{
    public class ConstantesModelo
    {
        // HTTP / RUTAS BASE
        public const string ConsHttp                                                    = @"https://";
        public const string ConsHttpInicio                                              = @"/";
        public const string ConsHttpAplicacion                                          = @"/aplicacion/";
        public const string ConsHttpRecuperar                                           = @"/recuperar/";
        public const string ConsHttpMFA                                                 = @"/mfa/";
        public const string ConsTituloWebPage                                           = @"DIPROTEC - Cedula Web";

        // VARIABLES DE ENTORNO - BASE DE DATOS / SITIO
        public static string ConsServidor                                               = Environment.GetEnvironmentVariable("EV-Cedula-Servidor-IP", EnvironmentVariableTarget.Machine);
        public static string ConsUsuario                                                = Environment.GetEnvironmentVariable("EV-Cedula-UsuarioRuntime-Nombre", EnvironmentVariableTarget.Machine);
        public static string ConsPassword                                               = Environment.GetEnvironmentVariable("EV-Cedula-UsuarioRuntime-Password", EnvironmentVariableTarget.Machine);
        public static string ConsBaseDatos                                              = Environment.GetEnvironmentVariable("EV-Cedula-Servidor-BD", EnvironmentVariableTarget.Machine);
        public static string ConsUrl                                                    = Environment.GetEnvironmentVariable("EV-Cedula-Website-Url", EnvironmentVariableTarget.Machine);
        public static string ConsWebsiteNombre                                          = Environment.GetEnvironmentVariable("EV-Cedula-Website-Nombre", EnvironmentVariableTarget.Machine);
        public static string ConsDownloadPath                                           = Environment.GetEnvironmentVariable("EV-Cedula-Website-Download-Path", EnvironmentVariableTarget.Machine);
        public static string ConsDownloadPathTemp                                       = Environment.GetEnvironmentVariable("EV-Cedula-Website-Download-Path-Temp", EnvironmentVariableTarget.Machine);
        public static string ConsPasswordHash                                           = Environment.GetEnvironmentVariable("EV-Cedula-Password-Hash", EnvironmentVariableTarget.Machine);
        public static string ConsPasswordSalt                                           = Environment.GetEnvironmentVariable("EV-Cedula-Password-Salt", EnvironmentVariableTarget.Machine);
        public static string ConsPasswordIterations                                     = Environment.GetEnvironmentVariable("EV-Cedula-Password-Iterations", EnvironmentVariableTarget.Machine);
        public static string ConsPasswordAlgoritmoId                                    = Environment.GetEnvironmentVariable("EV-Cedula-Password-AlgoritmoId", EnvironmentVariableTarget.Machine);
        public static string ConsCMFApiKey                                              = Environment.GetEnvironmentVariable("EV-CMF-Api-Key", EnvironmentVariableTarget.Machine);
        public static string ConsCMFApiUrl                                              = Environment.GetEnvironmentVariable("EV-CMF-Api-Url", EnvironmentVariableTarget.Machine);
        public static string ConsCMFIndicadorEconomico                                  = Environment.GetEnvironmentVariable("EV-CMF-Indicador-Economico", EnvironmentVariableTarget.Machine);

        // VARIABLES DE ENTORNO - RECAPTCHA
        public static string ReCaptchaScore                                             = Environment.GetEnvironmentVariable("EV-Cedula-ReCaptchaScore", EnvironmentVariableTarget.Machine);
        public static string ReCaptchaSecretKey_v3                                      = Environment.GetEnvironmentVariable("EV-Cedula-ReCaptchaSecretKey_v3", EnvironmentVariableTarget.Machine);
        public static string ReCaptchaSiteKey_v3                                        = Environment.GetEnvironmentVariable("EV-Cedula-ReCaptchaSiteKey_v3", EnvironmentVariableTarget.Machine);
        public static string ReCaptchaUrl                                               = Environment.GetEnvironmentVariable("EV-Cedula-ReCaptchaUrl", EnvironmentVariableTarget.Machine);

        // VARIABLES DE ENTORNO - ADMINISTRADOR RESPALDO
        public static string ConsAdministradorNombre                                    = Environment.GetEnvironmentVariable("EV-Administrador-Nombre", EnvironmentVariableTarget.Machine);
        public static string ConsAdministradorUsuario                                   = Environment.GetEnvironmentVariable("EV-Administrador-Usuario", EnvironmentVariableTarget.Machine);
        public static string ConsAdministradorPassword                                  = Environment.GetEnvironmentVariable("EV-Administrador-Password", EnvironmentVariableTarget.Machine);
        public static string ConsAdministradorPerfil                                    = Environment.GetEnvironmentVariable("EV-Administrador-Perfil", EnvironmentVariableTarget.Machine);

        // VARIABLES DE ENTORNO - EMAIL
        public static string ConsEmailPort                                              = Environment.GetEnvironmentVariable("EV-EmailPort", EnvironmentVariableTarget.Machine);
        public static string ConsEmailSmtp                                              = Environment.GetEnvironmentVariable("EV-EmailSmtp", EnvironmentVariableTarget.Machine);
        public static string ConsEmailUsuario                                           = Environment.GetEnvironmentVariable("EV-EmailUsuario", EnvironmentVariableTarget.Machine);
        public static string ConsEmailPassword                                          = Environment.GetEnvironmentVariable("EV-EmailPassword", EnvironmentVariableTarget.Machine);
        public static string ConsEmailEnvio                                             = Environment.GetEnvironmentVariable("EV-EmailEnvio", EnvironmentVariableTarget.Machine);
        public static string ConsEmailDestino                                           = Environment.GetEnvironmentVariable("EV-EmailDestino", EnvironmentVariableTarget.Machine);
        public static string ConsEmailSubjectRecuperacion                               = @"Recuperación Contraseña";
        public static string ConsEmailSubjectNuevoUsuario                               = @"Nuevo Usuario";

        // VARIABLES DE ENTORNO - COMMERCE API
        public static string ConsCommerceApiUrl                                         = Environment.GetEnvironmentVariable("EV-Commerce-WebApi-Url", EnvironmentVariableTarget.Machine);
        public static string ConsCommerceApiEPGetDispositivo                            = Environment.GetEnvironmentVariable("EV-Commerce-WebApi-EP-GetDispositivo", EnvironmentVariableTarget.Machine);
        public static string ConsCommerceApiHeaderKey                                   = Environment.GetEnvironmentVariable("EV-Commerce-Header-Key", EnvironmentVariableTarget.Machine);
        public static string ConsCommerceApiHeaderValue                                 = Environment.GetEnvironmentVariable("EV-Commerce-Header-Value", EnvironmentVariableTarget.Machine);

        // VARIABLES DE ENTORNO - JWT
        public static string ConsJwtIssuer                                              = Environment.GetEnvironmentVariable("EV-Cedula-JWTIssuer", EnvironmentVariableTarget.Machine);
        public static string ConsJwtAudience                                            = Environment.GetEnvironmentVariable("EV-Cedula-JWTAudience", EnvironmentVariableTarget.Machine);
        public static string ConsJwtPassword                                            = Environment.GetEnvironmentVariable("EV-Cedula-JWTPassword", EnvironmentVariableTarget.Machine);

        // VARIABLES DE ENTORNO - MFA TOTP
        public static string ConsMfaTotpClaveCifrado                                    = Environment.GetEnvironmentVariable("EV-Cedula-MFAEncryptionKey", EnvironmentVariableTarget.Machine);

        // CARACTERES BASE
        private const string ConsNumeros                                                = "0123456789";
        private const string ConsK                                                      = "kK";
        private const string ConsAbecedarioSinK                                         = "abcdefghijlmnñopqrstuvwxyzABCDEFGHIJLMNÑOPQRSTUVWXYZ";
        private const string ConsAbecedario                                             = ConsAbecedarioSinK + ConsK;
        private const string ConsAcentos                                                = "áéíóúÁÉÍÓÚüÜ";
        private const string ConsEspacio                                                = " ";
        private const string ConsSaltoLinea                                             = "\r\n";

        // CARACTERES ESPECIALES ATÓMICOS
        private const string ConsPunto                                                  = ".";
        private const string ConsDosPuntos                                              = ":";
        private const string ConsComa                                                   = ",";
        private const string ConsPuntoComa                                              = ";";
        private const string ConsGuion                                                  = "-";
        private const string ConsGuionBajo                                              = "_";
        private const string ConsBarra                                                  = "/";
        private const string ConsParentesis                                             = "()";
        private const string ConsApostrofeSimple                                        = "'";
        private const string ConsApostrofeTipografico                                   = "’";
        private const string ConsComillas                                               = "\"";
        private const string ConsExclamacion                                            = "!";
        private const string ConsExclamacionApertura                                    = "¡";
        private const string ConsInterrogacion                                          = "?";
        private const string ConsInterrogacionApertura                                  = "¿";
        private const string ConsNumeral                                                = "#";
        private const string ConsDolar                                                  = "$";
        private const string ConsPorcentaje                                             = "%";
        private const string ConsAmpersand                                              = "&";
        private const string ConsArroba                                                 = "@";
        private const string ConsMas                                                    = "+";
        private const string ConsIgual                                                  = "=";
        private const string ConsComparadores                                           = "<>";
        private const string ConsCorchetes                                              = "[]";
        private const string ConsAsterisco                                              = "*";
        private const string ConsCircunflejo                                            = "^";
        private const string ConsEmailExtendido                                         = "`{|}~";
        private const string ConsPuntoMedio                                             = "·";
        private const string ConsGrados                                                 = "°º";
        private const string ConsSuperindices                                           = "²³";
        private const string ConsMicro                                                  = "µμ";

        // COMBINACIONES BASE
        private const string ConsApostrofes                                             = ConsApostrofeSimple + ConsApostrofeTipografico;
        private const string ConsBaseAlfanumerica                                       = ConsAbecedario + ConsNumeros;
        private const string ConsBaseTextoAlfabetico                                    = ConsEspacio + ConsAcentos + ConsAbecedario;
        private const string ConsBaseTextoAlfanumerico                                  = ConsBaseTextoAlfabetico + ConsNumeros;
        private const string ConsBaseTextoAlfanumericoSinAcentos                        = ConsEspacio + ConsBaseAlfanumerica;

        // CONJUNTOS DE CARACTERES PERMITIDOS
        private const string ConsCaracteresPassword                                     = ConsExclamacion + ConsPuntoMedio + ConsNumeral + ConsDolar + ConsPorcentaje + ConsAmpersand + ConsBarra + ConsParentesis + ConsIgual + ConsInterrogacion + ConsInterrogacionApertura + ConsPuntoComa + ConsDosPuntos + ConsGuionBajo + ConsPunto + ConsComa + ConsGuion + ConsComparadores;
        private const string ConsCaracteresEmail                                        = ConsPunto + ConsExclamacion + ConsNumeral + ConsDolar + ConsPorcentaje + ConsAmpersand + ConsApostrofeSimple + ConsAsterisco + ConsMas + ConsBarra + ConsIgual + ConsInterrogacion + ConsCircunflejo + ConsGuionBajo + ConsEmailExtendido + ConsArroba + ConsGuion;
        private const string ConsCaracteresBusquedaUsuario                              = ConsPunto + ConsGuionBajo + ConsArroba + ConsMas;
        private const string ConsCaracteresNombre                                       = ConsPunto + ConsGuion + ConsApostrofes + ConsParentesis + ConsComa + ConsBarra;
        private const string ConsCaracteresNombreComercial                              = ConsCaracteresNombre + ConsAmpersand + ConsMas;
        private const string ConsCaracteresDireccion                                    = ConsPunto + ConsDosPuntos + ConsGuion + ConsApostrofes + ConsComa + ConsNumeral + ConsBarra + ConsGrados + ConsParentesis + ConsAmpersand;
        private const string ConsCaracteresTextoLibre                                   = ConsPunto + ConsDosPuntos + ConsGuion + ConsApostrofes + ConsExclamacion + ConsExclamacionApertura + ConsInterrogacion + ConsInterrogacionApertura + ConsComa + ConsPuntoComa + ConsGuionBajo + ConsBarra + ConsNumeral + ConsPorcentaje + ConsAmpersand + ConsParentesis + ConsArroba + ConsMas;
        private const string ConsCaracteresVersionAplicacion                            = ConsPunto + ConsGuion + ConsGuionBajo + ConsMas;
        private const string ConsCaracteresNombreCategoria                              = ConsPunto + ConsDosPuntos + ConsGuion + ConsApostrofes + ConsAmpersand + ConsParentesis + ConsComa + ConsBarra;
        private const string ConsCaracteresIconoCategoria                               = ConsGuion + ConsGuionBajo;
        private const string ConsCaracteresCodigoTecnico                                = ConsPunto + ConsGuion + ConsGuionBajo + ConsBarra + ConsMas + ConsNumeral;
        private const string ConsCaracteresBusquedaDispositivo                          = ConsCaracteresCodigoTecnico + ConsApostrofes + ConsAmpersand + ConsParentesis + ConsComa;
        private const string ConsCaracteresNombreModeloDispositivo                      = ConsPunto + ConsGuion + ConsGuionBajo + ConsMas + ConsBarra + ConsParentesis + ConsNumeral + ConsApostrofes + ConsAmpersand + ConsComa;
        private const string ConsCaracteresBusquedaLogAcceso                            = ConsPunto + ConsDosPuntos + ConsGuion + ConsGuionBajo + ConsBarra + ConsMas + ConsNumeral + ConsApostrofes + ConsAmpersand + ConsParentesis + ConsComa + ConsArroba + ConsPorcentaje + ConsCorchetes;
        private const string ConsCaracteresTextoModulo                                  = ConsPunto + ConsDosPuntos + ConsGuion + ConsApostrofes + ConsExclamacion + ConsExclamacionApertura + ConsInterrogacion + ConsInterrogacionApertura + ConsComa + ConsPuntoComa + ConsBarra + ConsAmpersand + ConsParentesis + ConsMas + ConsPorcentaje + ConsNumeral + ConsGrados;
        private const string ConsCaracteresUrlModulo                                    = ConsBarra + ConsPunto + ConsGuionBajo + ConsGuion + ConsInterrogacion + ConsIgual + ConsAmpersand;
        private const string ConsCaracteresBusquedaModulo                               = ConsCaracteresTextoModulo + ConsGuionBajo + ConsIgual;
        private const string ConsCaracteresFabricanteVersionAplicacion                  = ConsGuion + ConsApostrofes + ConsPunto + ConsAmpersand + ConsParentesis + ConsComa + ConsMas;
        private const string ConsCaracteresModeloVersionAplicacion                      = ConsPunto + ConsGuion + ConsGuionBajo + ConsBarra + ConsMas + ConsParentesis;
        private const string ConsCaracteresIdentificadorApkVersionAplicacion            = ConsGuion + ConsGuionBajo;
        private const string ConsCaracteresBusquedaVersionAplicacion                    = ConsPunto + ConsDosPuntos + ConsGuion + ConsGuionBajo + ConsBarra + ConsMas + ConsParentesis + ConsAmpersand + ConsApostrofes;

        // LOGIN
        public const string ConsLoginBotonIngresando                                    = @"Ingresando...";
        public const string ConsLoginBotonIniciarSesion                                 = @"Iniciar Sesión";

        // ALERTA COLORES
        public const string ConsAlertaColorDanger                                       = @"danger";
        public const string ConsAlertaColorSuccess                                      = @"success";
        public const string ConsAlertaColorWarning                                      = @"warning";
        public const string ConsAlertaColorError                                        = @"error";

        // USUARIO
        public const string ConsTextoRut                                                = ConsNumeros + ConsK;
        public const string ConsTextoPassword                                           = ConsBaseAlfanumerica + ConsCaracteresPassword;
        public const string ConsTextoEmail                                              = ConsBaseAlfanumerica + ConsCaracteresEmail;
        public const string ConsTextoNombrePersona                                      = ConsBaseTextoAlfabetico + ConsGuion + ConsApostrofes;
        public const string ConsTextoTelefono                                           = ConsNumeros;
        public const string ConsTextoBusquedaUsuario                                    = ConsBaseTextoAlfanumerico + ConsGuion + ConsApostrofes + ConsCaracteresBusquedaUsuario;
        public const string ConsExpresionRegularEmail                                   = @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)$";

        // EMPRESA
        public const string ConsTextoNombreEmpresa                                      = ConsBaseTextoAlfanumerico + ConsCaracteresNombreComercial;
        public const string ConsTextoDireccion                                          = ConsBaseTextoAlfanumerico + ConsCaracteresDireccion;
        public const string ConsTextoNombreLugar                                        = ConsTextoNombrePersona;
        public const string ConsTextoBusquedaEmpresa                                    = ConsTextoNombreEmpresa;
        public const string ConsTextoObservacion                                        = ConsBaseTextoAlfanumerico + ConsCaracteresTextoLibre + ConsSaltoLinea;

        // AVISO DE PLATAFORMA
        public const string ConsTextoTituloAvisoPlataforma                              = ConsBaseTextoAlfanumerico + ConsCaracteresTextoLibre;
        public const string ConsTextoMensajeAvisoPlataforma                             = ConsTextoTituloAvisoPlataforma + ConsSaltoLinea;
        public const string ConsTextoVersionAvisoPlataforma                             = ConsBaseTextoAlfanumericoSinAcentos + ConsCaracteresVersionAplicacion;
        public const string ConsTextoBusquedaAvisoPlataforma                            = ConsTextoTituloAvisoPlataforma;

        // CATEGORÍA
        public const string ConsTextoNombreCategoria                                    = ConsBaseTextoAlfanumerico + ConsCaracteresNombreCategoria;
        public const string ConsTextoIconoCategoria                                     = ConsBaseTextoAlfanumericoSinAcentos + ConsCaracteresIconoCategoria;
        public const string ConsTextoBusquedaCategoria                                  = ConsTextoNombreCategoria;

        // DISPOSITIVO
        public const string ConsTextoPartNumber                                         = ConsBaseAlfanumerica + ConsCaracteresCodigoTecnico;
        public const string ConsTextoSerialNumber                                       = ConsTextoPartNumber;
        public const string ConsTextoComentarioDispositivo                              = ConsTextoObservacion;
        public const string ConsTextoBusquedaDispositivo                                = ConsBaseTextoAlfanumerico + ConsCaracteresBusquedaDispositivo;

        // MARCA DE DISPOSITIVO
        public const string ConsTextoNombreMarcaDispositivo                             = ConsBaseTextoAlfanumerico + ConsCaracteresNombreComercial;
        public const string ConsTextoBusquedaMarcaDispositivo                           = ConsTextoNombreMarcaDispositivo;

        // MODELO DE DISPOSITIVO
        public const string ConsTextoNombreModeloDispositivo                            = ConsBaseTextoAlfanumerico + ConsCaracteresNombreModeloDispositivo;
        public const string ConsTextoBusquedaModeloDispositivo                          = ConsTextoNombreModeloDispositivo;

        // FAQ
        public const string ConsTextoBusquedaFaq                                        = ConsBaseTextoAlfanumerico + ConsGuion + ConsApostrofes;

        // LOG DE ACCESO
        public const string ConsTextoBusquedaLogAcceso                                  = ConsBaseTextoAlfanumerico + ConsCaracteresBusquedaLogAcceso;

        // MÓDULO
        public const string ConsTextoNombreModulo                                       = ConsBaseTextoAlfanumerico + ConsCaracteresTextoModulo;
        public const string ConsTextoTituloModulo                                       = ConsTextoNombreModulo;
        public const string ConsTextoUrlModulo                                          = ConsBaseAlfanumerica + ConsCaracteresUrlModulo;
        public const string ConsTextoBusquedaModulo                                     = ConsBaseTextoAlfanumerico + ConsCaracteresBusquedaModulo;

        // PERFIL
        public const string ConsTextoNombrePerfil                                       = ConsTextoNombrePersona;
        public const string ConsTextoBusquedaPerfil                                     = ConsTextoNombrePerfil;

        // REGLA
        public const string ConsTextoNombreRegla                                        = ConsTextoNombrePersona;
        public const string ConsTextoBusquedaRegla                                      = ConsTextoNombreRegla;

        // VISITA
        public const string ConsTextoBusquedaVisita                                     = ConsBaseTextoAlfanumerico + ConsCaracteresBusquedaUsuario + ConsGuion;
        public const string ConsTextoBusquedaCargaVisita                                = ConsTextoBusquedaVisita;
        public const string ConsTextoBusquedaRegistroVisita                             = ConsTextoBusquedaVisita;
        public const string ConsTextoBusquedaDetalleCargaVisita                         = ConsTextoNombreEmpresa + ConsDosPuntos;

        // VERSIÓN DE APLICACIÓN
        public const string ConsTextoFabricanteVersionAplicacion                        = ConsBaseTextoAlfanumerico + ConsCaracteresFabricanteVersionAplicacion;
        public const string ConsTextoModeloVersionAplicacion                            = ConsBaseTextoAlfanumerico + ConsCaracteresModeloVersionAplicacion;
        public const string ConsTextoIdentificadorApkVersionAplicacion                  = ConsBaseAlfanumerica + ConsCaracteresIdentificadorApkVersionAplicacion;
        public const string ConsTextoComentariosVersionAplicacion                       = ConsTextoObservacion;
        public const string ConsTextoBusquedaVersionAplicacion                          = ConsBaseTextoAlfanumerico + ConsCaracteresBusquedaVersionAplicacion;

        // EMPRESA TOKEN
        public static string ConsEmpresaTokenAplicacion                                 = Environment.GetEnvironmentVariable("EV-Cedula-Nombre", EnvironmentVariableTarget.Machine);
        public const string ConsEmpresaTokenClaimAplicacion                             = @"app";
        public const string ConsEmpresaTokenEstadoSinVigente                            = @"La empresa no tiene un token vigente para la API de Cédula.";
        public const string ConsEmpresaTokenEstadoVigente                               = @"El token vigente expira el {0}. Restan {1} días.";
        public const string ConsEmpresaTokenEstadoProximoVencer                         = @"El token expira el {0}. Restan {1} días; debe renovarlo antes del vencimiento.";
        public const string ConsEmpresaTokenEstadoVenceHoy                              = @"El token vigente expira hoy.";
        public const string ConsEmpresaTokenEstadoConvivencia                           = @"Existen dos tokens vigentes durante el período de convivencia. El token anterior dejará de operar al alcanzar su fecha de vencimiento.";
        public const string ConsEmpresaTokenEstadoLegacy                                = @"La empresa mantiene un token antiguo sin fecha de vencimiento. Debe renovarlo para aplicar la vigencia anual.";
        public const string ConsEmpresaTokenModalTitulo                                 = @"Token generado";
        public const string ConsEmpresaTokenModalDescripcion                            = @"Este token debe ser entregado al cliente para consumir la API de Cédula.";
        public const string ConsEmpresaTokenModalAdvertencia                            = @"El token se mostrará solo una vez. Debe copiarlo y almacenarlo de forma segura antes de cerrar esta ventana.";
        public const string ConsEmpresaTokenAdvertenciaGeneracionInicial                = @"La empresa fue registrada correctamente, pero no fue posible generar el token. Puede generarlo desde el listado de empresas.";

        // MFA TOTP
        public const string ConsMfaTotpUriOtpAuthPrefijo                                = @"otpauth://totp/";
        public const string ConsMfaTotpUriSeparadorEtiqueta                             = @":";
        public const string ConsMfaTotpUriInicioParametros                              = @"?";
        public const string ConsMfaTotpUriSeparadorParametros                           = @"&";
        public const string ConsMfaTotpUriSeparadorValor                                = @"=";
        public const string ConsMfaTotpUriParametroSecret                               = @"secret";
        public const string ConsMfaTotpUriParametroIssuer                               = @"issuer";
        public const string ConsMfaTotpUriParametroAlgorithm                            = @"algorithm";
        public const string ConsMfaTotpUriParametroDigits                               = @"digits";
        public const string ConsMfaTotpUriParametroPeriod                               = @"period";
        public const string ConsMfaTotpSessionLoginPendiente                            = @"MFALoginPendiente";
        public const string ConsMfaTotpAlgoritmo                                        = @"SHA1";
        public const string ConsMfaTotpEmisorPredeterminado                             = @"DIPROTEC Cedula Web";
        public const string ConsMfaTotpCuentaTipoUsuario                                = @"Usuario";
        public const string ConsMfaTotpCuentaTipoSuperAdmin                             = @"SuperAdmin";

        public const string ConsMfaTotpCuentaSeparador                                  = @"@";
        public const string ConsMfaTotpQrDataUriPrefijo                                 = @"data:image/png;base64,";
        public const string ConsMfaTotpCodigoPatron                                     = @"[0-9]{6}";
        public const string ConsMfaTotpCodigoRecuperacionCaracteres                     = @"ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        public const string ConsMfaTotpCodigoRecuperacionSeparador                      = @"-";
        public const string ConsMfaTotpCodigoRecuperacionEspacio                        = ConsEspacio;
        public const string ConsMfaTotpCodigoRecuperacionListaSeparador                 = @",";
        public const string ConsMfaTotpCodigoRecuperacionPatron                         = @"[A-HJ-NP-Z2-9]{4}-?[A-HJ-NP-Z2-9]{4}";

        public const byte ConsMfaTotpVersionProteccion                                  = 1;
        public const byte ConsMfaTotpDigitos                                            = 6;

        public const int ConsMfaTotpCodigoLargo                                         = 6;
        public const int ConsMfaTotpQrPixelesPorModulo                                  = 8;
        public const int ConsMfaTotpLargoClaveMaestra                                   = 64;
        public const int ConsMfaTotpLargoClaveCifrado                                   = 32;
        public const int ConsMfaTotpLargoClaveIntegridad                                = 32;
        public const int ConsMfaTotpLargoIv                                             = 16;
        public const int ConsMfaTotpLargoHashIntegridad                                 = 32;
        public const int ConsMfaTotpLargoSecretoTotp                                    = 20;
        public const int ConsMfaTotpLargoBloqueCifradoMinimo                            = 16;
        public const int ConsMfaTotpValorInicialDiferencia                              = 0;
        public const int ConsMfaTotpMinutosLoginPendiente                               = 10;
        public const int ConsMfaTotpAesKeySizeBits                                      = 256;
        public const int ConsMfaTotpAesBlockSizeBits                                    = 128;
        public const int ConsMfaTotpCodigoRecuperacionCaracteresCantidad                = 32;

        public const long ConsMfaTotpPasoTotpInicial                                    = 0;

        public const sbyte ConsMfaTotpEstadoVigente                                     = 0;
        public const sbyte ConsMfaTotpConfiguracionConfirmada                           = 0;
        public const sbyte ConsMfaTotpConfiguracionPendiente                            = 1;
        public const sbyte ConsMfaTotpRequerido                                         = 0;
        public const sbyte ConsMfaTotpNoRequerido                                       = 1;
        public const sbyte ConsMfaTotpCodigoRecuperacionEstadoDisponible                = 0;
        public const sbyte ConsMfaTotpCodigoRecuperacionEstadoNoDisponible              = 1;

        public const ushort ConsMfaTotpIntentosFallidosInicial                          = 0;
        public const ushort ConsMfaTotpIntentosMaximos                                  = 5;
        public const ushort ConsMfaTotpMinutosBloqueo                                   = 15;
        public const ushort ConsMfaTotpPeriodoSegundos                                  = 30;

        public const ushort ConsMfaTotpCodigoRecuperacionCantidad                       = 10;
        public const ushort ConsMfaTotpCodigoRecuperacionLargo                          = 8;
        public const ushort ConsMfaTotpCodigoRecuperacionBloqueLargo                    = 4;
        public const ushort ConsMfaTotpCodigoRecuperacionHashHexLargo                   = 64;

        // ERRORES - GENERICOS
        public const string ConsErrorPermisoAccesoModulo                                = @"No tiene permisos para acceder a este módulo.";
        public const string ConsErrorPermisoRegistrarRegistro                           = @"No tiene permisos para ingresar nuevos registros.";
        public const string ConsErrorPermisoModificarRegistro                           = @"No tiene permisos para modificar el registro seleccionado.";
        public const string ConsValidacionRegistroIdInvalido                            = @"No fue posible identificar el registro seleccionado.";
        public const string ConsErrorRegistroObtener                                    = @"No fue posible cargar los datos del registro seleccionado.";
        public const string ConsValidacionRegistroEstadoInvalido                        = @"No fue posible determinar el estado del registro seleccionado.";
        public const string ConsValidacionFechaHastaMayorFechaActual                    = @"La fecha hasta no puede ser mayor a la fecha actual.";
        public const string ConsValidacionPerfilSeleccionVacio                          = @"Debe seleccionar un perfil.";
        public const string ConsValidacionCategoriaSeleccionVacia                       = @"Debe seleccionar una categoría.";
        public const string ConsValidacionAccionInvalida                                = @"No fue posible identificar la acción seleccionada.";

        // ESTADOS CARGA
        public const sbyte ConsCargaEstadoFinalizada                                    = 0;
        public const sbyte ConsCargaEstadoAnulada                                       = 1;
        public const sbyte ConsCargaEstadoProcesando                                    = 2;
        public const sbyte ConsCargaEstadoFinalizadaConErrores                          = 3;
        public const sbyte ConsCargaEstadoFallida                                       = 4;

        public const sbyte ConsCargaDetalleEstadoRegistrado                             = 0;
        public const sbyte ConsCargaDetalleEstadoError                                  = 1;

        public const sbyte ConsCargaDetalleAccionSinAplicar                             = 0;
        public const sbyte ConsCargaDetalleAccionInsertada                              = 1;
        public const sbyte ConsCargaDetalleAccionActualizada                            = 2;
        public const sbyte ConsCargaDetalleAccionSinCambios                             = 3;

        public const sbyte ConsFiltroEstadoTodos                                        = 9;

        // MODULOS - BASE
        public const string ConsModuloUrl                                               = @"/aplicacion/modulo/";
        public const string ConsModulosDefault                                          = @"/aplicacion/";
        public const string ConsModulosFaq                                              = @"/aplicacion/faq/frmFaq.aspx";
        public const string ConsModulosBitacoraTitulo                                   = @"Bitácora de cambios";
        public const string ConsModulosAccionUrlNuevo                                   = @"NUEVO";
        public const string ConsModulosAccionUrlEditar                                  = @"EDITAR";
        public const string ConsModulosAccionUrlDetalle                                 = @"DETALLE";
        public const string ConsModulosAccionUrlVolver                                  = @"VOLVER";

        // MODULOS - DISPOSITIVOS
        public const string ConsModulosDispositivosTituloListadoVacio                   = @"No existen dispositivos para mostrar.";

        // MODULOS - EMPRESAS
        public const string ConsModulosEmpresasTituloListadoVacio                       = @"No existen empresas para mostrar.";

        // MODULOS - MARCAS
        public const string ConsModulosDispositivoMarcasTituloListadoVacio              = @"No existen marcas para mostrar.";

        // MODULOS - MODELOS
        public const string ConsModulosDispositivoModelosTituloListadoVacio             = @"No existen modelos para mostrar.";

        // MODULOS - PERFILES
        public const string ConsModulosPerfilesTituloListadoVacio                       = @"No existen perfiles para mostrar.";

        // MODULOS - REGLAS
        public const string ConsModulosReglasTituloListadoVacio                         = @"No existen reglas para mostrar.";

        // MODULOS - USUARIOS
        public const string ConsModulosUsuariosTituloListadoVacio                       = @"No existen usuarios para mostrar.";

        // MODULOS - SUPER ADMINISTRADORES
        public const string ConsModulosSuperAdministradoresTituloListadoVacio           = @"No existen super administradores para mostrar.";

        // MODULOS - VERSIONES APLICACION
        public const string ConsModulosVersionesAplicacionTituloListadoVacio            = @"No existen versiones de aplicaci&oacute;n para mostrar.";

        // MODULOS - VISITAS
        public const string ConsModulosVisitasTituloListadoVacio                        = @"No existen visitas para mostrar.";

        // MODULOS - REGISTRO VISITAS
        public const string ConsModulosRegistroVisitasTituloListadoVacio                = @"No existen registros de visitas para mostrar.";

        // MODULOS - CARGAR VISITAS
        public const string ConsModulosVisitasCargaTituloListadoVacio                   = @"No existen cargas de visitas para mostrar.";

        // MODULOS - LOGS
        public const string ConsModulosLogAccesoWebTituloListadoVacio                   = @"No existen accesos web para mostrar.";
        public const string ConsModulosLogAccesoDispositivoTituloListadoVacio           = @"No existen accesos dispositivos para mostrar.";

        // MODULOS - MODULOS X PERFIL
        public const string ConsModulosModulosPerfilTituloListadoVacio                  = @"No existen m&oacute;dulos x perfil para mostrar.";

        // MODULOS - CATEGORIAS
        public const string ConsModulosCategoriasTituloListadoVacio                     = @"No existen categor&iacute;as para mostrar.";

        // MODULOS - MODULOS
        public const string ConsModulosModulosTituloListadoVacio                        = @"No existen m&oacute;dulos para mostrar.";

        // MODULOS - AVISOS PLATAFORMA
        public const string ConsModulosAvisosPlataformaTituloListadoVacio               = @"No existen avisos para mostrar.";

        // ERRORES - GENERALES
        public const string ConsErrorBaseDatos                                          = @"Base De Datos inaccesible.";
        public const string ConsErrorGenericoConsultar                                  = @"Se produjo un error al consultar informaci&oacute;n.";
        public const string ConsErrorGenericoActualizar                                 = @"Se produjo un error al actualizar informaci&oacute;n.";
        public const string ConsErrorGenericoRegistrar                                  = @"Se produjo un error al registrar informaci&oacute;n.";
        public const string ConsErrorSesion                                             = @"Ha finalizado su sesión.";
        public const string ConsErrorLogin                                              = @"RUT empresa, RUT usuario y/o contraseña incorrectos.";
        public const string ConsErrorRecuperar                                          = @"RUT empresa, RUT usuario y/o email incorrectos.";
        public const string ConsErrorRecuperarEnvioMail                                 = @"No se pudo enviar email con la nueva contrase&ntilde;a.";
        public const string ConsAdvertenciaNuevoUsuarioEnvioMail                        = @"El usuario fue registrado exitosamente, pero no fue posible enviar el correo con las credenciales de acceso.";
        public const string ConsErrorIndicadoresEconomicosObtener                       = @"No fue posible obtener los indicadores económicos.";
        public const string ConsErrorNoticiasObtener                                    = @"No fue posible obtener noticias relevantes.";
        public const string ConsErrorPermisoModificarVigencia                           = @"Sin permiso para modificar vigencia";
        public const string ConsAdvertenciaRegistroModificado                           = @"El registro fue modificado por otro usuario o desde otra pestaña. Recargue la informaci&oacute;n antes de volver a actualizar.";

        // ERRORES - BITACORA
        public const string ConsErrorBitacoraObtener                                    = @"No fue posible obtener la bitácora.";

        // ERRORES - CAPTCHA
        public const string ConsErrorCaptchaSuccess                                     = @"No pudimos verificar su identidad. Int&eacute;ntelo nuevamente.";
        public const string ConsErrorCaptchaResponse                                    = @"Validaci&oacute;n reCAPTCHA incorrecta. Int&eacute;ntelo nuevamente.";
        public const string ConsErrorCaptchaScore                                       = @"Detectamos una actividad inusual. Int&eacute;ntelo nuevamente.";
        public const string ConsErrorCaptcha                                            = @"Hubo un problema al validar su solicitud. Int&eacute;ntelo nuevamente.";

        // ERRORES - SESION
        public const string ConsErrorSesionEmpresa                                      = @"No fue posible identificar la empresa de la sesión.";
        public const string ConsErrorSesionUsuario                                      = @"No fue posible identificar el usuario de la sesión.";

        // ERRORES - EMPRESA
        public const string ConsErrorRutEmpresa                                         = @"RUT incorrecto.";
        public const string ConsErrorEmpresaEstado                                      = @"Empresa deshabilitada.";
        public const string ConsErrorEmpresaSuspendida                                  = @"No es posible iniciar sesión porque la cuenta empresa se encuentra suspendida temporalmente.";
        public const string ConsErrorEmpresaExiste                                      = @"Empresa ya se encuentra registrada.";
        public const string ConsErrorEmpresaContactoEmail                               = @"Email incorrecto.";

        // ERRORES - USUARIO
        public const string ConsErrorUsuarioEstado                                      = @"Usuario deshabilitado.";
        public const string ConsErrorRutUsuario                                         = @"RUT incorrecto.";
        public const string ConsErrorUsuarioEmail                                       = @"Email incorrecto.";
        public const string ConsErrorUsuarioPassword                                    = @"Contraseña incorrecta.";
        public const string ConsErrorUsuarioExiste                                      = @"Usuario ya se encuentra registrado.";
        public const string ConsErrorUsuarioSuperAdministradorSinFichaEditable          = @"El administrador del sistema no tiene ficha de usuario editable.";

        // ERRORES - SUPER ADMINISTRADOR
        public const string ConsErrorSuperAdministradorEstado                           = @"Super administrador deshabilitado.";
        public const string ConsErrorRutSuperAdministrador                              = @"RUT incorrecto.";
        public const string ConsErrorSuperAdministradorEmail                            = @"Email incorrecto.";
        public const string ConsErrorSuperAdministradorPassword                         = @"Contraseña incorrecta.";
        public const string ConsErrorSuperAdministradorExiste                           = @"Super administrador ya se encuentra registrado.";

        // ERRORES - VISITA
        public const string ConsErrorVisitaRutDuplicado                                 = @"RUT ya se encuentra registrado en otra visita.";

        // ERRORES - VISITA CARGA
        public const string ConsErrorVisitaCargaRegistro                                = @"No fue posible procesar la carga masiva de visitas.";
        public const string ConsErrorVisitaCargaEncabezadoInvalido                      = @"El archivo CSV no contiene el encabezado esperado: RUT, nombre y empresa.";
        public const string ConsErrorVisitaCargaProcesamiento                           = @"La carga falló durante el procesamiento del archivo.";
        public const string ConsErrorVisitaCargaConsultarVisita                         = @"No fue posible consultar la visita.";
        public const string ConsErrorVisitaCargaRegistrarVisita                         = @"No fue posible registrar la visita.";
        public const string ConsErrorVisitaCargaActualizarVisita                        = @"No fue posible actualizar la visita.";

        // ERRORES - DISPOSITIVO
        public const string ConsValidacionDispositivoEstadoInvalido                     = @"No fue posible determinar el estado del dispositivo.";

        // ERRORES - DISPOSITIVO MODELO
        public const string ConsErrorDispositivoModeloFiltrosObtener                    = @"No fue posible obtener los filtros de búsqueda.";
        public const string ConsErrorDispositivoModeloListadoObtener                    = @"Ocurrió un error al obtener el listado de modelos.";
        public const string ConsErrorDispositivoModeloObtener                           = @"No fue posible obtener los datos del modelo.";
        public const string ConsErrorDispositivoModeloRegistro                          = @"Ocurrió un error al registrar el modelo.";
        public const string ConsErrorDispositivoModeloActualizar                        = @"Ocurrió un error al actualizar el modelo.";
        public const string ConsErrorDispositivoModeloNoVigente                         = @"El modelo seleccionado no se encuentra vigente o no existe.";

        // ERRORES - MODULO PERFIL
        public const string ConsErrorModuloPerfilEmpresaUsuario                         = @"No fue posible identificar la empresa del usuario.";
        public const string ConsErrorModuloPerfilUsuario                                = @"No fue posible identificar el usuario.";
        public const string ConsErrorModuloPerfilModulo                                 = @"No fue posible identificar el módulo solicitado.";

        // ERRORES - REPORTE
        public const string ConsErrorReporteExportar                                    = @"Ocurrió un error al generar la información para exportar.";

        // ERRORES - AVISO PLATAFORMA
        public const string ConsErrorAvisoPlataformaFechaTerminoVencida                 = @"El aviso no puede ser modificado porque su fecha de t&eacute;rmino ya se encuentra vencida.";
        public const string ConsErrorAvisoPlataformaRegistrarSinPermiso                 = @"No tiene permisos para ingresar nuevos avisos de plataforma.";
        public const string ConsErrorAvisoPlataformaModificarSinPermiso                 = @"No tiene permisos para modificar avisos de plataforma.";
        public const string ConsErrorAvisoPlataformaObtener                             = @"No fue posible cargar los datos del aviso.";

        // ERRORES - MODULO
        public const string ConsErrorModuloNavegacionConfig                             = @"No fue posible obtener la configuración de navegación del módulo.";
        public const string ConsErrorModuloNavegacionRegistrar                          = @"No fue posible registrar la navegación del módulo.";

        // ERRORES - EMPRESA TOKEN
        public const string ConsErrorEmpresaTokenConsultar                              = @"Se produjo un error al consultar los tokens.";
        public const string ConsErrorEmpresaTokenGenerar                                = @"Se produjo un error al generar el token.";
        public const string ConsErrorEmpresaTokenRenovar                                = @"Se produjo un error al renovar el token.";
        public const string ConsErrorEmpresaTokenRevocar                                = @"Se produjo un error al revocar el token.";
        public const string ConsErrorEmpresaTokenConfiguracionSeguridad                 = @"No fue posible generar el token. Verifique la configuración de seguridad.";
        public const string ConsErrorEmpresaTokenJwtIssuerNoConfigurado                 = @"La variable de entorno del emisor JWT no se encuentra configurada.";
        public const string ConsErrorEmpresaTokenJwtAudienceNoConfigurado               = @"La variable de entorno de la audiencia JWT no se encuentra configurada.";
        public const string ConsErrorEmpresaTokenJwtPasswordNoConfigurado               = @"La variable de entorno de la clave JWT no se encuentra configurada.";
        public const string ConsErrorEmpresaTokenAplicacionNoConfigurada                = @"La variable de entorno del nombre de la aplicación no se encuentra configurada.";
        public const string ConsErrorEmpresaTokenJwtBase64Invalido                      = @"La clave JWT no contiene un Base64 válido.";
        public const string ConsErrorEmpresaTokenJwtLongitudInvalida                    = @"La clave JWT debe contener al menos 256 bits.";
        public const string ConsErrorEmpresaTokenCopiar                                 = @"No fue posible copiar el token.";

        // ERRORES - MFA TOTP
        public const string ConsErrorMfaTotpConfiguracionNoVigente                      = @"La configuraci&oacute;n MFA no se encuentra vigente.";
        public const string ConsErrorMfaTotpProteccion                                  = @"No fue posible procesar la configuración de seguridad MFA.";
        public const string ConsErrorMfaTotpVersionProteccionNoCompatible               = @"La versión de protección del secreto TOTP no es compatible.";
        public const string ConsErrorMfaTotpIntegridadInvalida                          = @"La integridad del secreto TOTP protegido no es válida.";
        public const string ConsErrorMfaTotpVariableClaveNoConfigurada                  = @"La variable de entorno para cifrado MFA no está configurada.";
        public const string ConsErrorMfaTotpClaveBase64Invalida                         = @"La clave de cifrado MFA no contiene un Base64 válido.";
        public const string ConsErrorMfaTotpClaveLargoInvalido                          = @"La clave de cifrado MFA debe contener exactamente 64 bytes.";
        public const string ConsErrorMfaTotpSpRetornoNoExitoso                          = @"El SP retornó un código no exitoso: ";
        public const string ConsErrorMfaTotpValidacion                                  = @"No fue posible completar la validación MFA.";
        public const string ConsErrorMfaTotpCodigoRecuperacionCaracteres                = @"La configuración de caracteres para códigos de recuperación MFA no es válida.";
        public const string ConsErrorMfaTotpCodigoRecuperacionHash                      = @"No fue posible generar el hash del código de recuperación MFA.";
        public const string ConsErrorMfaTotpCodigoRecuperacionGenerar                   = @"No fue posible generar los códigos de recuperación MFA.";
        public const string ConsErrorMfaTotpIdentidadNoVigente                          = @"No fue posible completar el acceso porque la empresa o el usuario ya no se encuentra vigente.";
        public const string ConsErrorMfaTotpCodigoRecuperacionValidar                   = @"No fue posible validar el código de recuperación.";
        public const string ConsErrorMfaTotpCodigosRecuperacionRegenerar                = @"No fue posible regenerar los códigos de recuperación.";
        public const string ConsErrorMfaTotpCodigosRecuperacionCopiar                   = @"No fue posible copiar los códigos de recuperación.";
        public const string ConsErrorMfaTotpRestablecer                                 = @"No fue posible restablecer la configuración MFA.";

        // VALIDACIONES - CARGA
        public const string ConsValidacionCargaArchivoVacio                             = @"Debe seleccionar un archivo para importar.";
        public const string ConsValidacionCargaArchivoCsvVacio                          = @"Debe seleccionar un archivo CSV.";
        public const string ConsValidacionCargaArchivoInvalido                          = @"El archivo seleccionado no tiene un formato v&aacute;lido.";
        public const string ConsValidacionCargaSinRegistros                             = @"El archivo no contiene registros para procesar.";
        public const string ConsValidacionCargaIdInvalido                               = @"No fue posible identificar la carga masiva seleccionada.";
        public const string ConsValidacionCargaFilaColumnasRequeridas                   = @"La fila no contiene las columnas requeridas.";

        // VALIDACIONES - VISITA
        public const int ConsVisitaNombreLargoMaximo                                    = 150;
        public const int ConsVisitaEmpresaLargoMaximo                                   = 50;
        public const string ConsValidacionVisitaRutVacio                                = @"Debe ingresar RUT.";
        public const string ConsValidacionVisitaRutInvalido                             = @"El RUT indicado no es válido.";
        public const string ConsValidacionVisitaNombreVacio                             = @"Debe ingresar nombre.";
        public const string ConsValidacionVisitaNombreLargo                             = @"El nombre no puede superar los 150 caracteres.";
        public const string ConsValidacionVisitaEmpresaLarga                            = @"La empresa no puede superar los 50 caracteres.";

        // VALIDACIONES - VISITA CARGA
        public const int ConsVisitaCargaNombreLargoMaximo                               = ConsVisitaNombreLargoMaximo;
        public const int ConsVisitaCargaEmpresaLargoMaximo                              = ConsVisitaEmpresaLargoMaximo;
        public const string ConsValidacionVisitaCargaIdInvalido                         = @"No fue posible identificar la carga masiva seleccionada.";
        public const string ConsValidacionVisitaCargaRutVacio                           = ConsValidacionVisitaRutVacio;
        public const string ConsValidacionVisitaCargaRutInvalido                        = ConsValidacionVisitaRutInvalido;
        public const string ConsValidacionVisitaCargaRutDuplicado                       = @"RUT repetido en el archivo (primera aparición en la fila {0}).";
        public const string ConsValidacionVisitaCargaNombreVacio                        = ConsValidacionVisitaNombreVacio;
        public const string ConsValidacionVisitaCargaNombreLargo                        = ConsValidacionVisitaNombreLargo;
        public const string ConsValidacionVisitaCargaEmpresaVacia                       = @"Debe ingresar empresa.";
        public const string ConsValidacionVisitaCargaEmpresaLarga                       = ConsValidacionVisitaEmpresaLarga;

        // VALIDACIONES - USUARIO
        public const string ConsValidacionUsuarioRutVacio                               = @"Debe ingresar RUT.";
        public const string ConsValidacionUsuarioNombreVacio                            = @"Debe ingresar Nombre.";
        public const string ConsValidacionUsuarioApellidoVacio                          = @"Debe ingresar Apellido.";
        public const string ConsValidacionUsuarioTelefonoVacio                          = @"Debe ingresar Tel&oacute;fono.";
        public const string ConsValidacionUsuarioEmailVacio                             = @"Debe ingresar Email.";
        public const string ConsValidacionUsuarioPasswordVacio                          = @"Debe ingresar Contraseña.";
        public const string ConsValidacionUsuarioPasswordConfirmarVacio                 = @"Debe confirmar la contraseña.";
        public const string ConsValidacionUsuarioPerfilVacio                            = @"Debe seleccionar Perfil.";
        public const string ConsValidacionUsuarioPasswordNoCoincide                     = @"Las Contraseñas no coinciden.";

        // VALIDACIONES - SUPER ADMINISTRADOR
        public const string ConsValidacionSuperAdministradorRutVacio                    = @"Debe ingresar RUT.";
        public const string ConsValidacionSuperAdministradorNombreVacio                 = @"Debe ingresar Nombre.";
        public const string ConsValidacionSuperAdministradorApellidoVacio               = @"Debe ingresar Apellido.";
        public const string ConsValidacionSuperAdministradorTelefonoVacio               = @"Debe ingresar Tel&oacute;fono.";
        public const string ConsValidacionSuperAdministradorEmailVacio                  = @"Debe ingresar Email.";
        public const string ConsValidacionSuperAdministradorPasswordVacio               = @"Debe ingresar Contraseña.";
        public const string ConsValidacionSuperAdministradorPasswordConfirmarVacio      = @"Debe confirmar la contraseña.";
        public const string ConsValidacionSuperAdministradorPasswordNoCoincide          = @"Las Contraseñas no coinciden.";
        public const string ConsValidacionSuperAdministradorEstadoVacio                 = @"Debe seleccionar el estado del super administrador.";
        public const string ConsValidacionSuperAdministradorNoExiste                    = @"El super administrador no existe.";
        public const string ConsValidacionSuperAdministradorExiste                      = @"El usuario ya se encuentra registrado como super administrador.";

        // VALIDACIONES - REGLA
        public const string ConsValidacionReglaNombreVacio                              = @"Debe ingresar Nombre.";
        public const string ConsValidacionReglaPerfilVigente                            = @"No es posible anular la regla porque está siendo utilizada por uno o más perfiles vigentes.";
        public const string ConsValidacionReglaIndicadorInvalido                        = @"No fue posible determinar el valor de uno de los indicadores de la regla.";
        public const string ConsValidacionReglaListaBlancaNegraInvalida                 = @"No fue posible determinar la lista blanca / negra de la regla.";

        // VALIDACIONES - PERFIL
        public const string ConsValidacionPerfilNombreVacio                             = @"Debe ingresar Nombre.";
        public const string ConsValidacionPerfilReglaVacia                              = @"Debe seleccionar Regla.";
        public const string ConsValidacionPerfilUsuarioVigente                          = @"No es posible anular el perfil porque está siendo utilizado por uno o más usuarios vigentes.";

        // VALIDACIONES - DISPOSITIVO
        public const string ConsValidacionDispositivoMarcaVacia                         = @"Debe ingresar Marca.";
        public const string ConsValidacionDispositivoModeloVacio                        = @"Debe ingresar Modelo.";
        public const string ConsValidacionDispositivoPartnumberVacio                    = @"Debe ingresar Partnumber.";
        public const string ConsValidacionDispositivoSerialnumberVacio                  = @"Debe ingresar Serialnumber.";
        public const string ConsValidacionDispositivoIdVacio                            = @"Debe ingresar Id.";
        public const string ConsValidacionDispositivoRutEmpresaVacio                    = @"Debe ingresar RUT Empresa.";
        public const string ConsValidacionDispositivoSerialNumberExiste                 = @"Ya existe un dispositivo registrado con el serial number ingresado.";
        public const string ConsValidacionDispositivoIdentidadNoModificable             = @"El Part Number y Serial Number identifican al dispositivo y no pueden ser modificados.";

        // VALIDACIONES - DISPOSITIVO MODELO
        public const string ConsValidacionDispositivoModeloExiste                       = @"Ya existe un modelo registrado con la misma marca y nombre.";
        public const string ConsValidacionDispositivoModeloIdInvalido                   = @"No fue posible identificar el modelo seleccionado.";
        public const string ConsValidacionDispositivoModeloNombreLargo                  = @"El nombre del modelo no puede superar los 100 caracteres.";
        public const string ConsValidacionDispositivoModeloEstadoInvalido               = @"El estado seleccionado no es válido.";

        // VALIDACIONES - MARCA
        public const string ConsValidacionDispositivoMarcasNombreVacio                  = @"Debe ingresar Marca.";

        // VALIDACIONES - MODELO
        public const string ConsValidacionDispositivoModelosMarcaVacia                  = @"Debe seleccionar Marca.";
        public const string ConsValidacionDispositivoModelosNombreVacio                 = @"Debe ingresar Nombre.";

        // VALIDACIONES - EMPRESA
        public const string ConsValidacionEmpresaRutVacio                               = @"Debe ingresar RUT.";
        public const string ConsValidacionEmpresaNombreVacio                            = @"Debe ingresar Nombre.";
        public const string ConsValidacionEmpresaDireccionVacia                         = @"Debe ingresar Direcci&oacute;n.";
        public const string ConsValidacionEmpresaCiudadVacia                            = @"Debe ingresar Ciudad.";
        public const string ConsValidacionEmpresaComunaVacia                            = @"Debe ingresar Comuna.";
        public const string ConsValidacionEmpresaContactoVacio                          = @"Debe ingresar Contacto.";
        public const string ConsValidacionEmpresaContactoTelefonoVacio                  = @"Debe ingresar Contacto Tel&eacute;fono.";
        public const string ConsValidacionEmpresaContactoEmailVacio                     = @"Debe ingresar Contacto Email.";
        public const string ConsValidacionEmpresaExternoInvalido                        = @"No fue posible determinar si la empresa permite registrar dispositivos externos.";
        public const string ConsValidacionEmpresaTokenCopiarVacio                       = @"No hay un token disponible para copiar.";

        // VALIDACIONES - VERSION APLICACION
        public const string ConsValidacionVersionAplicacionFabricanteVacio              = @"Debe ingresar Fabricante.";
        public const string ConsValidacionVersionAplicacionModeloVacio                  = @"Debe ingresar Modelo.";
        public const string ConsValidacionVersionAplicacionFabricanteApkVacio           = @"Debe ingresar Fabricante APK.";
        public const string ConsValidacionVersionAplicacionModeloApkVacio               = @"Debe ingresar Modelo APK.";
        public const string ConsValidacionVersionAplicacionAndroidVersionVacio          = @"Debe seleccionar Versi&oacute;n.";
        public const string ConsValidacionVersionAplicacionApkVacio                     = @"Debe seleccionar archivo APK.";
        public const string ConsValidacionVersionAplicacionAndroidVersionVacia          = @"Debe seleccionar Android Versi&oacute;n.";
        public const string ConsValidacionVersionAplicacionComentariosVacio             = @"Debe ingresar comentarios.";
        public const string ConsValidacionVersionAplicacionVersionInvalida              = @"Debe seleccionar una versión válida.";
        public const string ConsValidacionVersionAplicacionVersionMayorCero             = @"La versión actual debe ser mayor a 0.0.0.";
        public const string ConsValidacionVersionAplicacionIdInvalido                   = @"No fue posible identificar la versión de aplicación seleccionada.";
        public const string ConsValidacionVersionAplicacionForceUpdateInvalido          = @"No fue posible determinar si la versión fuerza actualización.";
        public const string ConsValidacionVersionAplicacionEstadoInvalido               = @"No fue posible determinar el estado de la versión.";
        public const string ConsValidacionVersionAplicacionPublicacionInvalida          = @"No fue posible determinar el estado de publicación de la versión.";
        public const string ConsValidacionVersionAplicacionPublicadaDebeEstarActiva     = @"Una versi&oacute;n publicada debe estar activa.";
        public const string ConsValidacionVersionAplicacionApkNoExiste                  = @"No fue posible encontrar el archivo APK cargado.";
        public const string ConsValidacionVersionAplicacionApkExtensionInvalida         = @"El archivo seleccionado debe tener extensión .apk.";
        public const string ConsValidacionVersionAplicacionApkNombreInvalido            = @"El archivo APK debe llamarse {0}.";
        public const string ConsValidacionVersionAplicacionRutaRelativaNoConfigurada    = @"No se encuentra configurada la ruta relativa para la publicación del APK.";
        public const string ConsValidacionVersionAplicacionPrimeraVersionInvalida       = @"La primera versión debe ser igual o superior a 1.0.0.";
        public const string ConsValidacionVersionAplicacionPrimeraVersionMinimaInvalida = @"Para la primera versión, la versión mínima soportada debe ser igual a la versión actual.";
        public const string ConsValidacionVersionAplicacionVersionDebeSerSuperior       = @"La versión debe ser superior a la última registrada para el fabricante, modelo y versión Android.";
        public const string ConsValidacionVersionAplicacionSha256Invalido               = @"No fue posible calcular correctamente el hash SHA256 del archivo APK.";
        public const string ConsValidacionVersionAplicacionTamanioArchivoInvalido       = @"El archivo APK seleccionado no tiene un tamaño válido.";
        public const string ConsValidacionVersionAplicacionSoloUltimaPuedePublicarse    = @"Solo se puede publicar la versión más reciente para el fabricante, modelo y versión Android seleccionados.";

        // VALIDACIONES - CATEGORIA
        public const string ConsValidacionCategoriaNombreVacio                          = @"Debe ingresar el nombre de la categoría.";
        public const string ConsValidacionCategoriaOrdenVacio                           = @"Debe ingresar el orden de la categoría.";
        public const string ConsValidacionCategoriaIconoVacio                           = @"Debe ingresar el ícono de la categoría.";

        // VALIDACIONES - MODULO
        public const string ConsValidacionModuloCategoriaVacia                          = @"Debe seleccionar la categoría del módulo.";
        public const string ConsValidacionModuloNombreVacio                             = @"Debe ingresar el nombre del módulo.";

        public const string ConsValidacionModuloTituloPaginaVacio                       = @"Debe ingresar el título de página del módulo.";
        public const string ConsValidacionModuloTituloPaginaLargo                       = @"El título de página del módulo no puede superar los 150 caracteres.";
        public const string ConsValidacionModuloTituloContenidoVacio                    = @"Debe ingresar el título de contenido del módulo.";
        public const string ConsValidacionModuloTituloContenidoLargo                    = @"El título de contenido del módulo no puede superar los 150 caracteres.";

        public const string ConsValidacionModuloOrdenVacio                              = @"Debe ingresar el orden del módulo.";
        public const string ConsValidacionModuloUrlVacia                                = @"Debe ingresar la URL real del módulo.";
        public const string ConsValidacionModuloUrlPublicaVacia                         = @"Debe ingresar la URL pública del módulo.";
        public const string ConsValidacionModuloEstadoInvalido                          = @"El estado del módulo no es válido.";
        public const string ConsValidacionModuloAdministradorInvalido                   = @"El valor administrador del módulo no es válido.";
        public const string ConsValidacionModuloMenuVisibleInvalido                     = @"El valor visible menú del módulo no es válido.";
        public const string ConsValidacionModuloBitacoraEstadoInvalido                  = @"Debe seleccionar si el m&oacute;dulo utiliza bit&aacute;cora.";
        public const string ConsValidacionModuloBitacoraSpVacio                         = @"Debe ingresar el procedimiento almacenado de bit&aacute;cora.";
        public const string ConsValidacionModuloBitacoraParametroVacio                  = @"Debe ingresar el par&aacute;metro del procedimiento almacenado de bit&aacute;cora.";
        public const string ConsValidacionModuloNavegacionNoEncontrado                  = @"No fue posible resolver la página de destino.";
        public const string ConsValidacionModuloNavegacionDuplicado                     = @"Existe más de un módulo configurado para la página de destino.";
        public const string ConsValidacionModuloNavegacionNoVigente                     = @"La página de destino no se encuentra vigente.";
        public const string ConsValidacionModuloNavegacionError                         = @"No fue posible validar la navegación del módulo.";
        public const string ConsValidacionModuloContextoIdVacio                         = @"No fue posible identificar el módulo actual.";
        public const string ConsValidacionModuloContextoIdInvalido                      = @"El identificador del módulo actual no es válido.";
        public const string ConsValidacionModuloContextoNoEncontrado                    = @"El módulo actual no corresponde a la página solicitada.";
        public const string ConsValidacionModuloContextoNoVigente                       = @"El módulo actual no se encuentra vigente.";
        public const string ConsValidacionModuloContextoError                           = @"No fue posible validar el módulo actual.";
        public const string ConsValidacionModuloNavegacionAccionNoEncontrada            = @"No existe navegación configurada para la acción solicitada.";
        public const string ConsValidacionModuloNavegacionAccionNoVigente               = @"La navegación solicitada no se encuentra vigente.";
        public const string ConsValidacionModuloNavegacionAccionDestinoNoVigente        = @"La página de destino no se encuentra vigente.";
        public const string ConsValidacionModuloNavegacionAccionError                   = @"No fue posible validar la navegación solicitada.";
        public const string ConsValidacionModuloUrlDuplicada                            = @"Ya existe un módulo registrado con la URL indicada.";
        public const string ConsValidacionModuloUrlPublicaDuplicada                     = @"Ya existe un módulo registrado con la URL pública indicada.";
        public const string ConsValidacionModuloUrlDuplicadaError                       = @"No fue posible validar las URLs del módulo.";
        public const string ConsValidacionModuloNavegacionOrigenNoIdentificado          = @"No fue posible identificar el módulo origen.";
        public const string ConsValidacionModuloNavegacionDatosNoObtenidos              = @"No fue posible obtener los datos de navegación del módulo.";
        public const string ConsValidacionModuloNavegacionAccionRequerida               = @"Debe ingresar la acción de navegación.";
        public const string ConsValidacionModuloNavegacionAccionLargo                   = @"La acción de navegación no puede superar los 30 caracteres.";
        public const string ConsValidacionModuloNavegacionDestinoRequerido              = @"Debe seleccionar el módulo destino.";
        public const string ConsValidacionModuloNavegacionEstadoInvalido                = @"Debe seleccionar un estado válido para la navegación.";
        public const string ConsValidacionModuloNavegacionUsuarioNoIdentificado         = @"No fue posible identificar el usuario responsable.";
        public const string ConsValidacionModuloNavegacionAccionDuplicada               = @"Ya existe una navegación configurada para la acción seleccionada.";

        // VALIDACIONES - MODULO PERFIL
        public const string ConsValidacionModuloPerfilRelacionNoRegistrada              = @"El módulo seleccionado aún no tiene una relación de permisos registrada.";

        // VALIDACIONES - AVISOS PLATAFORMA
        public const string ConsValidacionAvisoPlataformaTituloVacio                    = @"Debe ingresar el t&iacute;tulo del aviso.";
        public const string ConsValidacionAvisoPlataformaMensajeVacio                   = @"Debe ingresar el mensaje del aviso.";
        public const string ConsValidacionAvisoPlataformaVersionVacia                   = @"Debe ingresar la versi&oacute;n asociada al aviso.";
        public const string ConsValidacionAvisoPlataformaFechaInicioInvalida            = @"La fecha de inicio del aviso no es v&aacute;lida.";
        public const string ConsValidacionAvisoPlataformaFechaTerminoInvalida           = @"La fecha de t&eacute;rmino del aviso no es v&aacute;lida.";
        public const string ConsValidacionAvisoPlataformaRangoFechasInvalido            = @"La fecha de t&eacute;rmino no puede ser menor a la fecha de inicio.";
        public const string ConsValidacionAvisoPlataformaMaximoDias                     = @"La vigencia del aviso no puede superar los 20 d&iacute;as.";
        public const string ConsValidacionAvisoPlataformaEstadoInvalido                 = @"El estado del aviso no es v&aacute;lido.";
        public const string ConsValidacionAvisoPlataformaIdInvalido                     = @"No fue posible identificar el aviso seleccionado.";

        // VALIDACIONES - COMMERCE API
        public const string ConsCommerceDispositivoDatosVacios                          = @"Debe informar el PartNumber y SerialNumber del dispositivo.";
        public const string ConsCommerceApiUrlNoConfigurada                             = @"No se encuentra configurada la dirección de la API Commerce.";
        public const string ConsCommerceApiEPNoConfigurado                              = @"No se encuentra configurado el endpoint de validación de dispositivos de la API Commerce.";
        public const string ConsCommerceApiRespuestaVacia                               = @"La API Commerce no entregó una respuesta válida.";
        public const string ConsCommerceApiRespuestaInvalida                            = @"La API Commerce entregó una respuesta inválida.";
        public const string ConsCommerceDispositivoNoValidado                           = @"El dispositivo no fue validado por Commerce.";
        public const string ConsCommerceApiValidacionError                              = @"No fue posible validar el dispositivo en Commerce.";
        public const string ConsCommerceApiTimeout                                      = @"La consulta a Commerce superó el tiempo máximo de espera.";
        public const string ConsCommerceApiComunicacionError                            = @"No fue posible establecer comunicación con Commerce.";
        public const string ConsCommerceApiHeaderKeyNoConfigurado                       = @"No se encuentra configurada la cabecera de autenticación de la API Commerce.";
        public const string ConsCommerceApiHeaderValueNoConfigurado                     = @"No se encuentra configurado el valor de autenticación de la API Commerce.";
        public const string ConsLogCommerceApiHeaderKeyNoConfigurado                    = @"No se encuentra configurada la variable de entorno EV-Commerce-Header-Key.";
        public const string ConsLogCommerceApiHeaderValueNoConfigurado                  = @"No se encuentra configurada la variable de entorno EV-Commerce-Header-Value.";
        public const string ConsLogCommerceApiHeaderNoAgregado                          = @"No fue posible agregar la cabecera de autenticación a la solicitud de Commerce.";
        public const string ConsValidacionDispositivoNoVendidoDiprotec                  = @"El Part Number y Serial Number ingresados no corresponden a un dispositivo suministrado por DIPROTEC.";
        public const string ConsValidacionDispositivoCommerceNoDisponible               = @"No fue posible validar el dispositivo en este momento. Intente nuevamente.";

        // VALIDACIONES - BASE DATOS
        public const string ConsValidacionBaseDatosTablaVacia                           = @"Debe seleccionar una tabla de base de datos.";

        // VALIDACIONES - EMPRESA TOKEN
        public const string ConsValidacionEmpresaTokenEmpresaNoIdentificada             = @"No fue posible identificar la empresa.";
        public const string ConsValidacionEmpresaTokenNoIdentificado                    = @"No fue posible identificar el token.";
        public const string ConsValidacionEmpresaTokenUsuarioNoIdentificado             = @"No fue posible identificar al usuario.";
        public const string ConsValidacionEmpresaTokenMotivoRevocacionVacio             = @"Debe ingresar el motivo de la revocación.";
        public const string ConsValidacionEmpresaTokenMotivoRevocacionLargo             = @"El motivo de la revocación no puede superar los 500 caracteres.";
        public const string ConsValidacionEmpresaTokenVacio                             = @"El token no puede estar vacío.";

        // VALIDACIONES - MFA TOTP
        public const string ConsValidacionMfaTotpPropietarioInvalido                    = @"No fue posible identificar correctamente al usuario para configurar MFA.";
        public const string ConsValidacionMfaTotpYaConfigurado                          = @"La autenticaci&oacute;n multifactor ya se encuentra configurada.";
        public const string ConsValidacionMfaTotpSecretoVacio                           = @"El secreto TOTP no puede estar vacío.";
        public const string ConsValidacionMfaTotpSecretoProtegidoInvalido               = @"El secreto TOTP protegido no tiene un formato válido.";
        public const string ConsValidacionMfaTotpEmisorVacio                            = @"El emisor TOTP no puede estar vacío.";
        public const string ConsValidacionMfaTotpCuentaVacia                            = @"La cuenta TOTP no puede estar vacía.";
        public const string ConsValidacionMfaTotpSecretoBase32Vacio                     = @"El secreto TOTP en formato Base32 no puede estar vacío.";
        public const string ConsValidacionMfaTotpCodigoVacio                            = @"Debe ingresar el código de autenticación.";
        public const string ConsValidacionMfaTotpConfiguracionNoEncontrada              = @"No fue posible encontrar una configuración MFA pendiente.";
        public const string ConsValidacionMfaTotpConfiguracionBloqueada                 = @"La configuración MFA se encuentra temporalmente bloqueada.";
        public const string ConsValidacionMfaTotpCodigoInvalido                         = @"El código de autenticación ingresado no es válido.";
        public const string ConsValidacionMfaTotpCodigoReutilizado                      = @"El código de autenticación ya fue utilizado.";
        public const string ConsValidacionMfaTotpConfiguracionNoConfirmada              = @"La configuración MFA todavía no ha sido confirmada.";
        public const string ConsValidacionMfaTotpCodigoRecuperacionFormatoInvalido      = @"El código de recuperación MFA no tiene un formato válido.";
        public const string ConsValidacionMfaTotpCodigoRecuperacionVacio                = @"Ingresa un código de recuperación.";
        public const string ConsValidacionMfaTotpCodigoRecuperacionInvalido             = @"El código de recuperación no es válido o ya fue utilizado.";

        // EXITO - GENERICO
        public const string ConsExitoRegistroActualizar                                 = @"Registro actualizado correctamente.";

        // EXITO - RECUPERACION / USUARIO
        public const string ConsExitoRecuperarEnvioMail                                 = @"La informaci&oacute;n de acceso se ha enviado exitosamente a su email.";
        public const string ConsExitoUsuarioRegistro                                    = @"Usuario registrado exitosamente.";
        public const string ConsExitoUsuarioActualizar                                  = @"Usuario actualizado exitosamente.";

        // EXITO - SUPER ADMINISTRADOR
        public const string ConsExitoSuperAdministradorRegistro                         = @"El super administrador fue registrado exitosamente.";
        public const string ConsExitoSuperAdministradorActualizar                       = @"El super administrador fue actualizado exitosamente.";

        // EXITO - REGLA
        public const string ConsExitoReglaRegistro                                      = @"Regla registrada exitosamente.";
        public const string ConsExitoReglaActualizar                                    = @"Regla actualizada exitosamente.";

        // EXITO - PERFIL
        public const string ConsExitoPerfilRegistro                                     = @"Perfil registrado exitosamente.";
        public const string ConsExitoPerfilActualizar                                   = @"Perfil actualizado exitosamente.";

        // EXITO - VISITA
        public const string ConsExitoVisitaRegistro                                     = @"Visita registrada exitosamente.";
        public const string ConsExitoVisitaActualizar                                   = @"Visita actualizada exitosamente.";

        // EXITO - VISITA CARGA
        public const string ConsExitoVisitaCargaProcesada                               = @"Carga procesada correctamente. Filas: {0}, OK: {1}, Error: {2}.";
        public const string ConsExitoVisitaCargaInsertada                               = @"Visita registrada.";
        public const string ConsExitoVisitaCargaActualizada                             = @"Visita actualizada.";
        public const string ConsExitoVisitaCargaSinCambios                              = @"Los datos son iguales a los registrados.";

        // EXITO - DISPOSITIVO
        public const string ConsExitoDispositivoRegistro                                = @"Dispositivo registrado exitosamente.";
        public const string ConsExitoDispositivoActualizar                              = @"Dispositivo actualizado exitosamente.";
        public const string ConsExitoDispositivoActualizarActivationCode                = @"C&oacute;digo de activaci&oacute;n actualizado exitosamente.";

        // EXITO - DISPOSITIVO MODELO
        public const string ConsExitoDispositivoModeloRegistro                          = @"El modelo fue registrado exitosamente.";
        public const string ConsExitoDispositivoModeloActualizar                        = @"El modelo fue actualizado exitosamente.";

        // EXITO - EMPRESA
        public const string ConsExitoEmpresaRegistro                                    = @"Empresa registrada exitosamente.";
        public const string ConsExitoEmpresaActualizar                                  = @"Empresa actualizada exitosamente.";

        // EXITO - VERSION APLICACION
        public const string ConsExitoVersionAplicacionRegistro                          = @"Versión de aplicación registrada exitosamente.";
        public const string ConsExitoVersionAplicacionActualizar                        = @"Versión de aplicación actualizada exitosamente.";
        public const string ConsErrorVersionAplicacionExiste                            = @"Ya existe una versión registrada para el mismo fabricante, modelo, versión Android y código de versión.";

        // EXITO - MARCA
        public const string ConsExitoMarcaRegistro                                      = @"Marca registrada exitosamente.";
        public const string ConsExitoMarcaActualizar                                    = @"Marca actualizada exitosamente.";

        // EXITO - CATEGORIA
        public const string ConsExitoCategoriaRegistro                                  = @"Categoría registrada exitosamente.";
        public const string ConsExitoCategoriaActualizar                                = @"Categoría actualizada exitosamente.";

        // EXITO - MODULO
        public const string ConsExitoModuloRegistro                                     = @"Módulo registrado exitosamente.";
        public const string ConsExitoModuloActualizar                                   = @"Módulo actualizado exitosamente.";

        // EXITO - AVISO PLATAFORMA
        public const string ConsExitoAvisoPlataformaRegistro                            = @"Aviso registrado exitosamente.";
        public const string ConsExitoAvisoPlataformaActualizar                          = @"Aviso actualizado exitosamente.";

        // EXITO - MODULO
        public const string ConsExitoModuloNavegacionRegistrar                          = @"Navegación del módulo registrado exitosamente.";

        // EXITO - EMPRESA TOKEN
        public const string ConsExitoEmpresaTokenGenerar                                = @"Token generado exitosamente.";
        public const string ConsExitoEmpresaTokenRenovar                                = @"Token renovado exitosamente.";
        public const string ConsExitoEmpresaTokenRevocar                                = @"Token revocado exitosamente.";
        public const string ConsExitoEmpresaTokenCopiar                                 = @"Token copiado al portapapeles.";

        // EXITO - MFA TOTP
        public const string ConsExitoMfaTotpConfiguracionIniciar                        = @"Configuraci&oacute;n MFA iniciada correctamente.";
        public const string ConsExitoMfaTotpConfiguracionObtener                        = @"Configuraci&oacute;n MFA pendiente recuperada correctamente.";
        public const string ConsExitoMfaTotpConfiguracionConfirmar                      = @"La autenticación multifactor fue configurada correctamente.";
        public const string ConsExitoMfaTotpValidacion                                  = @"Código de autenticación validado correctamente.";
        public const string ConsExitoMfaTotpCodigoRecuperacionValidar                   = @"Código de recuperación validado correctamente.";
        public const string ConsExitoMfaTotpCodigosRecuperacionRegenerar                = @"Los códigos de recuperación fueron regenerados correctamente.";
        public const string ConsExitoMfaTotpCodigosRecuperacionCopiar                   = @"Códigos de recuperación copiados al portapapeles.";
        public const string ConsExitoMfaTotpRestablecer                                 = @"La configuración MFA fue restablecida correctamente.";

        // LOGS - COMMERCE API
        public const string ConsLogCommerceApiUrlNoConfigurada                          = @"No se encuentra configurada la variable de entorno de la URL de Commerce.";
        public const string ConsLogCommerceApiEPNoConfigurado                           = @"No se encuentra configurada la variable de entorno del endpoint de Commerce.";
        public const string ConsLogCommerceApiRespuestaVacia                            = @"La API Commerce respondió sin contenido. HTTP: '{0}'.";
        public const string ConsLogCommerceApiRespuestaInvalida                         = @"No fue posible interpretar la respuesta de Commerce. HTTP: '{0}'.";
        public const string ConsLogCommerceApiRespuestaNula                             = @"La respuesta deserializada de Commerce es nula. HTTP: '{0}'.";
        public const string ConsLogCommerceApiErrorTecnico                              = @"Error técnico al consultar Commerce. HTTP: '{0}', Estado: '{1}', CodigoError: '{2}', CorrelationId: '{3}'.";
        public const string ConsLogCommerceApiTimeout                                   = @"Timeout al consultar la validación del dispositivo en Commerce.";
        public const string ConsLogCommerceApiComunicacionError                         = @"Error de comunicación al consultar la validación del dispositivo en Commerce.";
        public const string ConsLogCommerceApiErrorInesperado                           = @"Error inesperado al validar el dispositivo en Commerce.";

        // CONFIGURACION - COMMERCE API
        public const int ConsCommerceApiTimeoutSegundos                                 = 15;

        // PLANTILLA - RECUPERAR PASSWORD
        public const string ConsRecuperarPassword                                       = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"1000\" style=\"font-family: arial; font-size: 12px\">" +
                                                                                            "<tr>" +
                                                                                                "<td valign=\"top\" align=\"justify\" height=\"25\">La informaci&oacute;n contenida en este correo electr&oacute;nico es confidencial</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\" height=\"40\"><b>Estimado(a) ##usuario##:</b></td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td><b>Recuperaci&oacute;n de Contrase&ntilde;a.</b></td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\">Te informamos que hemos recibido de forma exitosa tu solicitud de recuperaci&oacute;n de contrase&ntilde;a realizada el d&iacute;a ##fecha##.</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>Tu nueva contrase&ntilde;a de acceso es: <b>##nueva_password##</b></td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td align=\"justify\">Atte.<br><b>Servicio Atenci&oacute;n Integral</b><br><b>DIPROTEC</b></td>" +
                                                                                            "</tr>" +
                                                                                        "</table>";

        // PLANTILLA - NUEVO USUARIO
        public const string ConsNuevoUsuario                                            = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"1000\" style=\"font-family: arial; font-size: 12px\">" +
                                                                                            "<tr>" +
                                                                                                "<td valign=\"top\" align=\"justify\" height=\"25\">La informaci&oacute;n contenida en este correo electr&oacute;nico es confidencial</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\" height=\"40\"><b>Estimado(a) ##usuario##:</b></td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td><b>Alta de usuario en plataforma <b>DIPROTEC ##website_nombre##</b>.</b></td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\">Te informamos que con fecha ##fecha## se ha creado exitosamente tu usuario de acceso a la plataforma <b>DIPROTEC ##website_nombre##</b>.</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\">A continuaci&oacute;n, se detallan tus credenciales de acceso:</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>" +
                                                                                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"650\" style=\"font-family: arial; font-size: 12px\">" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\" width=\"160px\" nowrap><b>Plataforma</b></td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;<a href=\"##url_plataforma##\" target=\"_blank\">##url_plataforma##</a></td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\"><b>RUT Empresa</b></td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##rut_empresa##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\"><b>Usuario</b></td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##rut_usuario##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\"><b>Correo</b></td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##email_usuario##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\"><b>Contrase&ntilde;a</b></td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##password##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td colspan=\"2\">&nbsp;</td>" +
                                                                                                        "</tr>" +
                                                                                                    "</table>" +
                                                                                                "</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\">Por seguridad, te recomendamos cambiar tu contrase&ntilde;a al ingresar por primera vez a la plataforma y no compartir tus credenciales con terceros.</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\">Si no reconoces esta alta de usuario o tienes inconvenientes para acceder, favor contactar al administrador de la plataforma o al equipo de soporte DIPROTEC.</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td align=\"justify\">Atte.<br><b>Servicio Atenci&oacute;n Integral</b><br><b>DIPROTEC</b></td>" +
                                                                                            "</tr>" +
                                                                                        "</table>";

        // PLANTILLA - ERROR
        public const string ConsCuerpoError                                             = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"1000\" style=\"font-family: arial; font-size: 12px\">" +
                                                                                            "<tr>" +
                                                                                                "<td valign=\"top\" align=\"justify\" height=\"25\">La informaci&oacute;n contenida en este correo electr&oacute;nico es confidencial</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td height=\"40\"><b>Error en app ##host##.</b></td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td style=\"text-align: justify\">Se ha detectado un error en la siguiente instrucci&oacute;n:</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>" +
                                                                                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"1000\" style=\"font-family: arial; font-size: 12px\">" +
                                                                                                        "<tr>" +
                                                                                                            "<td colspan=\"2\">&nbsp;</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\" width=\"250px\" nowrap>URL M&oacute;dulo</td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##url##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\" nowrap>Metodo</td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##metodo##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\" nowrap>Consulta SQL</td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##consulta##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\" nowrap>Error</td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##error##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\" nowrap>Solicitado por</td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##usuario##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td style=\"text-align: left\" nowrap>Fecha&nbsp;/&nbsp;Hora</td>" +
                                                                                                            "<td style=\"text-align: left\">:&nbsp;##fecha##&nbsp;/&nbsp;##hora##</td>" +
                                                                                                        "</tr>" +
                                                                                                        "<tr>" +
                                                                                                            "<td colspan=\"2\">&nbsp;</td>" +
                                                                                                        "</tr>" +
                                                                                                    "</table>" +
                                                                                                "</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td>&nbsp;</td>" +
                                                                                            "</tr>" +
                                                                                            "<tr>" +
                                                                                                "<td align=\"justify\">Atte.<br><b>Servicio Atenci&oacute;n Integral</b><br><b>DIPROTEC</b></td>" +
                                                                                            "</tr>" +
                                                                                        "</table>";
    }
}
