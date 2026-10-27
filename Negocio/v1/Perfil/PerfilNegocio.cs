using Datos.v1.Perfil;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Perfil;
using System.Collections.Generic;

namespace Negocio.v1.Perfil
{
    public class PerfilNegocio
    {
        public static List<PerfilModelo> GetReportePerfiles(ref int operacion, string rutEmpresa, string buscador, short estado, short orden, short orientacion)
        {
            return PerfilDatos.GetReportePerfiles(ref operacion, rutEmpresa, buscador, estado, orden, orientacion);
        }

        public static PerfilModelo GetPerfil(ref int operacion, PerfilModelo perfil)
        {
            return PerfilDatos.GetPerfil(ref operacion, perfil);
        }

        public static RespuestaModelo<List<PerfilModelo>> GetPerfiles(PerfilFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<PerfilModelo>>();

            var operacion = 0;
            var perfiles = PerfilDatos.GetReportePerfiles(ref operacion, filtro.RutEmpresa, filtro.Coincidencia, filtro.Estado, filtro.Orden, filtro.Orientacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = perfiles;
                     respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static int UpdatePerfil(PerfilModelo perfil)
        {
            return PerfilDatos.UpdatePerfil(perfil);
        }

        public static int InsertPerfil(PerfilModelo perfil)
        {
            return PerfilDatos.InsertPerfil(perfil);
        }

        public static RespuestaModelo<PerfilModelo> RegistrarPerfil(PerfilModelo perfil)
        {
            var respuesta = new RespuestaModelo<PerfilModelo>();

            if (string.IsNullOrWhiteSpace(perfil.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionPerfilNombreVacio;
                return respuesta;
            }

            perfil.Nombre = perfil.Nombre.Trim();

            if (perfil.IdRegla == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionPerfilReglaVacia;
                return respuesta;
            }

            var resultado = PerfilDatos.InsertPerfil(perfil);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = perfil;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoPerfilRegistro;
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

        public static RespuestaModelo<PerfilModelo> ActualizarPerfil(PerfilModelo perfil)
        {
            var respuesta = new RespuestaModelo<PerfilModelo>();

            if (string.IsNullOrWhiteSpace(perfil.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionPerfilNombreVacio;
                return respuesta;
            }

            perfil.Nombre = perfil.Nombre.Trim();

            if (perfil.IdRegla == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionPerfilReglaVacia;
                return respuesta;
            }

            var resultado = PerfilDatos.UpdatePerfil(perfil);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = perfil;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoPerfilActualizar;
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

                case -3:
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionPerfilUsuarioVigente;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;
            }
        }
    }
}