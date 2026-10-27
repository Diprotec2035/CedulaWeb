using System.Collections.Generic;
using Modelo;
using Datos;
using Modelo.v1.Empresa;
using Modelo.Comunes;

namespace Negocio
{
    public class EmpresaNegocio
    {
        public static List<EmpresaModelo> GetReporteEmpresas(ref int operacion, string buscador, short estado, short orden, short orientacion)
        {
            return EmpresaDatos.GetReporteEmpresas(ref operacion, buscador, estado, orden, orientacion);
        }

        public static EmpresaModelo GetEmpresa(ref int operacion, EmpresaModelo empresa)
        {
            return EmpresaDatos.GetEmpresa(ref operacion, empresa);
        }

        public static RespuestaModelo<List<EmpresaModelo>> GetEmpresas(EmpresaFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<EmpresaModelo>>();

            var operacion = 0;
            var dispositivos = EmpresaDatos.GetReporteEmpresas(ref operacion, filtro.Coincidencia, filtro.Estado, filtro.Orden, filtro.Orientacion);

            switch (operacion)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = dispositivos;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                    return respuesta;
            }
        }

        public static int UpdateEmpresa(EmpresaModelo empresa)
        {
            return EmpresaDatos.UpdateEmpresa(empresa);
        }

        public static int InsertEmpresa(EmpresaModelo empresa)
        {
            return EmpresaDatos.InsertEmpresa(empresa);
        }

        public static RespuestaModelo<EmpresaModelo> RegistrarEmpresa(EmpresaModelo empresa)
        {
            var respuesta = new RespuestaModelo<EmpresaModelo>();

            if (string.IsNullOrWhiteSpace(empresa.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaRutVacio;
                return respuesta;
            }

            var isValidRut = FuncionesNegocio.ValidateRut(empresa.Rut);

            if (isValidRut == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutEmpresa;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Direccion))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaDireccionVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Ciudad))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaCiudadVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Comuna))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaComunaVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Contacto))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaContactoVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.ContactoTelefono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaContactoTelefonoVacio;
                return respuesta;
            }

            empresa.ContactoTelefono = empresa.ContactoTelefono.Trim();

            if (string.IsNullOrWhiteSpace(empresa.ContactoEmail))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaContactoEmailVacio;
                return respuesta;
            }

            empresa.ContactoEmail = empresa.ContactoEmail.Trim().ToLowerInvariant();

            if (!FuncionesNegocio.ValidateEmail(empresa.ContactoEmail))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaContactoEmail;
                return respuesta;
            }

            var operacion = 0;

            var empresaFiltro = new EmpresaModelo
            {
                Rut = empresa.Rut,
                Estado = null
            };

            var empresaExiste = EmpresaDatos.GetEmpresa(ref operacion, empresaFiltro);

            if (operacion != 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (!string.IsNullOrWhiteSpace(empresaExiste.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaExiste;
                return respuesta;
            }

            var resultado = EmpresaDatos.InsertEmpresa(empresa);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = empresa;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoEmpresaRegistro;
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

        public static RespuestaModelo<EmpresaModelo> ActualizarEmpresa(EmpresaModelo empresa)
        {
            var respuesta = new RespuestaModelo<EmpresaModelo>();

            if (string.IsNullOrWhiteSpace(empresa.Rut))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaRutVacio;
                return respuesta;
            }

            var isValidRut = FuncionesNegocio.ValidateRut(empresa.Rut);

            if (isValidRut == "0")
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutEmpresa;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Nombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaNombreVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Direccion))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaDireccionVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Ciudad))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaCiudadVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Comuna))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaComunaVacia;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.Contacto))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaContactoVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresa.ContactoTelefono))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaContactoTelefonoVacio;
                return respuesta;
            }

            empresa.ContactoTelefono = empresa.ContactoTelefono.Trim();

            if (string.IsNullOrWhiteSpace(empresa.ContactoEmail))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaContactoEmailVacio;
                return respuesta;
            }

            empresa.ContactoEmail = empresa.ContactoEmail.Trim().ToLowerInvariant();

            if (!FuncionesNegocio.ValidateEmail(empresa.ContactoEmail))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaContactoEmail;
                return respuesta;
            }

            var resultado = EmpresaDatos.UpdateEmpresa(empresa);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = empresa;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoEmpresaActualizar;
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