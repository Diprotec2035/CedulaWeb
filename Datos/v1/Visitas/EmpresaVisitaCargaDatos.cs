using Datos.Comunes;
using Modelo.v1.Visitas;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Visitas
{
    public class EmpresaVisitaCargaDatos
    {
        public static int InsertEmpresaVisitaCarga(EmpresaVisitaCargaModelo carga, out uint idCarga)
        {
            var retorno = 0;
            idCarga = 0;

            var sql = "SP_INS_EMPRESA_VISITA_CARGA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(carga.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(carga.ArchivoNombre) + "," +
                FuncionesDatos.TextoSql(carga.ArchivoRuta) + "," +
                carga.Estado + "," +
                FuncionesDatos.TextoSql(carga.CreatedAt) + "," +
                FuncionesDatos.TextoSql(carga.CreatedBy) + "," +
                FuncionesDatos.TextoSql(carga.CreatedByNombre) + "," +
                "@P_EVC_ID," +
                "@P_RETORNO";

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", carga.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_EVC_ARCHIVO_NOMBRE", carga.ArchivoNombre);
                        cmd.Parameters.AddWithValue("P_EVC_ARCHIVO_RUTA", string.IsNullOrWhiteSpace(carga.ArchivoRuta) ? (object)DBNull.Value : carga.ArchivoRuta);
                        cmd.Parameters.AddWithValue("P_EVC_ESTADO", carga.Estado);
                        cmd.Parameters.AddWithValue("P_EVC_CREATED_AT", carga.CreatedAt);
                        cmd.Parameters.AddWithValue("P_EVC_CREATED_BY", carga.CreatedBy);
                        cmd.Parameters.AddWithValue("P_EVC_CREATED_BY_NOMBRE", carga.CreatedByNombre);

                        var pEvcId = cmd.Parameters.Add("P_EVC_ID", MySqlDbType.UInt32);
                        pEvcId.Direction = ParameterDirection.Output;

                        var pRetorno = cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte);
                        pRetorno.Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        if (pEvcId.Value != null && pEvcId.Value != DBNull.Value)
                        {
                            idCarga = Convert.ToUInt32(pEvcId.Value);
                        }

                        var retornoSp = Convert.ToByte(pRetorno.Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertEmpresaVisitaCarga), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertEmpresaVisitaCarga), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int InsertEmpresaVisitaCargaDetalle(EmpresaVisitaCargaDetalleModelo detalle)
        {
            var retorno = 0;
            var sql = "SP_INS_EMPRESA_VISITA_CARGA_DETALLE";

            var consulta = sql + " " +
                detalle.IdCarga + "," +
                detalle.Fila + "," +
                FuncionesDatos.TextoSql(detalle.Rut) + "," +
                FuncionesDatos.TextoSql(detalle.Nombre) + "," +
                FuncionesDatos.TextoSql(detalle.Empresa) + "," +
                detalle.Accion + "," +
                FuncionesDatos.TextoSql(detalle.NombreAnterior) + "," +
                FuncionesDatos.TextoSql(detalle.EmpresaAnterior) + "," +
                detalle.Estado + "," +
                FuncionesDatos.TextoSql(detalle.Mensaje);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EVC_ID", detalle.IdCarga);
                        cmd.Parameters.AddWithValue("P_EVCD_FILA", detalle.Fila);
                        cmd.Parameters.AddWithValue("P_EV_RUT", string.IsNullOrWhiteSpace(detalle.Rut) ? (object)DBNull.Value : detalle.Rut);
                        cmd.Parameters.AddWithValue("P_EV_NOMBRE", string.IsNullOrWhiteSpace(detalle.Nombre) ? (object)DBNull.Value : detalle.Nombre);
                        cmd.Parameters.AddWithValue("P_EV_EMPRESA", string.IsNullOrWhiteSpace(detalle.Empresa) ? (object)DBNull.Value : detalle.Empresa);
                        cmd.Parameters.AddWithValue("P_EVCD_ACCION", detalle.Accion);
                        cmd.Parameters.AddWithValue("P_EV_NOMBRE_ANTERIOR", string.IsNullOrWhiteSpace(detalle.NombreAnterior) ? (object)DBNull.Value : detalle.NombreAnterior);
                        cmd.Parameters.AddWithValue("P_EV_EMPRESA_ANTERIOR", string.IsNullOrWhiteSpace(detalle.EmpresaAnterior) ? (object)DBNull.Value : detalle.EmpresaAnterior);
                        cmd.Parameters.AddWithValue("P_EVCD_ESTADO", detalle.Estado);
                        cmd.Parameters.AddWithValue("P_EVCD_MENSAJE", string.IsNullOrWhiteSpace(detalle.Mensaje) ? (object)DBNull.Value : detalle.Mensaje);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.Int16).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToInt32(cmd.Parameters["P_RETORNO"].Value ?? -1);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertEmpresaVisitaCargaDetalle), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertEmpresaVisitaCargaDetalle), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateEmpresaVisitaCargaResultado(EmpresaVisitaCargaModelo carga)
        {
            var retorno = 0;
            var sql = @"SP_UPD_EMPRESA_VISITA_CARGA_RESULTADO";

            var consulta = sql + " " +
                carga.Id + "," +
                carga.TotalFilas + "," +
                carga.TotalOk + "," +
                carga.TotalError + "," +
                carga.Estado;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EVC_ID", carga.Id);
                        cmd.Parameters.AddWithValue("P_EVC_TOTAL_FILAS", carga.TotalFilas);
                        cmd.Parameters.AddWithValue("P_EVC_TOTAL_OK", carga.TotalOk);
                        cmd.Parameters.AddWithValue("P_EVC_TOTAL_ERROR", carga.TotalError);
                        cmd.Parameters.AddWithValue("P_EVC_ESTADO", carga.Estado);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else if (retornoSp == 1)
                        {
                            retorno = 1;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateEmpresaVisitaCargaResultado), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateEmpresaVisitaCargaResultado), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static List<EmpresaVisitaCargaModelo> GetEmpresaVisitaCarga(EmpresaVisitaCargaFiltroModelo filtro)
        {
            var lista = new List<EmpresaVisitaCargaModelo>();
            var sql = "SP_SEL_EMPRESA_VISITA_CARGA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(filtro.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(filtro.Coincidencia) + "," +
                filtro.Estado + "," +
                FuncionesDatos.TextoSql(filtro.Desde) + "," +
                FuncionesDatos.TextoSql(filtro.Hasta) + "," +
                filtro.Orden + "," +
                filtro.Orientacion;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", filtro.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_BUSCADOR", filtro.Coincidencia ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_EVC_ESTADO", filtro.Estado);
                        cmd.Parameters.AddWithValue("P_EVC_CREATED_AT_DESDE", filtro.Desde ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_EVC_CREATED_AT_HASTA", filtro.Hasta ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_CAMPO", filtro.Orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", filtro.Orientacion);

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(MapEmpresaVisitaCarga(dr));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetEmpresaVisitaCarga), consulta);
            }

            return lista;
        }

        public static EmpresaVisitaCargaModelo GetEmpresaVisitaCargaId(uint idCarga, string rutEmpresa)
        {
            EmpresaVisitaCargaModelo carga = null;
            var sql = "SP_SEL_EMPRESA_VISITA_CARGA_ID";

            var consulta = sql + " " +
                idCarga + "," +
                FuncionesDatos.TextoSql(rutEmpresa);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EVC_ID", idCarga);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", rutEmpresa);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                carga = MapEmpresaVisitaCarga(dr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetEmpresaVisitaCargaId), consulta);
            }

            return carga;
        }

        public static List<EmpresaVisitaCargaDetalleModelo> GetEmpresaVisitaCargaDetalle(EmpresaVisitaCargaDetalleFiltroModelo filtro)
        {
            var lista = new List<EmpresaVisitaCargaDetalleModelo>();
            var sql = "SP_SEL_EMPRESA_VISITA_CARGA_DETALLE";

            var consulta = sql + " " +
                filtro.IdCarga + "," +
                FuncionesDatos.TextoSql(filtro.RutEmpresa) + "," +
                filtro.Estado + "," +
                filtro.Accion + "," +
                FuncionesDatos.TextoSql(filtro.Coincidencia);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EVC_ID", filtro.IdCarga);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", filtro.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_EVCD_ESTADO", filtro.Estado);
                        cmd.Parameters.AddWithValue("P_EVCD_ACCION", filtro.Accion);
                        cmd.Parameters.AddWithValue("P_BUSCADOR", filtro.Coincidencia ?? string.Empty);

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new EmpresaVisitaCargaDetalleModelo
                                {
                                    Id = Convert.ToUInt32(dr["EVCD_ID"]),
                                    IdCarga = Convert.ToUInt32(dr["EVC_ID"]),
                                    Fila = Convert.ToInt32(dr["EVCD_FILA"]),
                                    Rut = Convert.ToString(dr["EV_RUT"]),
                                    Nombre = Convert.ToString(dr["EV_NOMBRE"]),
                                    Empresa = Convert.ToString(dr["EV_EMPRESA"]),
                                    Accion = Convert.ToSByte(dr["EVCD_ACCION"]),
                                    AccionNombre = Convert.ToString(dr["EVCD_ACCION_NOMBRE"]),
                                    NombreAnterior = Convert.ToString(dr["EV_NOMBRE_ANTERIOR"]),
                                    EmpresaAnterior = Convert.ToString(dr["EV_EMPRESA_ANTERIOR"]),
                                    Estado = Convert.ToSByte(dr["EVCD_ESTADO"]),
                                    EstadoNombre = Convert.ToString(dr["EVCD_ESTADO_NOMBRE"]),
                                    Mensaje = Convert.ToString(dr["EVCD_MENSAJE"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetEmpresaVisitaCargaDetalle), consulta);
            }

            return lista;
        }

        private static EmpresaVisitaCargaModelo MapEmpresaVisitaCarga(IDataRecord dr)
        {
            return new EmpresaVisitaCargaModelo
            {
                Id = Convert.ToUInt32(dr["EVC_ID"]),
                RutEmpresa = Convert.ToString(dr["EMP_RUT"]),
                ArchivoNombre = Convert.ToString(dr["EVC_ARCHIVO_NOMBRE"]),
                ArchivoRuta = Convert.ToString(dr["EVC_ARCHIVO_RUTA"]),
                TotalFilas = Convert.ToInt32(dr["EVC_TOTAL_FILAS"]),
                TotalOk = Convert.ToInt32(dr["EVC_TOTAL_OK"]),
                TotalError = Convert.ToInt32(dr["EVC_TOTAL_ERROR"]),
                Estado = Convert.ToSByte(dr["EVC_ESTADO"]),
                EstadoNombre = Convert.ToString(dr["EVC_ESTADO_NOMBRE"]),
                CreatedFecha = Convert.ToString(dr["EVC_CREATED_FECHA"]),
                CreatedHora = Convert.ToString(dr["EVC_CREATED_HORA"]),
                CreatedBy = Convert.ToString(dr["EVC_CREATED_BY"]),
                CreatedByNombre = dr["EVC_CREATED_BY_NOMBRE"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(dr["EVC_CREATED_BY_NOMBRE"])
            };
        }
    }
}
