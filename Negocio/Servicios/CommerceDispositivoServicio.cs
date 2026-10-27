using Modelo;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Negocio.Servicios
{
    public class CommerceDispositivoServicio
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient ClienteHttp = CrearClienteHttp();

        public class ValidacionResultado
        {
            public bool ConsultaProcesada { get; set; }

            public bool DispositivoValido { get; set; }

            public int Estado { get; set; }

            public string Mensaje { get; set; }

            public string CodigoError { get; set; }

            public string CorrelationId { get; set; }

            public string Identificador { get; set; }

            public int? Retorno { get; set; }
        }

        private class CommerceRespuestaModelo<T>
        {
            public int Estado { get; set; }

            public string Respuesta { get; set; }

            public T Data { get; set; }

            public string CodigoError { get; set; }

            public string CorrelationId { get; set; }
        }

        private class CommerceDispositivoModelo
        {
            public string Identificador { get; set; }

            public int Retorno { get; set; }
        }

        public static async Task<ValidacionResultado> ValidarDispositivoAsync(string partNumber, string serialNumber)
        {
            var resultado = new ValidacionResultado
            {
                ConsultaProcesada = false,
                DispositivoValido = false,
                Estado = 0,
                Mensaje = string.Empty,
                CodigoError = string.Empty,
                CorrelationId = string.Empty,
                Identificador = string.Empty,
                Retorno = null
            };

            partNumber = partNumber != null ? partNumber.Trim() : string.Empty;
            serialNumber = serialNumber != null ? serialNumber.Trim() : string.Empty;

            if (string.IsNullOrWhiteSpace(partNumber) || string.IsNullOrWhiteSpace(serialNumber))
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceDispositivoDatosVacios;
                return resultado;
            }

            var apiBaseUrl = ConstantesModelo.ConsCommerceApiUrl;
            var apiEndpoint = ConstantesModelo.ConsCommerceApiEPGetDispositivo;
            var apiHeaderKey = ConstantesModelo.ConsCommerceApiHeaderKey;
            var apiHeaderValue = ConstantesModelo.ConsCommerceApiHeaderValue;

            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceApiUrlNoConfigurada;
                Logger.Error(ConstantesModelo.ConsLogCommerceApiUrlNoConfigurada);

                return resultado;
            }

            if (string.IsNullOrWhiteSpace(apiEndpoint))
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceApiEPNoConfigurado;
                Logger.Error(ConstantesModelo.ConsLogCommerceApiEPNoConfigurado);

                return resultado;
            }

            if (string.IsNullOrWhiteSpace(apiHeaderKey))
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceApiHeaderKeyNoConfigurado;
                Logger.Error(ConstantesModelo.ConsLogCommerceApiHeaderKeyNoConfigurado);

                return resultado;
            }

            if (string.IsNullOrWhiteSpace(apiHeaderValue))
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceApiHeaderValueNoConfigurado;
                Logger.Error(ConstantesModelo.ConsLogCommerceApiHeaderValueNoConfigurado);

                return resultado;
            }

            apiBaseUrl = apiBaseUrl.Trim().TrimEnd('/');
            apiEndpoint = apiEndpoint.Trim();
            apiHeaderKey = apiHeaderKey.Trim();
            apiHeaderValue = apiHeaderValue.Trim();

            if (!apiEndpoint.StartsWith(ConstantesModelo.ConsHttpInicio))
            {
                apiEndpoint = ConstantesModelo.ConsHttpInicio + apiEndpoint;
            }

            var url = apiBaseUrl + apiEndpoint + "?partNumber=" + Uri.EscapeDataString(partNumber) + "&serialNumber=" + Uri.EscapeDataString(serialNumber);

            try
            {
                using (var solicitudHttp = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    if (!solicitudHttp.Headers.TryAddWithoutValidation(apiHeaderKey, apiHeaderValue))
                    {
                        resultado.Mensaje = ConstantesModelo.ConsCommerceApiValidacionError;
                        Logger.Error(ConstantesModelo.ConsLogCommerceApiHeaderNoAgregado);

                        return resultado;
                    }

                    using (var respuestaHttp = await ClienteHttp.SendAsync(solicitudHttp).ConfigureAwait(false))
                    {
                        var contenido = await respuestaHttp.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (string.IsNullOrWhiteSpace(contenido))
                        {
                            resultado.Estado = Convert.ToInt32(respuestaHttp.StatusCode);
                            resultado.Mensaje = ConstantesModelo.ConsCommerceApiRespuestaVacia;

                            Logger.Error(ConstantesModelo.ConsLogCommerceApiRespuestaVacia, Convert.ToInt32(respuestaHttp.StatusCode));

                            return resultado;
                        }

                        CommerceRespuestaModelo<CommerceDispositivoModelo> respuestaCommerce;

                        try
                        {
                            respuestaCommerce = JsonConvert.DeserializeObject<CommerceRespuestaModelo<CommerceDispositivoModelo>>(contenido);
                        }
                        catch (JsonException ex)
                        {
                            resultado.Estado = Convert.ToInt32(respuestaHttp.StatusCode);
                            resultado.Mensaje = ConstantesModelo.ConsCommerceApiRespuestaInvalida;

                            Logger.Error(ex, ConstantesModelo.ConsLogCommerceApiRespuestaInvalida, Convert.ToInt32(respuestaHttp.StatusCode));

                            return resultado;
                        }

                        if (respuestaCommerce == null)
                        {
                            resultado.Estado = Convert.ToInt32(respuestaHttp.StatusCode);
                            resultado.Mensaje = ConstantesModelo.ConsCommerceApiRespuestaInvalida;

                            Logger.Error(ConstantesModelo.ConsLogCommerceApiRespuestaNula, Convert.ToInt32(respuestaHttp.StatusCode));

                            return resultado;
                        }

                        resultado.Estado = respuestaCommerce.Estado;
                        resultado.Mensaje = respuestaCommerce.Respuesta ?? string.Empty;
                        resultado.CodigoError = respuestaCommerce.CodigoError ?? string.Empty;
                        resultado.CorrelationId = respuestaCommerce.CorrelationId ?? string.Empty;

                        var codigoHttp = Convert.ToInt32(respuestaHttp.StatusCode);

                        var errorTecnico =
                            codigoHttp == 401 ||
                            codigoHttp == 403 ||
                            codigoHttp == 408 ||
                            codigoHttp == 429 ||
                            codigoHttp >= 500;

                        if (errorTecnico)
                        {
                            if (string.IsNullOrWhiteSpace(resultado.Mensaje))
                            {
                                resultado.Mensaje = ConstantesModelo.ConsCommerceApiValidacionError;
                            }

                            Logger.Error(
                                ConstantesModelo.ConsLogCommerceApiErrorTecnico,
                                codigoHttp,
                                resultado.Estado,
                                resultado.CodigoError,
                                resultado.CorrelationId
                            );

                            return resultado;
                        }

                        resultado.ConsultaProcesada = true;

                        if (respuestaCommerce.Data != null)
                        {
                            resultado.Identificador = respuestaCommerce.Data.Identificador ?? string.Empty;
                            resultado.Retorno = respuestaCommerce.Data.Retorno;
                        }

                        resultado.DispositivoValido =
                            respuestaHttp.IsSuccessStatusCode &&
                            respuestaCommerce.Estado == 200 &&
                            respuestaCommerce.Data != null &&
                            respuestaCommerce.Data.Retorno == 0;

                        if (!resultado.DispositivoValido && string.IsNullOrWhiteSpace(resultado.Mensaje))
                        {
                            resultado.Mensaje = ConstantesModelo.ConsCommerceDispositivoNoValidado;
                        }

                        return resultado;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceApiTimeout;

                Logger.Error(ex, ConstantesModelo.ConsLogCommerceApiTimeout);

                return resultado;
            }
            catch (HttpRequestException ex)
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceApiComunicacionError;

                Logger.Error(ex, ConstantesModelo.ConsLogCommerceApiComunicacionError);

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Mensaje = ConstantesModelo.ConsCommerceApiValidacionError;

                Logger.Error(ex, ConstantesModelo.ConsLogCommerceApiErrorInesperado);

                return resultado;
            }
        }

        private static HttpClient CrearClienteHttp()
        {
            var cliente = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(ConstantesModelo.ConsCommerceApiTimeoutSegundos)
            };

            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return cliente;
        }
    }
}