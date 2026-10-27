using Modelo;
using Modelo.Comunes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Negocio.Servicios
{
    public class IndicadorEconomicoServicio
    {
        private const int TimeoutSegundos = 10;
        private const int DiasPeriodoFallback = 10;

        private static readonly object CacheLock = new object();

        private static List<IndicadorEconomicoModelo> indicadoresCache;
        private static DateTime fechaCache = DateTime.MinValue;

        public static async Task<List<IndicadorEconomicoModelo>> GetIndicadoresAsync()
        {
            var indicadoresDesdeCache = ObtenerIndicadoresDesdeCache();

            if (indicadoresDesdeCache != null)
            {
                return indicadoresDesdeCache;
            }

            var indicadores = new List<IndicadorEconomicoModelo>();

            var apiKey = ConstantesModelo.ConsCMFApiKey;
            var baseUrl = ConstantesModelo.ConsCMFApiUrl;

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(baseUrl))
            {
                return indicadores;
            }

            baseUrl = baseUrl.TrimEnd('/');

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

            using (var cliente = new HttpClient())
            {
                cliente.Timeout = TimeSpan.FromSeconds(TimeoutSegundos);

                await AgregarIndicadorCmfAsync(
                    cliente,
                    indicadores,
                    baseUrl,
                    apiKey,
                    "uf",
                    "UF",
                    "UF");

                await AgregarIndicadorCmfAsync(
                    cliente,
                    indicadores,
                    baseUrl,
                    apiKey,
                    "dolar",
                    "Dólar observado",
                    "Pesos");

                await AgregarIndicadorCmfAsync(
                    cliente,
                    indicadores,
                    baseUrl,
                    apiKey,
                    "euro",
                    "Euro",
                    "Pesos");

                await AgregarIndicadorCmfAsync(
                    cliente,
                    indicadores,
                    baseUrl,
                    apiKey,
                    "utm",
                    "UTM",
                    "Pesos");
            }

            if (indicadores.Count > 0)
            {
                GuardarIndicadoresEnCache(indicadores);
            }

            return indicadores;
        }

        private static async Task AgregarIndicadorCmfAsync(
            HttpClient cliente,
            List<IndicadorEconomicoModelo> indicadores,
            string baseUrl,
            string apiKey,
            string codigo,
            string nombreMostrar,
            string unidadMedida)
        {
            try
            {
                var item = await ObtenerItemIndicadorActualAsync(
                    cliente,
                    baseUrl,
                    apiKey,
                    codigo);

                if (item == null)
                {
                    item = await ObtenerItemIndicadorPeriodoAsync(
                        cliente,
                        baseUrl,
                        apiKey,
                        codigo);
                }

                if (item == null)
                {
                    return;
                }

                var valorTexto = Convert.ToString(item["Valor"]);
                var fechaTexto = Convert.ToString(item["Fecha"]);

                var valor = ParsearDecimalChileno(valorTexto);
                var fecha = ParsearFecha(fechaTexto);

                if (valor <= 0)
                {
                    return;
                }

                indicadores.Add(new IndicadorEconomicoModelo
                {
                    Codigo = codigo,
                    Nombre = nombreMostrar,
                    UnidadMedida = unidadMedida,
                    Valor = valor,
                    Fecha = fecha,
                    ValorFormateado = FormatearValor(codigo, valor)
                });
            }
            catch (Exception)
            {
                return;
            }
        }

        private static async Task<JToken> ObtenerItemIndicadorActualAsync(
            HttpClient cliente,
            string baseUrl,
            string apiKey,
            string codigo)
        {
            var url = ConstruirUrlIndicadorActual(baseUrl, codigo, apiKey);

            return await ObtenerItemMasRecienteDesdeUrlAsync(cliente, url);
        }

        private static async Task<JToken> ObtenerItemIndicadorPeriodoAsync(
            HttpClient cliente,
            string baseUrl,
            string apiKey,
            string codigo)
        {
            var fechaFin = DateTime.Now;
            var fechaInicio = fechaFin.AddDays(-DiasPeriodoFallback);

            var url = ConstruirUrlIndicadorPeriodo(
                baseUrl,
                codigo,
                apiKey,
                fechaInicio,
                fechaFin);

            return await ObtenerItemMasRecienteDesdeUrlAsync(cliente, url);
        }

        private static async Task<JToken> ObtenerItemMasRecienteDesdeUrlAsync(HttpClient cliente, string url)
        {
            var respuesta = await cliente.GetAsync(url);

            if (!respuesta.IsSuccessStatusCode)
            {
                return null;
            }

            var contenido = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contenido))
            {
                return null;
            }

            contenido = contenido.Trim();

            if (!contenido.StartsWith("{"))
            {
                return null;
            }

            var json = JObject.Parse(contenido);

            if (json["CodigoError"] != null)
            {
                return null;
            }

            return ObtenerItemMasReciente(json);
        }

        private static string ConstruirUrlIndicadorActual(string baseUrl, string codigo, string apiKey)
        {
            return baseUrl +
                   "/" +
                   codigo +
                   "?apikey=" +
                   Uri.EscapeDataString(apiKey) +
                   "&formato=json";
        }

        private static string ConstruirUrlIndicadorPeriodo(
            string baseUrl,
            string codigo,
            string apiKey,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            return baseUrl +
                   "/" +
                   codigo +
                   "/periodo/" +
                   fechaInicio.Year.ToString("0000") +
                   "/" +
                   fechaInicio.Month.ToString("00") +
                   "/dias_i/" +
                   fechaInicio.Day.ToString("00") +
                   "/" +
                   fechaFin.Year.ToString("0000") +
                   "/" +
                   fechaFin.Month.ToString("00") +
                   "/dias_f/" +
                   fechaFin.Day.ToString("00") +
                   "?apikey=" +
                   Uri.EscapeDataString(apiKey) +
                   "&formato=json";
        }

        private static JToken ObtenerItemMasReciente(JObject json)
        {
            JToken itemMasReciente = null;
            DateTime fechaMasReciente = DateTime.MinValue;

            foreach (var propiedad in json.Properties())
            {
                var arreglo = propiedad.Value as JArray;

                if (arreglo == null || arreglo.Count == 0)
                {
                    continue;
                }

                foreach (var item in arreglo)
                {
                    var fechaTexto = Convert.ToString(item["Fecha"]);
                    var fecha = ParsearFecha(fechaTexto);

                    if (fecha == DateTime.MinValue)
                    {
                        continue;
                    }

                    if (itemMasReciente == null || fecha > fechaMasReciente)
                    {
                        itemMasReciente = item;
                        fechaMasReciente = fecha;
                    }
                }
            }

            return itemMasReciente;
        }

        private static decimal ParsearDecimalChileno(string valorTexto)
        {
            if (string.IsNullOrWhiteSpace(valorTexto))
            {
                return 0;
            }

            decimal valor;

            if (decimal.TryParse(
                valorTexto,
                NumberStyles.Any,
                new CultureInfo("es-CL"),
                out valor))
            {
                return valor;
            }

            if (decimal.TryParse(
                valorTexto,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out valor))
            {
                return valor;
            }

            return 0;
        }

        private static DateTime ParsearFecha(string fechaTexto)
        {
            if (string.IsNullOrWhiteSpace(fechaTexto))
            {
                return DateTime.MinValue;
            }

            DateTime fecha;

            if (DateTime.TryParse(
                fechaTexto,
                new CultureInfo("es-CL"),
                DateTimeStyles.None,
                out fecha))
            {
                return fecha;
            }

            if (DateTime.TryParse(
                fechaTexto,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fecha))
            {
                return fecha;
            }

            return DateTime.MinValue;
        }

        private static string FormatearValor(string codigo, decimal valor)
        {
            var cultura = new CultureInfo("es-CL");

            return valor.ToString("C2", cultura);
        }

        private static List<IndicadorEconomicoModelo> ObtenerIndicadoresDesdeCache()
        {
            var cacheMinutos = ObtenerCacheMinutos();

            if (cacheMinutos <= 0)
            {
                return null;
            }

            lock (CacheLock)
            {
                if (indicadoresCache == null || indicadoresCache.Count == 0)
                {
                    return null;
                }

                if (fechaCache.AddMinutes(cacheMinutos) <= DateTime.Now)
                {
                    return null;
                }

                return ClonarIndicadores(indicadoresCache);
            }
        }

        private static void GuardarIndicadoresEnCache(List<IndicadorEconomicoModelo> indicadores)
        {
            var cacheMinutos = ObtenerCacheMinutos();

            if (cacheMinutos <= 0)
            {
                return;
            }

            lock (CacheLock)
            {
                indicadoresCache = ClonarIndicadores(indicadores);
                fechaCache = DateTime.Now;
            }
        }

        private static int ObtenerCacheMinutos()
        {
            var cacheMinutosTexto = Environment.GetEnvironmentVariable(ConstantesModelo.ConsCMFIndicadorEconomico);

            int cacheMinutos;

            if (!int.TryParse(cacheMinutosTexto, out cacheMinutos))
            {
                return 0;
            }

            if (cacheMinutos <= 0)
            {
                return 0;
            }

            return cacheMinutos;
        }

        private static List<IndicadorEconomicoModelo> ClonarIndicadores(List<IndicadorEconomicoModelo> indicadores)
        {
            var copia = new List<IndicadorEconomicoModelo>();

            foreach (var indicador in indicadores)
            {
                copia.Add(new IndicadorEconomicoModelo
                {
                    Codigo = indicador.Codigo,
                    Nombre = indicador.Nombre,
                    UnidadMedida = indicador.UnidadMedida,
                    Valor = indicador.Valor,
                    Fecha = indicador.Fecha,
                    ValorFormateado = indicador.ValorFormateado
                });
            }

            return copia;
        }
    }
}