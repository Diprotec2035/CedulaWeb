using Modelo;
using Modelo.v1.ModuloPerfil;
using Negocio.v1.ModuloPerfil;
using System;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.helpers
{
    public class PermisoModuloHelper
    {
        public static PermisoModuloUsuarioModelo ObtenerPermisoModulo(Page page, string urlModuloPadre)
        {
            if (page == null)
            {
                return CrearPermisoDenegado();
            }

            var contextoModulo = ModuloContextoHelper.ValidarModuloPagina(page);

            if (contextoModulo == null || !contextoModulo.Exitoso || contextoModulo.ModuloId == 0)
            {
                return CrearPermisoDenegado();
            }

            var rutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]);
            var rutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]);
            var esSuperAdmin = Convert.ToString(HttpContext.Current.Session["superAdmin"]) == "1";

            var respuesta = ModuloPerfilNegocio.GetPermisoModuloUsuario(rutEmpresa, rutUsuario, urlModuloPadre, esSuperAdmin);

            if (!respuesta.Exitoso || respuesta.Datos == null)
            {
                return CrearPermisoDenegado();
            }

            return respuesta.Datos;
        }

        public static bool TienePermisoVer(Page page, string urlModuloPadre)
        {
            var permiso = ObtenerPermisoModulo(page, urlModuloPadre);

            return permiso.Estado == 0;
        }

        public static bool TienePermisoConsulta(Page page, string urlModuloPadre)
        {
            var permiso = ObtenerPermisoModulo(page, urlModuloPadre);

            return permiso.Estado == 0 && permiso.Consulta == 0;
        }

        public static bool TienePermisoNuevo(Page page, string urlModuloPadre)
        {
            var permiso = ObtenerPermisoModulo(page, urlModuloPadre);

            return permiso.Estado == 0 && permiso.Nuevo == 0;
        }

        public static bool TienePermisoModificar(Page page, string urlModuloPadre)
        {
            var permiso = ObtenerPermisoModulo(page, urlModuloPadre);

            return permiso.Estado == 0 && permiso.Modifica == 0;
        }

        public static bool ValidarPuedeVer(Page page, string urlModuloPadre, string urlRetorno)
        {
            if (TienePermisoVer(page, urlModuloPadre))
            {
                return true;
            }

            RedirigirSinPermiso(page, urlRetorno, ConstantesModelo.ConsErrorPermisoAccesoModulo);

            return false;
        }

        public static bool ValidarPuedeConsulta(Page page, string urlModuloPadre, string urlRetorno, string mensaje)
        {
            if (TienePermisoConsulta(page, urlModuloPadre))
            {
                return true;
            }

            RedirigirSinPermiso(page, urlRetorno, mensaje);

            return false;
        }

        public static bool ValidarPuedeNuevo(Page page, string urlModuloPadre, string urlRetorno, string mensaje)
        {
            if (TienePermisoNuevo(page, urlModuloPadre))
            {
                return true;
            }

            RedirigirSinPermiso(page, urlRetorno, mensaje);

            return false;
        }

        public static bool ValidarPuedeModificar(Page page, string urlModuloPadre, string urlRetorno, string mensaje)
        {
            if (TienePermisoModificar(page, urlModuloPadre))
            {
                return true;
            }

            RedirigirSinPermiso(page, urlRetorno, mensaje);

            return false;
        }

        public static void RedirigirSinPermiso(Page page, string urlRetorno, string mensaje)
        {
            HttpContext.Current.Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
            HttpContext.Current.Session["AlertaTitulo"] = string.Empty;
            HttpContext.Current.Session["AlertaMensaje"] = mensaje;

            page.Response.Redirect(urlRetorno, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public static string ScriptSinPermiso(string mensaje)
        {
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);

            return "mostrarAlerta('warning', '', '" + mensajeJs + "');";
        }

        private static PermisoModuloUsuarioModelo CrearPermisoDenegado()
        {
            return new PermisoModuloUsuarioModelo
            {
                IdModulo = 0,
                NombreModulo = string.Empty,
                Estado = 1,
                Consulta = 1,
                Nuevo = 1,
                Modifica = 1
            };
        }
    }
}