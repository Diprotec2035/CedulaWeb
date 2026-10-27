using Datos.v1.Bitacora;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Bitacora;
using System;
using System.Collections.Generic;

namespace Negocio.v1.Bitacora
{
    public class BitacoraNegocio
    {
        public static RespuestaModelo<List<BitacoraModelo>> GetBitacora(string storedProcedure, string parametro, string idRegistro)
        {
            var respuesta = new RespuestaModelo<List<BitacoraModelo>>
            {
                Exitoso = false,
                Datos = new List<BitacoraModelo>(),
                Mensaje = string.Empty
            };

            try
            {
                if (string.IsNullOrWhiteSpace(storedProcedure) ||
                    string.IsNullOrWhiteSpace(parametro) ||
                    string.IsNullOrWhiteSpace(idRegistro))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBitacoraObtener;
                    return respuesta;
                }

                var operacion = 0;

                var lista = BitacoraDatos.GetBitacora(
                    ref operacion,
                    storedProcedure,
                    parametro,
                    idRegistro
                );

                if (operacion < 0)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBitacoraObtener;
                    return respuesta;
                }

                respuesta.Exitoso = true;
                respuesta.Datos = lista;
            }
            catch (Exception)
            {
                respuesta.Exitoso = false;
                respuesta.Datos = new List<BitacoraModelo>();
                respuesta.Mensaje = ConstantesModelo.ConsErrorBitacoraObtener;
            }

            return respuesta;
        }
    }
}