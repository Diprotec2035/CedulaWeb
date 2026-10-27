using Datos.v1.Visitas;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Visitas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Negocio.v1.Visitas
{
    public class EmpresaVisitaCargaNegocio
    {
        public static RespuestaModelo<List<EmpresaVisitaCargaModelo>> GetEmpresaVisitaCarga(EmpresaVisitaCargaFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<EmpresaVisitaCargaModelo>>
            {
                Exitoso = false,
                Datos = new List<EmpresaVisitaCargaModelo>(),
                Mensaje = string.Empty
            };

            if (filtro == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorGenericoConsultar;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(filtro.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionEmpresa;
                return respuesta;
            }

            filtro.Coincidencia = filtro.Coincidencia != null ? filtro.Coincidencia.Trim() : string.Empty;
            filtro.Desde = filtro.Desde != null ? filtro.Desde.Trim() : string.Empty;
            filtro.Hasta = filtro.Hasta != null ? filtro.Hasta.Trim() : string.Empty;

            var listado = EmpresaVisitaCargaDatos.GetEmpresaVisitaCarga(filtro);

            respuesta.Exitoso = true;
            respuesta.Datos = listado;
            return respuesta;
        }

        public static RespuestaModelo<EmpresaVisitaCargaModelo> GetEmpresaVisitaCargaId(uint idCarga, string rutEmpresa)
        {
            var respuesta = new RespuestaModelo<EmpresaVisitaCargaModelo>
            {
                Exitoso = false,
                Datos = null,
                Mensaje = string.Empty
            };

            if (idCarga == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaCargaIdInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionEmpresa;
                return respuesta;
            }

            var carga = EmpresaVisitaCargaDatos.GetEmpresaVisitaCargaId(idCarga, rutEmpresa);

            if (carga == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaCargaIdInvalido;
                return respuesta;
            }

            respuesta.Exitoso = true;
            respuesta.Datos = carga;
            return respuesta;
        }

        public static RespuestaModelo<List<EmpresaVisitaCargaDetalleModelo>> GetEmpresaVisitaCargaDetalle(EmpresaVisitaCargaDetalleFiltroModelo filtro)
        {
            var respuesta = new RespuestaModelo<List<EmpresaVisitaCargaDetalleModelo>>
            {
                Exitoso = false,
                Datos = new List<EmpresaVisitaCargaDetalleModelo>(),
                Mensaje = string.Empty
            };

            if (filtro == null || filtro.IdCarga == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaCargaIdInvalido;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(filtro.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionEmpresa;
                return respuesta;
            }

            filtro.Coincidencia = filtro.Coincidencia != null ? filtro.Coincidencia.Trim() : string.Empty;

            var listado = EmpresaVisitaCargaDatos.GetEmpresaVisitaCargaDetalle(filtro);

            respuesta.Exitoso = true;
            respuesta.Datos = listado;
            return respuesta;
        }

        public static RespuestaModelo<EmpresaVisitaCargaModelo> RegistrarEmpresaVisitaCarga(EmpresaVisitaCargaModelo carga)
        {
            var respuesta = new RespuestaModelo<EmpresaVisitaCargaModelo>
            {
                Exitoso = false,
                Datos = null,
                Mensaje = string.Empty
            };

            if (carga == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaRegistro;
                return respuesta;
            }

            carga.RutEmpresa = carga.RutEmpresa != null ? carga.RutEmpresa.Trim() : string.Empty;
            carga.ArchivoNombre = carga.ArchivoNombre != null ? carga.ArchivoNombre.Trim() : string.Empty;
            carga.ArchivoRuta = carga.ArchivoRuta != null ? carga.ArchivoRuta.Trim() : string.Empty;
            carga.CreatedBy = carga.CreatedBy != null ? carga.CreatedBy.Trim() : string.Empty;

            if (string.IsNullOrWhiteSpace(carga.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionEmpresa;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(carga.ArchivoNombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCargaArchivoVacio;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(carga.CreatedBy))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionUsuario;
                return respuesta;
            }

            carga.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
            carga.TotalFilas = 0;
            carga.TotalOk = 0;
            carga.TotalError = 0;
            carga.Estado = ConstantesModelo.ConsCargaEstadoProcesando;

            uint idCarga;
            var resultado = EmpresaVisitaCargaDatos.InsertEmpresaVisitaCarga(carga, out idCarga);

            switch (resultado)
            {
                case 0:
                    carga.Id = idCarga;

                    respuesta.Exitoso = true;
                    respuesta.Datos = carga;
                    respuesta.Mensaje = string.Empty;
                    return respuesta;

                case -1:
                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaRegistro;
                    return respuesta;
            }
        }

        public static RespuestaModelo<EmpresaVisitaCargaDetalleModelo> RegistrarEmpresaVisitaCargaDetalle(EmpresaVisitaCargaDetalleModelo detalle)
        {
            var respuesta = new RespuestaModelo<EmpresaVisitaCargaDetalleModelo>
            {
                Exitoso = false,
                Datos = null,
                Mensaje = string.Empty
            };

            if (detalle == null || detalle.IdCarga == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaCargaIdInvalido;
                return respuesta;
            }

            if (detalle.Fila <= 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaRegistro;
                return respuesta;
            }

            detalle.Rut = detalle.Rut != null ? detalle.Rut.Trim() : string.Empty;
            detalle.Nombre = detalle.Nombre != null ? detalle.Nombre.Trim() : string.Empty;
            detalle.Empresa = detalle.Empresa != null ? detalle.Empresa.Trim() : string.Empty;
            detalle.NombreAnterior = detalle.NombreAnterior != null ? detalle.NombreAnterior.Trim() : string.Empty;
            detalle.EmpresaAnterior = detalle.EmpresaAnterior != null ? detalle.EmpresaAnterior.Trim() : string.Empty;
            detalle.Mensaje = detalle.Mensaje != null ? detalle.Mensaje.Trim() : string.Empty;

            var resultado = EmpresaVisitaCargaDatos.InsertEmpresaVisitaCargaDetalle(detalle);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = detalle;
                    return respuesta;

                case -1:
                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaRegistro;
                    return respuesta;
            }
        }

        public static RespuestaModelo<EmpresaVisitaCargaModelo> ActualizarEmpresaVisitaCargaResultado(EmpresaVisitaCargaModelo carga)
        {
            var respuesta = new RespuestaModelo<EmpresaVisitaCargaModelo>
            {
                Exitoso = false,
                Datos = null,
                Mensaje = string.Empty
            };

            if (carga == null || carga.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaCargaIdInvalido;
                return respuesta;
            }

            var resultado = EmpresaVisitaCargaDatos.UpdateEmpresaVisitaCargaResultado(carga);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = carga;
                    return respuesta;

                case 1:
                    respuesta.Mensaje = ConstantesModelo.ConsValidacionVisitaCargaIdInvalido;
                    return respuesta;

                case -1:
                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaRegistro;
                    return respuesta;
            }
        }

        public static RespuestaModelo<EmpresaVisitaCargaModelo> ProcesarCargaVisitasCsv(EmpresaVisitaCargaModelo carga, Stream archivoCsv)
        {
            var respuesta = new RespuestaModelo<EmpresaVisitaCargaModelo>
            {
                Exitoso = false,
                Datos = null,
                Mensaje = string.Empty,
                AlertaColor = ConstantesModelo.ConsAlertaColorDanger,
                AlertaTitulo = string.Empty
            };

            if (carga == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaRegistro;
                return respuesta;
            }

            carga.RutEmpresa = carga.RutEmpresa != null ? carga.RutEmpresa.Trim() : string.Empty;
            carga.ArchivoNombre = carga.ArchivoNombre != null ? carga.ArchivoNombre.Trim() : string.Empty;
            carga.ArchivoRuta = carga.ArchivoRuta != null ? carga.ArchivoRuta.Trim() : string.Empty;
            carga.CreatedBy = carga.CreatedBy != null ? carga.CreatedBy.Trim() : string.Empty;

            if (string.IsNullOrWhiteSpace(carga.RutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionEmpresa;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(carga.CreatedBy))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorSesionUsuario;
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(carga.ArchivoNombre))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCargaArchivoVacio;
                return respuesta;
            }

            var extension = Path.GetExtension(carga.ArchivoNombre);

            if (string.IsNullOrWhiteSpace(extension) ||
                !extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCargaArchivoInvalido;
                return respuesta;
            }

            if (archivoCsv == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCargaArchivoVacio;
                return respuesta;
            }

            List<string> lineas;

            try
            {
                lineas = LeerLineasCsv(archivoCsv);
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCargaArchivoInvalido;
                return respuesta;
            }

            if (lineas == null || lineas.Count == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCargaSinRegistros;
                return respuesta;
            }

            var encabezado = ObtenerColumnasCsv(lineas[0]);

            if (!TieneEncabezadoVisitaValido(encabezado))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaEncabezadoInvalido;
                return respuesta;
            }

            if (!TieneFilasVisita(lineas))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionCargaSinRegistros;
                return respuesta;
            }

            var respuestaCarga = RegistrarEmpresaVisitaCarga(carga);

            if (!respuestaCarga.Exitoso)
            {
                respuesta.Mensaje = respuestaCarga.Mensaje;
                return respuesta;
            }

            carga = respuestaCarga.Datos;

            var rutsArchivo = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            try
            {
                for (var indice = 1; indice < lineas.Count; indice++)
                {
                    var numeroFila = indice + 1;
                    var linea = lineas[indice];

                    if (string.IsNullOrWhiteSpace(linea))
                    {
                        continue;
                    }

                    carga.TotalFilas++;

                    var columnas = ObtenerColumnasCsv(linea);

                    var rut = ObtenerColumna(columnas, 0);
                    var nombre = ObtenerColumna(columnas, 1);
                    var empresa = ObtenerColumna(columnas, 2);

                    var detalle = new EmpresaVisitaCargaDetalleModelo
                    {
                        IdCarga = carga.Id,
                        Fila = numeroFila,
                        Rut = rut,
                        Nombre = nombre,
                        Empresa = empresa,
                        Accion = ConstantesModelo.ConsCargaDetalleAccionSinAplicar,
                        NombreAnterior = string.Empty,
                        EmpresaAnterior = string.Empty,
                        Estado = ConstantesModelo.ConsCargaDetalleEstadoError,
                        Mensaje = string.Empty
                    };

                    string rutNormalizado;

                    var mensajeValidacion = ValidarFilaVisitaCarga(columnas, rut, nombre, empresa, out rutNormalizado);

                    if (!string.IsNullOrWhiteSpace(mensajeValidacion))
                    {
                        detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoError;
                        detalle.Mensaje = mensajeValidacion;

                        RegistrarEmpresaVisitaCargaDetalle(detalle);

                        carga.TotalError++;
                        continue;
                    }

                    detalle.Rut = rutNormalizado;

                    int filaPrevia;

                    if (rutsArchivo.TryGetValue(rutNormalizado, out filaPrevia))
                    {
                        detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoError;
                        detalle.Mensaje = string.Format(ConstantesModelo.ConsValidacionVisitaCargaRutDuplicado, filaPrevia);

                        RegistrarEmpresaVisitaCargaDetalle(detalle);

                        carga.TotalError++;
                        continue;
                    }

                    rutsArchivo.Add(rutNormalizado, numeroFila);

                    AplicarVisitaCarga(carga.RutEmpresa, carga.CreatedBy, detalle);

                    if (detalle.Estado == ConstantesModelo.ConsCargaDetalleEstadoRegistrado)
                    {
                        carga.TotalOk++;
                    }
                    else
                    {
                        carga.TotalError++;
                    }

                    RegistrarEmpresaVisitaCargaDetalle(detalle);
                }

                carga.Estado = carga.TotalError == 0
                    ? ConstantesModelo.ConsCargaEstadoFinalizada
                    : ConstantesModelo.ConsCargaEstadoFinalizadaConErrores;

                var respuestaResultado = ActualizarEmpresaVisitaCargaResultado(carga);

                if (!respuestaResultado.Exitoso)
                {
                    respuesta.Mensaje = respuestaResultado.Mensaje;
                    respuesta.Datos = carga;
                    return respuesta;
                }

                respuesta.Exitoso = true;
                respuesta.Datos = carga;
                respuesta.AlertaColor = carga.TotalError == 0 ? ConstantesModelo.ConsAlertaColorSuccess : ConstantesModelo.ConsAlertaColorWarning;
                respuesta.AlertaTitulo = string.Empty;
                respuesta.Mensaje = string.Format(
                    ConstantesModelo.ConsExitoVisitaCargaProcesada,
                    carga.TotalFilas,
                    carga.TotalOk,
                    carga.TotalError
                );

                return respuesta;
            }
            catch
            {
                carga.Estado = ConstantesModelo.ConsCargaEstadoFallida;

                ActualizarEmpresaVisitaCargaResultado(carga);

                respuesta.Exitoso = false;
                respuesta.Datos = carga;
                respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorDanger;
                respuesta.AlertaTitulo = string.Empty;
                respuesta.Mensaje = ConstantesModelo.ConsErrorVisitaCargaProcesamiento;

                return respuesta;
            }
        }

        private static void AplicarVisitaCarga(string rutEmpresa, string rutUsuario, EmpresaVisitaCargaDetalleModelo detalle)
        {
            var operacion = 0;

            var visita = EmpresaVisitaNegocio.GetEmpresaVisita(
                ref operacion,
                new EmpresaVisitaModelo
                {
                    RutEmpresa = rutEmpresa,
                    Rut = detalle.Rut
                }
            );

            if (operacion != 0)
            {
                detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoError;
                detalle.Mensaje = ConstantesModelo.ConsErrorVisitaCargaConsultarVisita;
                return;
            }

            if (visita == null || string.IsNullOrWhiteSpace(visita.Rut))
            {
                var respuestaRegistro = EmpresaVisitaNegocio.RegistrarEmpresaVisita(
                    new EmpresaVisitaModelo
                    {
                        RutEmpresa = rutEmpresa,
                        Rut = detalle.Rut,
                        Nombre = detalle.Nombre,
                        Empresa = detalle.Empresa,
                        Vigente = 0,
                        RutUsuario = rutUsuario
                    }
                );

                if (!respuestaRegistro.Exitoso)
                {
                    detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoError;
                    detalle.Mensaje = !string.IsNullOrWhiteSpace(respuestaRegistro.Mensaje)
                        ? respuestaRegistro.Mensaje
                        : ConstantesModelo.ConsErrorVisitaCargaRegistrarVisita;
                    return;
                }

                detalle.Accion = ConstantesModelo.ConsCargaDetalleAccionInsertada;
                detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoRegistrado;
                detalle.Mensaje = ConstantesModelo.ConsExitoVisitaCargaInsertada;
                return;
            }

            var nombreActual = NormalizarValorVisita(visita.Nombre);
            var empresaActual = NormalizarValorVisita(visita.Empresa);

            var cambiaNombre = !string.Equals(nombreActual, NormalizarValorVisita(detalle.Nombre), StringComparison.Ordinal);
            var cambiaEmpresa = !string.Equals(empresaActual, NormalizarValorVisita(detalle.Empresa), StringComparison.Ordinal);

            if (!cambiaNombre && !cambiaEmpresa)
            {
                detalle.Accion = ConstantesModelo.ConsCargaDetalleAccionSinCambios;
                detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoRegistrado;
                detalle.Mensaje = ConstantesModelo.ConsExitoVisitaCargaSinCambios;
                return;
            }

            var respuestaActualizacion = EmpresaVisitaNegocio.ActualizarEmpresaVisita(
                new EmpresaVisitaModelo
                {
                    RutEmpresa = rutEmpresa,
                    Rut = detalle.Rut,
                    Nombre = detalle.Nombre,
                    Empresa = detalle.Empresa,
                    Vigente = visita.Vigente,
                    RutUsuario = rutUsuario,
                    UpdatedAt = visita.UpdatedAt
                }
            );

            if (!respuestaActualizacion.Exitoso)
            {
                detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoError;
                detalle.Mensaje = !string.IsNullOrWhiteSpace(respuestaActualizacion.Mensaje)
                    ? respuestaActualizacion.Mensaje
                    : ConstantesModelo.ConsErrorVisitaCargaActualizarVisita;
                return;
            }

            if (cambiaNombre)
            {
                detalle.NombreAnterior = nombreActual;
            }

            if (cambiaEmpresa)
            {
                detalle.EmpresaAnterior = empresaActual;
            }

            detalle.Accion = ConstantesModelo.ConsCargaDetalleAccionActualizada;
            detalle.Estado = ConstantesModelo.ConsCargaDetalleEstadoRegistrado;
            detalle.Mensaje = ConstantesModelo.ConsExitoVisitaCargaActualizada;
        }

        private static List<string> LeerLineasCsv(Stream archivoCsv)
        {
            var lineas = new List<string>();

            if (archivoCsv.CanSeek)
            {
                archivoCsv.Position = 0;
            }

            byte[] contenidoBytes;

            using (var memoria = new MemoryStream())
            {
                archivoCsv.CopyTo(memoria);
                contenidoBytes = memoria.ToArray();
            }

            string contenido;

            if (contenidoBytes.Length >= 3 &&
                contenidoBytes[0] == 0xEF &&
                contenidoBytes[1] == 0xBB &&
                contenidoBytes[2] == 0xBF)
            {
                contenido = new UTF8Encoding(false).GetString(contenidoBytes, 3, contenidoBytes.Length - 3);
            }
            else
            {
                try
                {
                    contenido = new UTF8Encoding(false, true).GetString(contenidoBytes);
                }
                catch (DecoderFallbackException)
                {
                    contenido = Encoding.GetEncoding(1252).GetString(contenidoBytes);
                }
            }

            using (var reader = new StringReader(contenido))
            {
                string linea;

                while ((linea = reader.ReadLine()) != null)
                {
                    lineas.Add(linea);
                }
            }

            return lineas;
        }

        private static List<string> ObtenerColumnasCsv(string linea)
        {
            var columnas = new List<string>();
            var valor = new StringBuilder();
            var dentroComillas = false;

            if (linea == null)
            {
                return columnas;
            }

            var separador = ObtenerSeparadorCsv(linea);

            for (var i = 0; i < linea.Length; i++)
            {
                var caracter = linea[i];

                if (caracter == '"')
                {
                    if (dentroComillas && i + 1 < linea.Length && linea[i + 1] == '"')
                    {
                        valor.Append('"');
                        i++;
                    }
                    else
                    {
                        dentroComillas = !dentroComillas;
                    }
                }
                else if (caracter == separador && !dentroComillas)
                {
                    columnas.Add(valor.ToString().Trim());
                    valor.Clear();
                }
                else
                {
                    valor.Append(caracter);
                }
            }

            columnas.Add(valor.ToString().Trim());

            return columnas;
        }

        private static char ObtenerSeparadorCsv(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                return ',';
            }

            var cantidadPuntoComa = 0;
            var cantidadComa = 0;
            var dentroComillas = false;

            for (var i = 0; i < linea.Length; i++)
            {
                var caracter = linea[i];

                if (caracter == '"')
                {
                    dentroComillas = !dentroComillas;
                }
                else if (!dentroComillas)
                {
                    if (caracter == ';')
                    {
                        cantidadPuntoComa++;
                    }
                    else if (caracter == ',')
                    {
                        cantidadComa++;
                    }
                }
            }

            return cantidadPuntoComa > cantidadComa ? ';' : ',';
        }

        private static bool TieneEncabezadoVisitaValido(List<string> columnas)
        {
            if (columnas == null || columnas.Count < 3)
            {
                return false;
            }

            var rut = NormalizarTexto(columnas[0]);
            var nombre = NormalizarTexto(columnas[1]);
            var empresa = NormalizarTexto(columnas[2]);

            var rutValido =
                rut == "rut" ||
                rut == "run" ||
                rut == "rut visita" ||
                rut == "rut visitante";

            var nombreValido =
                nombre == "nombre" ||
                nombre == "nombres" ||
                nombre == "nombre completo" ||
                nombre == "nombre y apellido" ||
                nombre == "nombre apellido" ||
                nombre == "nombre visita" ||
                nombre == "nombre visitante";

            var empresaValida =
                empresa == "empresa" ||
                empresa == "empresa visita" ||
                empresa == "empresa visitante";

            return rutValido && nombreValido && empresaValida;
        }

        private static bool TieneFilasVisita(List<string> lineas)
        {
            if (lineas == null || lineas.Count <= 1)
            {
                return false;
            }

            for (var i = 1; i < lineas.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(lineas[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ValidarFilaVisitaCarga(
            List<string> columnas,
            string rut,
            string nombre,
            string empresa,
            out string rutNormalizado
        )
        {
            rutNormalizado = string.Empty;

            if (columnas == null || columnas.Count < 3)
            {
                return ConstantesModelo.ConsValidacionCargaFilaColumnasRequeridas;
            }

            if (string.IsNullOrWhiteSpace(rut))
            {
                return ConstantesModelo.ConsValidacionVisitaCargaRutVacio;
            }

            if (!FuncionesNegocio.TryNormalizarRut(rut, out rutNormalizado))
            {
                return ConstantesModelo.ConsValidacionVisitaCargaRutInvalido;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return ConstantesModelo.ConsValidacionVisitaCargaNombreVacio;
            }

            if (nombre.Length > ConstantesModelo.ConsVisitaCargaNombreLargoMaximo)
            {
                return ConstantesModelo.ConsValidacionVisitaCargaNombreLargo;
            }

            if (string.IsNullOrWhiteSpace(empresa))
            {
                return ConstantesModelo.ConsValidacionVisitaCargaEmpresaVacia;
            }

            if (empresa.Length > ConstantesModelo.ConsVisitaCargaEmpresaLargoMaximo)
            {
                return ConstantesModelo.ConsValidacionVisitaCargaEmpresaLarga;
            }

            return string.Empty;
        }

        private static string ObtenerColumna(List<string> columnas, int indice)
        {
            if (columnas == null || columnas.Count <= indice || columnas[indice] == null)
            {
                return string.Empty;
            }

            return columnas[indice].Trim();
        }

        private static string NormalizarValorVisita(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim().ToUpperInvariant();
        }

        private static string NormalizarTexto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim().ToLower()
                    .Replace("á", "a")
                    .Replace("é", "e")
                    .Replace("í", "i")
                    .Replace("ó", "o")
                    .Replace("ú", "u");
        }
    }
}
