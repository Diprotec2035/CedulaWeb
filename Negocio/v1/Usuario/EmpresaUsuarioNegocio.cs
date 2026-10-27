using Datos;
using Datos.Comunes;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Usuario;
using System;
using System.Collections.Generic;

namespace Negocio
{
    public class EmpresaUsuarioNegocio
    {
        public static List<EmpresaUsuarioModelo> GetReporteUsuarios(ref int operacion, string rutEmpresa, string buscador, short estado, short orden, short orientacion)
        {
            return EmpresaUsuarioDatos.GetReporteUsuarios(ref operacion, rutEmpresa, buscador, estado, orden, orientacion);
        }

        public static EmpresaUsuarioModelo GetEmpresaUsuario(ref int operacion, EmpresaUsuarioModelo user)
        {
            return EmpresaUsuarioDatos.GetEmpresaUsuario(ref operacion, user);
        }

        public static int UpdateEmpresaUsuario(EmpresaUsuarioModelo empresaUsuario)
        {
            return EmpresaUsuarioDatos.UpdateEmpresaUsuario(empresaUsuario);
        }

        public static int InsertEmpresaUsuario(EmpresaUsuarioModelo empresaUsuario)
        {
            var passwordFinal = PasswordHelperNegocio.GenerarPasswordUsuario(empresaUsuario.Password);

            empresaUsuario.PasswordHash = passwordFinal.hashHex;
            empresaUsuario.PasswordSalt = passwordFinal.saltHex;
            empresaUsuario.PasswordAlgoritmo = passwordFinal.algoritmo;

            return EmpresaUsuarioDatos.InsertEmpresaUsuario(empresaUsuario);
        }

        public static int UpdateUsuarioPassword(EmpresaUsuarioModelo usuario)
        {
            var passwordFinal = PasswordHelperNegocio.GenerarPasswordUsuario(usuario.Password);

            usuario.PasswordHash = passwordFinal.hashHex;
            usuario.PasswordSalt = passwordFinal.saltHex;
            usuario.PasswordAlgoritmo = passwordFinal.algoritmo;

            return EmpresaUsuarioDatos.UpdateUsuarioPassword(usuario);
        }

        public static int SendUsuarioCredenciales(EmpresaUsuarioModelo usuario)
        {
            try
            {
                var body = ConstantesModelo.ConsRecuperarPassword;

                body = body.Replace("##usuario##", usuario.NombreUsuario.ToUpper() + " " + usuario.ApellidoUsuario.ToUpper());
                body = body.Replace("##fecha##", DateTime.Now.ToString("dd-MM-yyyy"));
                body = body.Replace("##nueva_password##", usuario.Password);

                var asunto = ConstantesModelo.ConsTituloWebPage + " | " + ConstantesModelo.ConsEmailSubjectRecuperacion;

                return FuncionesDatos.SendEmail(usuario.Email, asunto, body);
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(SendUsuarioCredenciales), "Envío de correo de recuperación de credenciales. Email: " + FuncionesDatos.TextoSql(usuario.Email));
                return -1;
            }
        }

