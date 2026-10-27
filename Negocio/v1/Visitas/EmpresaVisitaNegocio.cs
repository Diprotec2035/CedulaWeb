using Datos.v1.Visitas;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Visitas;
using System.Collections.Generic;

namespace Negocio.v1.Visitas
{
    public class EmpresaVisitaNegocio
    {
        public static RespuestaModelo<List<EmpresaVisitaModelo>> GetEmpresaVisitas(EmpresaVisitaFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<EmpresaVisitaModelo>>();

            var operacion = 0;
            var visitas = EmpresaVisitaDatos.GetReporteEmpresaVisitas(ref operacion, filtro.RutEmpresa, filtro.Coincidencia, filtro.Vigente, filtro.Orden, filtro.Orientacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = visitas;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static EmpresaVisitaModelo GetEmpresaVisita(ref int operacion, EmpresaVisitaModelo empresaVisita)
        {
            return EmpresaVisitaDatos.GetEmpresaVisita(ref operacion, empresaVisita);
        }

        public static RespuestaModelo<EmpresaVisitaModelo> RegistrarEmpresaVisita(EmpresaVisitaModelo empresaVisita)
        {
            var respuesta = new RespuestaModelo<EmpresaVisitaModelo>();

            if (string.IsNullOrWhiteSpace(empresaVisita.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaRutVacio;
                return respuesta;
            }

            string rutNormalizado;

            if (!FuncionesNegocio.TryNormalizarRut(empresaVisita.Rut, out rutNormalizado))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaRutInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresaVisita.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresaVisita.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            empresaVisita.Rut = rutNormalizado;
            empresaVisita.Nombre = empresaVisita.Nombre.Trim();
            empresaVisita.Empresa = empresaVisita.Empresa != null ? empresaVisita.Empresa.Trim() : string.Empty;

            if (empresaVisita.Nombre.Length > ConstantesModelo.ConsVisitaNombreLargoMaximo)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaNombreLargo;
                return respuesta;
            }

            if (empresaVisita.Empresa.Length > ConstantesModelo.ConsVisitaEmpresaLargoMaximo)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaEmpresaLarga;
                return respuesta;
            }

            var operacion = 0;

            var empresaVisitaActual = EmpresaVisitaDatos.GetEmpresaVisita(
                ref operacion,
                new EmpresaVisitaModelo
                {
                    Rut = empresaVisita.Rut,
                    RutEmpresa = empresaVisita.RutEmpresa
                }
            );

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoRegistrar;
                return respuesta;
            }

            if (empresaVisitaActual != null && !string.IsNullOrWhiteSpace(empresaVisitaActual.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaRutDuplicado;
                return respuesta;
            }

            var resultado = EmpresaVisitaDatos.InsertEmpresaVisita(empresaVisita);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = empresaVisita;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoVisitaRegistro;
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

        public static RespuestaModelo<EmpresaVisitaModelo> ActualizarEmpresaVisita(EmpresaVisitaModelo empresaVisita)
        {
            var respuesta = new RespuestaModelo<EmpresaVisitaModelo>();

            if (string.IsNullOrWhiteSpace(empresaVisita.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaRutVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresaVisita.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresaVisita.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            empresaVisita.Rut = empresaVisita.Rut.Trim();
            empresaVisita.Nombre = empresaVisita.Nombre.Trim();
            empresaVisita.Empresa = empresaVisita.Empresa != null ? empresaVisita.Empresa.Trim() : string.Empty;

            if (empresaVisita.Nombre.Length > ConstantesModelo.ConsVisitaNombreLargoMaximo)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaNombreLargo;
                return respuesta;
            }

            if (empresaVisita.Empresa.Length > ConstantesModelo.ConsVisitaEmpresaLargoMaximo)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaEmpresaLarga;
                return respuesta;
            }

            var operacion = 0;

            var empresaVisitaActual = EmpresaVisitaDatos.GetEmpresaVisita(
                ref operacion,
                new EmpresaVisitaModelo
                {
                    Rut = empresaVisita.Rut,
                    RutEmpresa = empresaVisita.RutEmpresa
                }
            );

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoActualizar;
                return respuesta;
            }

            if (empresaVisitaActual == null || string.IsNullOrWhiteSpace(empresaVisitaActual.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRegistroObtener;
                return respuesta;
            }

            var resultado = EmpresaVisitaDatos.UpdateEmpresaVisita(empresaVisita);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = empresaVisita;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoVisitaActualizar;
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
