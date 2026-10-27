using Datos.v1.VersionAplicacion;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.VersionAplicacion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;

namespace Negocio.v1.VersionAplicacion
{
    public class VersionAplicacionNegocio
    {
        public static List<VersionAplicacionModelo> GetReporteVersionesAplicacion(ref int operacion, string buscador, short estado, short orden, short orientacion)
        {
            return VersionAplicacionDatos.GetReporteVersionesAplicacion(ref operacion, buscador, estado, orden, orientacion);
        }

        public static VersionAplicacionModelo GetVersionAplicacion(ref int operacion, VersionAplicacionModelo ver)
        {
            return VersionAplicacionDatos.GetVersionAplicacion(ref operacion, ver);
        }

        public static int UpdateVersion(VersionAplicacionModelo version)
        {
            return VersionAplicacionDatos.UpdateVersionAplicacion(version);
        }

        public static int InsertVersion(VersionAplicacionModelo version)
        {
            return VersionAplicacionDatos.InsertVersionAplicacion(version);
        }

        public static RespuestaModelo<List<VersionAplicacionModelo>> GetVersionesAplicacion(VersionAplicacionFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<VersionAplicacionModelo>>();

            var operacion = 0;
            var versiones = VersionAplicacionDatos.GetReporteVersionesAplicacion(
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
                    respuesta.Datos = versiones;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<VersionAplicacionModelo> RegistrarVersionAplicacion(VersionAplicacionModelo version, string apkTempPath)
        {
            var respuesta = new RespuestaModelo<VersionAplicacionModelo>();

            if (version == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.Fabricante))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionFabricanteVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.Modelo))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionModeloVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.FabricanteApk))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionFabricanteApkVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.ModeloApk))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionModeloApkVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.VersionName))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionVersionInvalida;
                return respuesta;
            }

            if (!TryCalcularVersionCode(version.VersionName, out var versionCode))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionVersionInvalida;
                return respuesta;
            }

            if (versionCode <= 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionVersionMayorCero;
                return respuesta;
            }

            if (version.AndroidVersion <= 0 || version.AndroidVersion > 99)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionAndroidVersionVacio;
                return respuesta;
            }

            if (version.ForceUpdate != 0 && version.ForceUpdate != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionForceUpdateInvalido;
                return respuesta;
            }

            if (version.IsActive != 0 && version.IsActive != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionEstadoInvalido;
                return respuesta;
            }

            if (version.Ispublished != 0 && version.Ispublished != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionPublicacionInvalida;
                return respuesta;
            }

            if (version.Ispublished == 0 && version.IsActive != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionPublicadaDebeEstarActiva;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.CreatedBy))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesion;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.ApkFileName))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionApkVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(apkTempPath))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionApkVacio;
                return respuesta;
            }

            if (!File.Exists(apkTempPath))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionApkNoExiste;
                return respuesta;
            }

            if (!version.ApkFileName.EndsWith(".apk", StringComparison.OrdinalIgnoreCase))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionApkExtensionInvalida;
                return respuesta;
            }

            version.Fabricante = version.Fabricante.Trim().ToUpper();
            version.Modelo = version.Modelo.Trim().ToUpper();

            version.FabricanteApk = version.FabricanteApk.Trim().ToUpper();
            version.ModeloApk = version.ModeloApk.Trim().ToUpper();

            version.VersionName = NormalizarVersionName(version.VersionName);
            version.VersionCode = versionCode;

            var apkFileNameEsperado = ConstruirNombreApkEsperado(
                version.FabricanteApk,
                version.ModeloApk,
                "A" + version.AndroidVersion,
                version.VersionName
            );

            if (!string.Equals(
                version.ApkFileName,
                apkFileNameEsperado,
                StringComparison.Ordinal))
            {
                respuesta.Mensaje = string.Format(
                    ConstantesModelo.ConsValidacionVersionAplicacionApkNombreInvalido,
                    apkFileNameEsperado
                );

                return respuesta;
            }

            version.ApkFileName = apkFileNameEsperado;

            var rutaRelativa = ConstantesModelo.ConsUrl + "/" + ConstantesModelo.ConsDownloadPath;

            if (string.IsNullOrWhiteSpace(rutaRelativa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionRutaRelativaNoConfigurada;
                return respuesta;
            }

            version.ApkRelativePath = rutaRelativa.TrimEnd('/');

            var operacionUltimaVersion = 0;
            var ultimaVersion = VersionAplicacionDatos.GetUltimaVersion(ref operacionUltimaVersion, version);

            if (operacionUltimaVersion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            var esPrimeraVersion = ultimaVersion == null || ultimaVersion.Id == 0;

            if (esPrimeraVersion)
            {
                var versionMinimaPrimera = CalcularVersionCode(1, 0, 0);

                if (version.VersionCode < versionMinimaPrimera)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionPrimeraVersionInvalida;
                    return respuesta;
                }
            }
            else
            {
                if (version.VersionCode <= ultimaVersion.VersionCode)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionVersionDebeSerSuperior;
                    return respuesta;
                }
            }

            string rutaApkFinal;

            try
            {
                rutaApkFinal = CopiarApkADescargas(apkTempPath, version.ApkFileName);
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            if (!File.Exists(rutaApkFinal))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionApkNoExiste;
                return respuesta;
            }

            version.ApkSha256 = CalcularSha256Archivo(rutaApkFinal);
            version.FileSizeBytes = new FileInfo(rutaApkFinal).Length;

            if (string.IsNullOrWhiteSpace(version.ApkSha256) || version.ApkSha256.Length != 64)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionSha256Invalido;
                return respuesta;
            }

            if (version.FileSizeBytes <= 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionTamanioArchivoInvalido;
                return respuesta;
            }

            version.ReleaseNotes = string.IsNullOrWhiteSpace(version.ReleaseNotes)
                ? null
                : version.ReleaseNotes.Trim();

            version.CreatedAt = DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.ffffff",
                CultureInfo.InvariantCulture
            );

            version.PublishedAt = version.Ispublished == 0
                ? DateTime.Now.ToString(
                    "yyyy-MM-dd HH:mm:ss.ffffff",
                    CultureInfo.InvariantCulture
                )
                : null;

            var resultado = VersionAplicacionDatos.InsertVersionAplicacion(version);

            if (resultado != 0)
            {
                EliminarArchivoSiExiste(rutaApkFinal);
            }

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = version;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoVersionAplicacionRegistro;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case -1:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;

                case -2:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                    return respuesta;

                case -3:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorVersionAplicacionExiste;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<VersionAplicacionModelo> ActualizarVersionAplicacion(VersionAplicacionModelo version)
        {
            var respuesta = new RespuestaModelo<VersionAplicacionModelo>();

            if (version == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            if (version.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionIdInvalido;
                return respuesta;
            }

            if (version.UpdatedAt == DateTime.MinValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRegistroObtener;
                return respuesta;
            }

            if (version.ForceUpdate != 0 && version.ForceUpdate != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionForceUpdateInvalido;
                return respuesta;
            }

            if (version.IsActive != 0 && version.IsActive != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionEstadoInvalido;
                return respuesta;
            }

            if (version.Ispublished != 0 && version.Ispublished != 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionPublicacionInvalida;
                return respuesta;
            }

            if (version.Ispublished == 0 && version.IsActive != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionPublicadaDebeEstarActiva;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(version.ReleaseNotes))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionComentariosVacio;
                return respuesta;
            }

            var operacion = 0;

            var versionFiltro = new VersionAplicacionModelo
            {
                Id = version.Id
            };

            var versionActual = VersionAplicacionDatos.GetVersionAplicacion(ref operacion, versionFiltro);

            if (operacion != 0 || versionActual == null || versionActual.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (versionActual.UpdatedAt != version.UpdatedAt)
            {
                respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            if (version.Ispublished == 0)
            {
                var operacionUltimaVersion = 0;

                var ultimaVersion = VersionAplicacionDatos.GetUltimaVersion(
                    ref operacionUltimaVersion,
                    versionActual
                );

                if (operacionUltimaVersion != 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
                }

                if (ultimaVersion == null || ultimaVersion.Id == 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
                }

                if (ultimaVersion.Id != versionActual.Id)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionVersionAplicacionSoloUltimaPuedePublicarse;
                    return respuesta;
                }
            }

            versionActual.ForceUpdate = version.ForceUpdate;
            versionActual.IsActive = version.IsActive;
            versionActual.Ispublished = version.Ispublished;
            versionActual.ReleaseNotes = version.ReleaseNotes.Trim();
            versionActual.UpdatedAt = version.UpdatedAt;
            versionActual.RutUsuario = version.RutUsuario;

            if (versionActual.Ispublished == 0)
            {
                versionActual.PublishedAt = DateTime.Now.ToString(
                    "yyyy-MM-dd HH:mm:ss.ffffff",
                    CultureInfo.InvariantCulture
                );
            }
            else
            {
                versionActual.PublishedAt = null;
            }

            var resultado = VersionAplicacionDatos.UpdateVersionAplicacion(versionActual);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = versionActual;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoVersionAplicacionActualizar;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case 3:
                    respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case -1:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;

                case -2:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;
            }
        }

        private static bool TryCalcularVersionCode(string versionName, out int versionCode)
        {
            versionCode = 0;

            if (string.IsNullOrWhiteSpace(versionName))
            {
                return false;
            }

            var partes = versionName.Trim().Split('.');

            if (partes.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(partes[0], out var mayor) ||
                !int.TryParse(partes[1], out var menor) ||
                !int.TryParse(partes[2], out var revision))
            {
                return false;
            }

            if (mayor < 0 || mayor > 99 ||
                menor < 0 || menor > 99 ||
                revision < 0 || revision > 99)
            {
                return false;
            }

            versionCode = CalcularVersionCode(mayor, menor, revision);

            return true;
        }

        private static int CalcularVersionCode(int mayor, int menor, int revision)
        {
            return (mayor * 10000) + (menor * 100) + revision;
        }

        private static string NormalizarVersionName(string versionName)
        {
            var partes = versionName.Trim().Split('.');

            var mayor = Convert.ToInt32(partes[0]);
            var menor = Convert.ToInt32(partes[1]);
            var revision = Convert.ToInt32(partes[2]);

            return mayor + "." + menor + "." + revision;
        }

        private static string ConstruirNombreApkEsperado(string fabricante, string modelo, string versionAndroid, string versionName)
        {
            return fabricante.Trim().ToUpper() + "_" +
                   modelo.Trim().ToUpper() + "_" +
                   versionAndroid + "_" +
                   "CEDULA_" +
                   versionName +
                   ".apk";
        }

        private static string CalcularSha256Archivo(string rutaArchivo)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            using (var stream = File.OpenRead(rutaArchivo))
            {
                var hash = sha256.ComputeHash(stream);

                return BitConverter.ToString(hash)
                    .Replace("-", string.Empty)
                    .ToLowerInvariant();
            }
        }

        private static string CopiarApkADescargas(string rutaTemporalApk, string nombreApkFinal)
        {
            var rutaWebDestino = "~/" + ConstantesModelo.ConsDownloadPath.TrimStart('/', '~');

            var carpetaDestino = HttpContext.Current.Server.MapPath(rutaWebDestino);

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            var rutaDestino = Path.Combine(carpetaDestino, nombreApkFinal);

            File.Copy(rutaTemporalApk, rutaDestino, true);

            if (File.Exists(rutaTemporalApk))
            {
                File.Delete(rutaTemporalApk);
            }

            return rutaDestino;
        }

        private static void EliminarArchivoSiExiste(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                return;
            }

            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
        }
    }
}