        public static int SendNuevoUsuarioCredenciales(EmpresaUsuarioModelo usuario)
        {
            try
            {
                var body = ConstantesModelo.ConsNuevoUsuario;

                body = body.Replace("##usuario##", usuario.NombreUsuario.ToUpper() + " " + usuario.ApellidoUsuario.ToUpper());
                body = body.Replace("##fecha##", DateTime.Now.ToString("dd-MM-yyyy"));
                body = body.Replace("##website_nombre##", ConstantesModelo.ConsWebsiteNombre);
                body = body.Replace("##url_plataforma##", ConstantesModelo.ConsUrl);
                body = body.Replace("##rut_empresa##", usuario.RutEmpresa);
                body = body.Replace("##rut_usuario##", usuario.RutUsuario);
                body = body.Replace("##email_usuario##", usuario.Email);
                body = body.Replace("##password##", usuario.Password);

                var asunto = ConstantesModelo.ConsTituloWebPage + " | " + ConstantesModelo.ConsEmailSubjectNuevoUsuario;

                return FuncionesDatos.SendEmail(usuario.Email, asunto, body);
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(SendNuevoUsuarioCredenciales), "Envío de correo de nuevo usuario. Empresa: " + FuncionesDatos.TextoSql(usuario.RutEmpresa) + ", Usuario: " + FuncionesDatos.TextoSql(usuario.RutUsuario) + ", Email: " + FuncionesDatos.TextoSql(usuario.Email));
                return -1;
            }
        }

        public static RespuestaModelo<List<EmpresaUsuarioModelo>> GetUsuarios(EmpresaUsuarioFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<EmpresaUsuarioModelo>>();

            var operacion = 0;
            var usuarios = EmpresaUsuarioDatos.GetReporteUsuarios(ref operacion, filtro.RutEmpresa, filtro.Coincidencia, filtro.Estado, filtro.Orden, filtro.Orientacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = usuarios;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<EmpresaUsuarioModelo> RegistrarUsuario(EmpresaUsuarioModelo usuario)
        {
            var respuesta = new RespuestaModelo<EmpresaUsuarioModelo>();

            if (string.IsNullOrWhiteSpace(usuario.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioRutVacio;
                return respuesta;
            }

            var isValidRut = FuncionesNegocio.ValidateRut(usuario.RutUsuario);

            if (isValidRut == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutUsuario;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.ApellidoUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioApellidoVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPasswordVacio;
                return respuesta;
            }

            if (usuario.Password != usuario.PasswordConfirmar)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPasswordNoCoincide;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.Telefono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioTelefonoVacio;
                return respuesta;
            }

            usuario.Telefono = usuario.Telefono.Trim();

            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioEmailVacio;
                return respuesta;
            }

            usuario.Email = usuario.Email.Trim().ToLowerInvariant();

            if (!FuncionesNegocio.ValidateEmail(usuario.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioEmail;
                return respuesta;
            }

            if (usuario.IdPerfil == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPerfilVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.RutUsuarioBitacora))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            var operacion = 0;
            var usuarioExiste = EmpresaUsuarioDatos.GetEmpresaUsuario(ref operacion, usuario);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (!string.IsNullOrWhiteSpace(usuarioExiste.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioExiste;
                return respuesta;
            }

            var passwordTemporal = usuario.Password;

            var passwordFinal = PasswordHelperNegocio.GenerarPasswordUsuario(passwordTemporal);
            usuario.PasswordHash = passwordFinal.hashHex;
            usuario.PasswordSalt = passwordFinal.saltHex;
            usuario.PasswordAlgoritmo = passwordFinal.algoritmo;

            var resultado = EmpresaUsuarioDatos.InsertEmpresaUsuario(usuario);

            switch (resultado)
            {
                case 0:
                    usuario.Password = passwordTemporal;

                    if (SendNuevoUsuarioCredenciales(usuario) != 0)
                    {
                        respuesta.Exitoso = true;
                        respuesta.Datos = usuario;
                        respuesta.Mensaje = ConstantesModelo.ConsAdvertenciaNuevoUsuarioEnvioMail;
                        respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorWarning;
                        respuesta.AlertaTitulo = string.Empty;
                        return respuesta;
                    }

                    respuesta.Exitoso = true;
                    respuesta.Datos = usuario;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoUsuarioRegistro;
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

        public static RespuestaModelo<EmpresaUsuarioModelo> ActualizarUsuario(EmpresaUsuarioModelo usuario)
        {
            var respuesta = new RespuestaModelo<EmpresaUsuarioModelo>();

            if (string.IsNullOrWhiteSpace(usuario.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioRutVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.ApellidoUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioApellidoVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.Telefono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioTelefonoVacio;
                return respuesta;
            }

            usuario.Telefono = usuario.Telefono.Trim();

            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioEmailVacio;
                return respuesta;
            }

            usuario.Email = usuario.Email.Trim().ToLowerInvariant();

            if (!FuncionesNegocio.ValidateEmail(usuario.Email))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorUsuarioEmail;
                return respuesta;
            }

            if (usuario.IdPerfil == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPerfilVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(usuario.RutUsuarioBitacora))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            if (usuario.EsEdicionMisDatos && !string.Equals(usuario.RutUsuario.Trim(), usuario.RutUsuarioBitacora.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorPermisoModificarRegistro;
                return respuesta;
            }

            var actualizarPassword = !string.IsNullOrWhiteSpace(usuario.Password) || !string.IsNullOrWhiteSpace(usuario.PasswordConfirmar);

            if (actualizarPassword)
            {
                if (string.IsNullOrWhiteSpace(usuario.Password))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPasswordVacio;
                    return respuesta;
                }

                if (string.IsNullOrWhiteSpace(usuario.PasswordConfirmar))
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPasswordVacio;
                    return respuesta;
                }

                if (usuario.Password != usuario.PasswordConfirmar)
                {
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionUsuarioPasswordNoCoincide;
                    return respuesta;
                }

                var passwordFinal = PasswordHelperNegocio.GenerarPasswordUsuario(usuario.Password);
                usuario.PasswordHash = passwordFinal.hashHex;
                usuario.PasswordSalt = passwordFinal.saltHex;
                usuario.PasswordAlgoritmo = passwordFinal.algoritmo;
            }

            var resultado = EmpresaUsuarioDatos.UpdateEmpresaUsuario(usuario);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = usuario;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoUsuarioActualizar;
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