using Modelo;
using Modelo.v1.Modulo;
using Negocio.v1.Modulo;
using System;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.helpers
{
    public static class ModuloContextoHelper
    {
        private const string PrefijoCacheContexto = "MODULO_CONTEXTO_PAGINA_";

        public static ModuloContextoModelo ValidarModuloPagina(Page page)
        {
            var contexto = new ModuloContextoModelo();

            if (page == null)
            {
                contexto.Mensaje = ConstantesModelo.ConsValidacionModuloContextoError;
                return contexto;
            }

            var modIdTexto = Convert.ToString(page.Request.QueryString["mod_id"]);

            if (string.IsNullOrWhiteSpace(modIdTexto))
            {
                contexto.Mensaje = ConstantesModelo.ConsValidacionModuloContextoIdVacio;
                return contexto;
            }

            uint moduloId;

            if (!uint.TryParse(modIdTexto, out moduloId) || moduloId == 0)
            {
                contexto.Mensaje = ConstantesModelo.ConsValidacionModuloContextoIdInvalido;
                return contexto;
            }

            var urlReal = ObtenerUrlReal(page);
            var urlPublica = ObtenerUrlPublica(page);

            var claveCache =
                PrefijoCacheContexto +
                moduloId +
                "|" +
                (urlReal ?? string.Empty).ToUpperInvariant() +
                "|" +
                (urlPublica ?? string.Empty).ToUpperInvariant();

            var itemsPeticion = HttpContext.Current?.Items;

            if (itemsPeticion != null)
            {
                var contextoCache = itemsPeticion[claveCache] as ModuloContextoModelo;

                if (contextoCache != null)
                {
                    return contextoCache;
                }
            }

            contexto = ModuloNegocio.GetModuloContexto(moduloId, urlReal, urlPublica);

            if (contexto == null)
            {
                contexto = new ModuloContextoModelo
                {
                    Mensaje = ConstantesModelo.ConsValidacionModuloContextoError
                };
            }

            if (!contexto.Exitoso && string.IsNullOrWhiteSpace(contexto.Mensaje))
            {
                contexto.Mensaje = ConstantesModelo.ConsValidacionModuloContextoError;
            }

            if (itemsPeticion != null)
            {
                itemsPeticion[claveCache] = contexto;
            }

            return contexto;
        }

        public static string ObtenerTituloPagina(Page page)
        {
            var contexto = ValidarModuloPagina(page);

            if (contexto == null || !contexto.Exitoso || string.IsNullOrWhiteSpace(contexto.TituloPagina))
            {
                return string.Empty;
            }

            return contexto.TituloPagina.Trim();
        }

        public static string ObtenerTituloContenido(Page page)
        {
            var contexto = ValidarModuloPagina(page);

            if (contexto == null || !contexto.Exitoso || string.IsNullOrWhiteSpace(contexto.TituloContenido))
            {
                return string.Empty;
            }

            return contexto.TituloContenido.Trim();
        }

        public static void MostrarAlerta(Page page, string mensaje)
        {
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);

            var script =
                "$('#global-loader').hide();" +
                "mostrarAlerta('warning', '', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(page, page.GetType(), "MostrarAlerta", script, true);
        }

        private static string ObtenerUrlReal(Page page)
        {
            if (page?.Request?.Url == null)
            {
                return string.Empty;
            }

            return page.Request.Url.AbsolutePath;
        }

        private static string ObtenerUrlPublica(Page page)
        {
            var urlPublica = Convert.ToString(page.RouteData.Values["url"]);

            if (!string.IsNullOrWhiteSpace(urlPublica))
            {
                return urlPublica;
            }

            var rawUrl = Convert.ToString(page.Request.RawUrl);

            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                return string.Empty;
            }

            var indexQuery = rawUrl.IndexOf("?", StringComparison.Ordinal);

            if (indexQuery >= 0)
            {
                rawUrl = rawUrl.Substring(0, indexQuery);
            }

            rawUrl = rawUrl.Trim('/');

            if (rawUrl.Contains("/"))
            {
                return string.Empty;
            }

            return rawUrl;
        }
    }
}