using Datos.v1.SuperAdmin;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.SuperAdmin;
using System;
using System.Collections.Generic;

namespace Negocio.v1.SuperAdmin
{
    public class SuperAdminNegocio
    {
        public static SuperAdminModelo GetSuperAdmin(ref int operacion, SuperAdminModelo superAdmin)
        {
            return SuperAdminDatos.GetSuperAdmin(ref operacion, superAdmin);
        }

        public static RespuestaModelo<SuperAdminModelo> ValidarSuperAdminLogin(SuperAdminModelo superAdmin)
        {
            var respuesta = new RespuestaModelo<SuperAdminModelo>();

            if (string.IsNullOrWhiteSpace(superAdmin.RutAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioRutVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.Password))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPasswordVacio;
                return respuesta;
            }

            var operacion = 0;
            var superAdminExiste = SuperAdminDatos.GetSuperAdmin(ref operacion, superAdmin);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorLogin;
                return respuesta;
            }

            if (superAdminExiste == null || string.IsNullOrWhiteSpace(superAdminExiste.RutAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorLogin;
                return respuesta;
            }

            respuesta.Datos = superAdminExiste;

            if (superAdminExiste.Estado != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioEstado;
                return respuesta;
            }

            if (!PasswordHelperNegocio.VerificarPassword(superAdmin.Password, superAdminExiste.PasswordHash, superAdminExiste.PasswordSalt, superAdminExiste.PasswordAlgoritmo))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioPassword;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = superAdminExiste;
            respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
            respuesta.AlertaTitulo = string.Empty;

            return respuesta;
        }

        public static List<SuperAdminModelo> GetReporteSuperAdmin(ref int operacion, string buscador, sbyte? estado, int campo, int orden)
        {
            return SuperAdminDatos.GetReporteSuperAdmin(ref operacion, buscador, estado, campo, orden);
        }

        public static List<SuperAdminModelo> GetSuperAdmins(SuperAdminFiltroModelo filtro)
        {
            var operacion = 0;
            var buscador = filtro != null && !string.IsNullOrWhiteSpace(filtro.Coincidencia) ? filtro.Coincidencia.Trim() : string.Empty;

            sbyte? estado = null;

            if (filtro != null && filtro.Estado.HasValue && filtro.Estado.Value >= 0)
            {
                estado = Convert.ToSByte(filtro.Estado.Value);
            }

            var orden = filtro != null && filtro.Orden > 0 ? filtro.Orden : 1;
            var orientacion = filtro != null && filtro.Orientacion > 0 ? filtro.Orientacion : 1;

            return SuperAdminDatos.GetReporteSuperAdmin(ref operacion, buscador, estado, orden, orientacion);
        }

        public static RespuestaModelo<SuperAdminModelo> RegistrarSuperAdmin(SuperAdminModelo superAdmin)
        {
            var respuesta = new RespuestaModelo<SuperAdminModelo>();

            if (string.IsNullOrWhiteSpace(superAdmin.RutAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorRutVacio;
                return respuesta;
            }

            if (FuncionesNegocio.ValidateRut(superAdmin.RutAdmin) == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutSuperAdministrador;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.NombreAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.ApellidoAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorApellidoVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorEmailVacio;
                return respuesta;
            }

            superAdmin.Email = superAdmin.Email.Trim().ToLowerInvariant();

            if (!FuncionesNegocio.ValidateEmail(superAdmin.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSuperAdministradorEmail;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.Telefono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorTelefonoVacio;
                return respuesta;
            }

            superAdmin.Telefono = superAdmin.Telefono.Trim();

            if (string.IsNullOrWhiteSpace(superAdmin.Password))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorPasswordVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.PasswordConfirmar))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorPasswordConfirmarVacio;
                return respuesta;
            }

            if (superAdmin.Password != superAdmin.PasswordConfirmar)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorPasswordNoCoincide;
                return respuesta;
            }

            if (!superAdmin.Estado.HasValue)
            {
                superAdmin.Estado = 0;
            }

            var passwordFinal = PasswordHelperNegocio.GenerarPasswordUsuario(superAdmin.Password);

            superAdmin.PasswordHash = passwordFinal.hashHex;
            superAdmin.PasswordSalt = passwordFinal.saltHex;
            superAdmin.PasswordAlgoritmo = passwordFinal.algoritmo;

            var resultado = SuperAdminDatos.InsertSuperAdmin(superAdmin);

            if (resultado == 1)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorExiste;
                return respuesta;
            }

            if (resultado != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = superAdmin;
            respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
            respuesta.AlertaTitulo = string.Empty;
            respuesta.Mensaje = ConstantesModelo.ConsExitoSuperAdministradorRegistro;

            return respuesta;
        }

        public static RespuestaModelo<SuperAdminModelo> ActualizarSuperAdmin(SuperAdminModelo superAdmin)
        {
            var respuesta = new RespuestaModelo<SuperAdminModelo>();

            if (string.IsNullOrWhiteSpace(superAdmin.RutAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorRutVacio;
                return respuesta;
            }

            if (FuncionesNegocio.ValidateRut(superAdmin.RutAdmin) == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutSuperAdministrador;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.NombreAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.ApellidoAdmin))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorApellidoVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorEmailVacio;
                return respuesta;
            }

            superAdmin.Email = superAdmin.Email.Trim().ToLowerInvariant();

            if (!FuncionesNegocio.ValidateEmail(superAdmin.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSuperAdministradorEmail;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(superAdmin.Telefono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorTelefonoVacio;
                return respuesta;
            }

            superAdmin.Telefono = superAdmin.Telefono.Trim();

            if (!superAdmin.Estado.HasValue)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorEstadoVacio;
                return respuesta;
            }

            if (!string.IsNullOrWhiteSpace(superAdmin.Password) || !string.IsNullOrWhiteSpace(superAdmin.PasswordConfirmar))
            {
                if (string.IsNullOrWhiteSpace(superAdmin.Password))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorPasswordVacio;
                    return respuesta;
                }

                if (string.IsNullOrWhiteSpace(superAdmin.PasswordConfirmar))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorPasswordConfirmarVacio;
                    return respuesta;
                }

                if (superAdmin.Password != superAdmin.PasswordConfirmar)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorPasswordNoCoincide;
                    return respuesta;
                }

                var passwordFinal = PasswordHelperNegocio.GenerarPasswordUsuario(superAdmin.Password);

                superAdmin.PasswordHash = passwordFinal.hashHex;
                superAdmin.PasswordSalt = passwordFinal.saltHex;
                superAdmin.PasswordAlgoritmo = passwordFinal.algoritmo;
            }
            else
            {
                superAdmin.PasswordHash = string.Empty;
                superAdmin.PasswordSalt = string.Empty;
                superAdmin.PasswordAlgoritmo = string.Empty;
            }

            var resultado = SuperAdminDatos.UpdateSuperAdmin(superAdmin);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = superAdmin;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoSuperAdministradorActualizar;
                    return respuesta;

                case 1:
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionSuperAdministradorNoExiste;
                    return respuesta;

                case 3:
                    respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaRegistroModificado;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;
            }
        }
    }
}