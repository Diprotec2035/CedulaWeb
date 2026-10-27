using Datos.v1.Categoria;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Categoria;
using System.Collections.Generic;

namespace Negocio.v1.Categoria
{
    public class CategoriaNegocio
    {
        public static RespuestaModelo<List<CategoriaModelo>> GetCategorias(CategoriaFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<CategoriaModelo>>();

            var operacion = 0;
            var categorias = CategoriaDatos.GetReporteCategorias(ref operacion, filtro.Coincidencia, filtro.Estado, filtro.Orden, filtro.Orientacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = categorias;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static List<CategoriaModelo> GetReporteCategorias(ref int operacion, string buscador, short estado, short orden, short orientacion)
        {
            return CategoriaDatos.GetReporteCategorias(ref operacion, buscador, estado, orden, orientacion);
        }

        public static CategoriaModelo GetCategoria(ref int operacion, CategoriaModelo categoria)
        {
            return CategoriaDatos.GetCategoria(ref operacion, categoria);
        }

        public static int InsertCategoria(CategoriaModelo categoria)
        {
            return CategoriaDatos.InsertCategoria(categoria);
        }

        public static int UpdateCategoria(CategoriaModelo categoria)
        {
            return CategoriaDatos.UpdateCategoria(categoria);
        }

        public static RespuestaModelo<CategoriaModelo> RegistrarCategoria(CategoriaModelo categoria)
        {
            var respuesta = new RespuestaModelo<CategoriaModelo>();

            if (string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCategoriaNombreVacio;
                return respuesta;
            }

            categoria.Nombre = categoria.Nombre.Trim();

            if (!categoria.Orden.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCategoriaOrdenVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(categoria.Icono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCategoriaIconoVacio;
                return respuesta;
            }

            categoria.Icono = categoria.Icono.Trim();

            var resultado = CategoriaDatos.InsertCategoria(categoria);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = categoria;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoCategoriaRegistro;
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

        public static RespuestaModelo<CategoriaModelo> ActualizarCategoria(CategoriaModelo categoria)
        {
            var respuesta = new RespuestaModelo<CategoriaModelo>();

            if (categoria.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCategoriaNombreVacio;
                return respuesta;
            }

            categoria.Nombre = categoria.Nombre.Trim();

            if (!categoria.Orden.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCategoriaOrdenVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(categoria.Icono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCategoriaIconoVacio;
                return respuesta;
            }

            categoria.Icono = categoria.Icono.Trim();

            var resultado = CategoriaDatos.UpdateCategoria(categoria);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = categoria;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoCategoriaActualizar;
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

        public static List<CategoriaModelo> GetCategoriaUsuario(ref int operacion, EmpresaUsuarioModelo usuario)
        {
            return CategoriaDatos.GetCategoriaUsuario(ref operacion, usuario);
        }

        public static List<CategoriaModelo> GetCategoriasMenu(ref int operacion)
        {
            return CategoriaDatos.GetCategoriasMenu(ref operacion);
        }

        public static List<CategoriaModelo> GetCategoriasModulosPerfil(ref int operacion)
        {
            return CategoriaDatos.GetCategoriasModulosPerfil(ref operacion);
        }
    }
}