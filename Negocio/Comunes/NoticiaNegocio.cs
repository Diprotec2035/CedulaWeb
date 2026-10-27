using Modelo;
using Modelo.Comunes;
using Negocio.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Negocio.Comunes
{
    public class NoticiaNegocio
    {
        public static async Task<RespuestaModelo<List<NoticiaModelo>>> GetNoticiasHome()
        {
            var respuesta = new RespuestaModelo<List<NoticiaModelo>>
            {
                Exitoso = false,
                Datos = new List<NoticiaModelo>(),
                Mensaje = string.Empty
            };

            try
            {
                var noticias = await NoticiaServicio.GetNoticiasAsync();

                respuesta.Exitoso = true;
                respuesta.Datos = noticias;

                if (noticias == null || noticias.Count == 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorNoticiasObtener;
                }
            }
            catch (Exception)
            {
                respuesta.Exitoso = false;
                respuesta.Datos = new List<NoticiaModelo>();
                respuesta.Mensaje = ConstantesModelo.ConsErrorNoticiasObtener;
            }

            return respuesta;
        }
    }
}
