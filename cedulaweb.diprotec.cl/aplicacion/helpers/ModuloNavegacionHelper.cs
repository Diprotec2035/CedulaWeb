using Modelo;
using Modelo.Comunes;
using Modelo.v1.Modulo;
using Negocio.v1.Modulo;
using System;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.helpers
{
    public static class ModuloNavegacionHelper
    {
        private const string PrefijoMantenerFiltrosListado = "MANTENER_FILTROS_LISTADO_";
        private const string PrefijoCacheNavegacionAccion = "MODULO_NAVEGACION_ACCION_";

        public static string GetUrlAccion(Page page, string accion)
        {
            return GetUrlAccion(page, accion, string.Empty);
        }

        public static string GetUrlAccion(Page page, string accion, string parametrosAdicionales)
        {
            if (page == null || string.IsNullOrWhiteSpace(accion))
            {
                GuardarAlertaNavegacion(ConstantesModelo.ConsAlertaColorWarning, string.Empty, ConstantesModelo.ConsValidacionModuloNavegacionAccionNoEncontrada);
                return ConstantesModelo.ConsModulosDefault;
            }

            var contextoModulo = ModuloContextoHelper.ValidarModuloPagina(page);

            if (!contextoModulo.Exitoso)
            {
                GuardarAlertaNavegacion(ConstantesModelo.ConsAlertaColorWarning, string.Empty, string.IsNullOrWhiteSpace(contextoModulo.Mensaje) ? ConstantesModelo.ConsValidacionModuloContextoError : contextoModulo.Mensaje);
                return ConstantesModelo.ConsModulosDefault;
            }

            accion = accion.Trim();

            var respuesta = ObtenerRespuestaNavegacionAccion(contextoModulo.ModuloId, accion);

            if (!respuesta.Exitoso || respuesta.Datos == null)
            {
                GuardarAlertaNavegacion(
                    respuesta != null && !string.IsNullOrWhiteSpace(respuesta.AlertaColor)
                        ? respuesta.AlertaColor
                        : ConstantesModelo.ConsAlertaColorWarning,
                    respuesta != null && respuesta.AlertaTitulo != null
                        ? respuesta.AlertaTitulo
                        : string.Empty,
                    respuesta != null && !string.IsNullOrWhiteSpace(respuesta.Mensaje)
                        ? respuesta.Mensaje
                        : ConstantesModelo.ConsValidacionModuloNavegacionAccionError
                );

                return ConstantesModelo.ConsModulosDefault;
            }

            var navegacion = respuesta.Datos;

            var urlBase = !string.IsNullOrWhiteSpace(navegacion.UrlPublica)
                ? ConstantesModelo.ConsModuloUrl + navegacion.UrlPublica.Trim()
                : navegacion.Url;

            if (string.IsNullOrWhiteSpace(urlBase))
            {
                GuardarAlertaNavegacion(ConstantesModelo.ConsAlertaColorWarning, string.Empty, ConstantesModelo.ConsValidacionModuloNavegacionAccionNoEncontrada);
                return ConstantesModelo.ConsModulosDefault;
            }

            if (accion.Equals(ConstantesModelo.ConsModulosAccionUrlVolver, StringComparison.OrdinalIgnoreCase))
            {
                MarcarMantenerFiltros(navegacion.ModuloIdDestino);
            }

            return ArmarUrlFinal(urlBase, navegacion.ModuloIdDestino, parametrosAdicionales);
        }

        public static string ObtenerTituloAccion(Page page, string accion)
        {
            if (page == null || string.IsNullOrWhiteSpace(accion))
            {
                return string.Empty;
            }

            var contextoModulo = ModuloContextoHelper.ValidarModuloPagina(page);

            if (contextoModulo == null || !contextoModulo.Exitoso)
            {
                return string.Empty;
            }

            var respuesta = ObtenerRespuestaNavegacionAccion(contextoModulo.ModuloId, accion.Trim());

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null || string.IsNullOrWhiteSpace(respuesta.Datos.TituloPagina))
            {
                return string.Empty;
            }

            return respuesta.Datos.TituloPagina.Trim();
        }

        private static RespuestaModelo<ModuloNavegacionAccionModelo> ObtenerRespuestaNavegacionAccion(uint moduloIdOrigen, string accion)
        {
            var accionNormalizada = accion.Trim();
            var claveCache = PrefijoCacheNavegacionAccion + moduloIdOrigen + "_" + accionNormalizada.ToUpperInvariant();
            var itemsPeticion = HttpContext.Current?.Items;

            if (itemsPeticion != null && itemsPeticion[claveCache] is RespuestaModelo<ModuloNavegacionAccionModelo> respuestaCache)
            {
                return respuestaCache;
            }

            var respuesta = ModuloNegocio.GetModuloNavegacionAccion(moduloIdOrigen, accionNormalizada);

            if (itemsPeticion != null)
            {
                itemsPeticion[claveCache] = respuesta;
            }

            return respuesta;
        }

        private static string ArmarUrlFinal(string urlBase, uint moduloId, string parametrosAdicionales)
        {
            var url = urlBase;

            if (!string.IsNullOrWhiteSpace(parametrosAdicionales))
            {
                parametrosAdicionales = parametrosAdicionales.Trim();

                if (parametrosAdicionales.StartsWith("?", StringComparison.Ordinal) || parametrosAdicionales.StartsWith("&", StringComparison.Ordinal))
                {
                    parametrosAdicionales = parametrosAdicionales.Substring(1);
                }

                if (!string.IsNullOrWhiteSpace(parametrosAdicionales))
                {
                    var separadorParametros = url.Contains("?") ? "&" : "?";
                    url += separadorParametros + parametrosAdicionales;
                }
            }

            var separadorModulo = url.Contains("?") ? "&" : "?";

            return url + separadorModulo + "mod_id=" + HttpUtility.UrlEncode(Convert.ToString(moduloId));
        }

        private static void GuardarAlertaNavegacion(string color, string titulo, string mensaje)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return;
            }

            HttpContext.Current.Session["AlertaColor"] = string.IsNullOrWhiteSpace(color)
                ? ConstantesModelo.ConsAlertaColorWarning
                : color;

            HttpContext.Current.Session["AlertaTitulo"] = titulo ?? string.Empty;

            HttpContext.Current.Session["AlertaMensaje"] = string.IsNullOrWhiteSpace(mensaje)
                ? ConstantesModelo.ConsValidacionModuloNavegacionAccionError
                : mensaje;
        }

        private static void MarcarMantenerFiltros(uint moduloIdDestino)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null || moduloIdDestino == 0)
            {
                return;
            }

            HttpContext.Current.Session[PrefijoMantenerFiltrosListado + moduloIdDestino] = "1";
        }
    }
}