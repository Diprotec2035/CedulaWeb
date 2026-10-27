using Datos.v1.ModuloPerfil;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.ModuloPerfil;
using System.Collections.Generic;

namespace Negocio.v1.ModuloPerfil
{
    public class ModuloPerfilNegocio
    {
        public static RespuestaModelo<List<ModuloPerfilModelo>> GetModulosPerfil(ModuloPerfilFiltroModelo moduloPerfil)
        {
            var respuesta = new RespuestaModelo<List<ModuloPerfilModelo>>();

            var operacion = 0;
            var mp = ModuloPerfilDatos.GetModulosPerfil(ref operacion, moduloPerfil.RutEmpresa, moduloPerfil.IdPerfil, moduloPerfil.IdCategoria);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = mp;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<ModuloPerfilModelo> UpdateModuloPerfilEstado(ModuloPerfilModelo moduloPerfil)
        {
            if (!ValidarModuloPerfil(moduloPerfil))
            {
                return CrearRespuestaError(moduloPerfil);
            }

            return CrearRespuestaActualizacion(ModuloPerfilDatos.UpdateModuloPerfilEstado(moduloPerfil), moduloPerfil);
        }

        public static RespuestaModelo<ModuloPerfilModelo> UpdateModuloPerfilNuevo(ModuloPerfilModelo moduloPerfil)
        {
            if (!ValidarModuloPerfil(moduloPerfil))
            {
                return CrearRespuestaError(moduloPerfil);
            }

            return CrearRespuestaActualizacion(ModuloPerfilDatos.UpdateModuloPerfilNuevo(moduloPerfil), moduloPerfil);
        }

        public static RespuestaModelo<ModuloPerfilModelo> UpdateModuloPerfilConsulta(ModuloPerfilModelo moduloPerfil)
        {
            if (!ValidarModuloPerfil(moduloPerfil))
            {
                return CrearRespuestaError(moduloPerfil);
            }

            return CrearRespuestaActualizacion(ModuloPerfilDatos.UpdateModuloPerfilConsulta(moduloPerfil), moduloPerfil);
        }

        public static RespuestaModelo<ModuloPerfilModelo> UpdateModuloPerfilModifica(ModuloPerfilModelo moduloPerfil)
        {
            if (!ValidarModuloPerfil(moduloPerfil))
            {
                return CrearRespuestaError(moduloPerfil);
            }

            return CrearRespuestaActualizacion(ModuloPerfilDatos.UpdateModuloPerfilModifica(moduloPerfil), moduloPerfil);
        }

        private static RespuestaModelo<ModuloPerfilModelo> CrearRespuestaActualizacion(int resultado, ModuloPerfilModelo moduloPerfil)
        {
            var respuesta = new RespuestaModelo<ModuloPerfilModelo>
            {
                Datos = moduloPerfil,
                AlertaTitulo = string.Empty
            };

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoRegistroActualizar;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    return respuesta;

                case 3:
                    respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                    return respuesta;
            }
        }

        private static RespuestaModelo<ModuloPerfilModelo> CrearRespuestaError(ModuloPerfilModelo moduloPerfil)
        {
            return new RespuestaModelo<ModuloPerfilModelo>
            {
                Exitoso = false,
                Datos = moduloPerfil,
                Mensaje = ConstantesModelo.ConsErrorGenericoActualizar,
                AlertaColor = ConstantesModelo.ConsAlertaColorDanger,
                AlertaTitulo = string.Empty
            };
        }

        private static bool ValidarModuloPerfil(ModuloPerfilModelo moduloPerfil)
        {
            if (moduloPerfil == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(moduloPerfil.RutEmpresa))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(moduloPerfil.RutUsuario))
            {
                return false;
            }

            if (moduloPerfil.IdPerfil == 0)
            {
                return false;
            }

            if (moduloPerfil.IdModulo == 0)
            {
                return false;
            }

            if (moduloPerfil.Id > 0 && !moduloPerfil.UpdatedAt.HasValue)
            {
                return false;
            }

            return true;
        }

        public static RespuestaModelo<PermisoModuloUsuarioModelo> GetPermisoModuloUsuario(string rutEmpresa, string rutUsuario, string urlModuloPublica, bool esSuperAdmin = false)
        {
            var respuesta = new RespuestaModelo<PermisoModuloUsuarioModelo>();

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorModuloPerfilEmpresaUsuario;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(rutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorModuloPerfilUsuario;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(urlModuloPublica))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorModuloPerfilModulo;
                return respuesta;
            }

            if (esSuperAdmin)
            {
                respuesta.Exitoso = true;
                respuesta.Datos = new PermisoModuloUsuarioModelo
                {
                    IdModulo = 0,
                    NombreModulo = ConstantesModelo.ConsAdministradorPerfil,
                    Estado = 0,
                    Consulta = 0,
                    Nuevo = 0,
                    Modifica = 0
                };
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                respuesta.AlertaTitulo = string.Empty;
                return respuesta;
            }

            var operacion = 0;
            var permiso = ModuloPerfilDatos.GetPermisoModuloUsuario(ref operacion, rutEmpresa, rutUsuario, urlModuloPublica);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (permiso == null)
            {
                permiso = new PermisoModuloUsuarioModelo
                {
                    IdModulo = 0,
                    NombreModulo = string.Empty,
                    Estado = 1,
                    Consulta = 1,
                    Nuevo = 1,
                    Modifica = 1
                };
            }

            respuesta.Exitoso = true;
            respuesta.Datos = permiso;
            respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
            respuesta.AlertaTitulo = string.Empty;

            return respuesta;
        }
    }
}