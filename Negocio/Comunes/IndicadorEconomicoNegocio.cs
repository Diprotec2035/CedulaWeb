using Modelo;
using Modelo.Comunes;
using Negocio.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Negocio.Comunes
{
    public class IndicadorEconomicoNegocio
    {
        public static async Task<RespuestaModelo<List<IndicadorEconomicoModelo>>> GetIndicadoresHome()
        {
            var respuesta = new RespuestaModelo<List<IndicadorEconomicoModelo>>
            {
                Exitoso = false,
                Datos = new List<IndicadorEconomicoModelo>(),
                Mensaje = string.Empty
            };

            try
            {
                var indicadores = await IndicadorEconomicoServicio.GetIndicadoresAsync();

                respuesta.Exitoso = true;
                respuesta.Datos = indicadores;

                if (indicadores == null || indicadores.Count == 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorIndicadoresEconomicosObtener;
                }
            }
            catch (Exception)
            {
                respuesta.Exitoso = false;
                respuesta.Datos = new List<IndicadorEconomicoModelo>();
                respuesta.Mensaje = ConstantesModelo.ConsErrorIndicadoresEconomicosObtener;
            }

            return respuesta;
        }
    }
}
