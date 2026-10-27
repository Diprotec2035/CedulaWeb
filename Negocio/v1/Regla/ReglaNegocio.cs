using Datos.v1.Regla;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Regla;
using System.Collections.Generic;

namespace Negocio.v1.Regla
{
    public class ReglaNegocio
    {
        public static List<ReglaModelo> GetReporteReglas(ref int operacion, string rutEmpresa, string buscador, short? estado, short orden, short orientacion)
        {
            return ReglaDatos.GetReporteReglas(ref operacion, rutEmpresa, buscador, estado, orden, orientacion);
        }

        public static ReglaModelo GetRegla(ref int operacion, ReglaModelo regla)
        {
            return ReglaDatos.GetRegla(ref operacion, regla);
        }

        public static RespuestaModelo<List<ReglaModelo>> GetReglas(ReglaFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<ReglaModelo>>();

            var operacion = 0;
            var reglas = ReglaDatos.GetReporteReglas(ref operacion, filtro.RutEmpresa, filtro.Coincidencia, filtro.Estado, filtro.Orden, filtro.Orientacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = reglas;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static RespuestaModelo<ReglaModelo> RegistrarRegla(ReglaModelo regla)
        {
            var respuesta = new RespuestaModelo<ReglaModelo>();

            if (string.IsNullOrWhiteSpace(regla.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionReglaNombreVacio;
                return respuesta;
            }

            var mensajeIndicadores = ValidarIndicadoresRegla(regla);

            if (!string.IsNullOrEmpty(mensajeIndicadores))
            {
                respuesta.Mensaje = mensajeIndicadores;
                return respuesta;
            }

            regla.Nombre = regla.Nombre.Trim();

            var resultado = ReglaDatos.InsertRegla(regla);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = regla;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoReglaRegistro;
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

        public static RespuestaModelo<ReglaModelo> ActualizarRegla(ReglaModelo regla)
        {
            var respuesta = new RespuestaModelo<ReglaModelo>();

            if (string.IsNullOrWhiteSpace(regla.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionReglaNombreVacio;
                return respuesta;
            }

            var mensajeIndicadores = ValidarIndicadoresRegla(regla);

            if (!string.IsNullOrEmpty(mensajeIndicadores))
            {
                respuesta.Mensaje = mensajeIndicadores;
                return respuesta;
            }

            regla.Nombre = regla.Nombre.Trim();

            var resultado = ReglaDatos.UpdateRegla(regla);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = regla;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoReglaActualizar;
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
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionReglaPerfilVigente;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                    return respuesta;
            }
        }

        private static string ValidarIndicadoresRegla(ReglaModelo regla)
        {
            if (!EsIndicadorValido(regla.NombreApellido) ||
                !EsIndicadorValido(regla.Empresa) ||
                !EsIndicadorValido(regla.Patente) ||
                !EsIndicadorValido(regla.Comentario) ||
                !EsIndicadorValido(regla.Fotografia) ||
                !EsIndicadorValido(regla.EntradaSalida) ||
                !EsIndicadorValido(regla.EliminaEnviados))
            {
                return ConstantesModelo.ConsValidacionReglaIndicadorInvalido;
            }

            if (regla.ListaBlancaNegra < 0 || regla.ListaBlancaNegra > 2)
            {
                return ConstantesModelo.ConsValidacionReglaListaBlancaNegraInvalida;
            }

            return string.Empty;
        }

        private static bool EsIndicadorValido(sbyte valor)
        {
            return valor == 0 || valor == 1;
        }
    }
}
