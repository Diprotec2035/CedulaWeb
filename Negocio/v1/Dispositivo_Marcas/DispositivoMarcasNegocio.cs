using Datos.v1.DispositivoMarcas;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.DispositivoMarcas;
using System.Collections.Generic;

namespace Negocio.v1.DispositivoMarcas
{
    public class DispositivoMarcasNegocio
    {
        public static RespuestaModelo<List<DispositivoMarcasModelo>> GetReporteDispositivoMarcas(DispositivoMarcasFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<DispositivoMarcasModelo>>();
            var operacion = 0;
            var marcas = DispositivoMarcasDatos.GetReporteDispositivoMarcas(ref operacion, filtro.Coincidencia, filtro.Estado, filtro.Orden, filtro.Orientacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = marcas;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<DispositivoMarcasModelo> RegistrarDispositivoMarca(DispositivoMarcasModelo marca)
        {
            var respuesta = new RespuestaModelo<DispositivoMarcasModelo>();

            if (string.IsNullOrWhiteSpace(marca.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoMarcasNombreVacio;
                return respuesta;
            }

            marca.Nombre = marca.Nombre.Trim();

            var resultado = DispositivoMarcasDatos.InsertDispositivoMarca(marca);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = marca;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoMarcaRegistro;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                case -1:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;

                case -2:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorBaseDatos;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                    return respuesta;
            }
        }

        public static DispositivoMarcasModelo GetDispositivoMarca(ref int operacion, DispositivoMarcasModelo marca)
        {
            return DispositivoMarcasDatos.GetDispositivoMarca(ref operacion, marca);
        }

        public static RespuestaModelo<DispositivoMarcasModelo> ActualizarDispositivoMarca(DispositivoMarcasModelo marca)
        {
            var respuesta = new RespuestaModelo<DispositivoMarcasModelo>();

            if (string.IsNullOrWhiteSpace(marca.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionDispositivoMarcasNombreVacio;
                return respuesta;
            }

            marca.Nombre = marca.Nombre.Trim();

            var resultado = DispositivoMarcasDatos.UpdateDispositivoMarca(marca);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = marca;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoMarcaActualizar;
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
    }
}