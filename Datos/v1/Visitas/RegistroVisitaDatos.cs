using Datos.Comunes;
using Modelo.v1.Visitas;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Visitas
{
    public class RegistroVisitaDatos
    {
        public static List<ReporteRegistroVisitaModelo> GetReporteRegistroVisitas(ref int operacion, RegistroVisitaFiltroModelo filtro)
        {
            var lista = new List<ReporteRegistroVisitaModelo>();
            var sql = @"SP_SEL_REPORTE_REGISTRO_VISITA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(filtro.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(filtro.Coincidencia) + "," +
                (filtro.Operacion == 9 ? "NULL" : filtro.Operacion.ToString()) + "," +
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
                        cmd.Parameters.AddWithValue("P_OPERACION", filtro.Operacion == 9 ? (object)DBNull.Value : filtro.Operacion);
                        cmd.Parameters.AddWithValue("P_DESDE", filtro.Desde ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_HASTA", filtro.Hasta ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_CAMPO", filtro.Orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", filtro.Orientacion);

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var fila = new ReporteRegistroVisitaModelo
                                {
                                    Rut = Convert.ToString(dr["RV_RUT"]),
                                    NombreApellido = Convert.ToString(dr["RV_NOMBRE_APELLIDO"]).ToUpper(),
                                    Empresa = Convert.ToString(dr["RV_EMPRESA"]).ToUpper(),
                                    Patente = Convert.ToString(dr["RV_PATENTE"]).ToUpper(),
                                    Comentario = Convert.ToString(dr["RV_COMENTARIO"]),
                                    EntradaSalida = Convert.ToSByte(dr["RV_ENTRADA_SALIDA"]),
                                    Fecha = Convert.ToString(dr["RV_FECHA"]),
                                    Hora = Convert.ToString(dr["RV_HORA"]),
                                    Fotografia = BlobToBase64OrNull(dr, "RV_FOTOGRAFIA"),
                                    IdDispositivo = Convert.ToUInt32(dr["DIS_ID"]),
                                    SerialNumber = Convert.ToString(dr["DIS_SERIALNUMBER"]),
                                    Marca = Convert.ToString(dr["DIS_MARCA"]),
                                    Modelo = Convert.ToString(dr["DIS_MODELO"])
                                };

                                lista.Add(fila);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteRegistroVisitas), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static RegistroVisitaModelo GetRegistroVisita(ref int operacion, RegistroVisitaModelo rv)
        {
            var registroVisita = new RegistroVisitaModelo
            {
                Id = 0,
                Rut = string.Empty,
                Fecha = string.Empty,
                Hora = string.Empty,
                Ip = string.Empty,
                NombreApellido = string.Empty,
                Empresa = string.Empty,
                Patente = string.Empty,
                Comentario = string.Empty,
                EntradaSalida = 0,
                RutEmpresa = string.Empty,
                RutUsuario = string.Empty,
                IdDispositivo = 0
            };

            var sql = @"SP_SEL_REGISTRO_VISITA";

            var consulta = sql + " " +
                rv.Id + "," +
                FuncionesDatos.TextoSql(rv.RutEmpresa);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_RV_ID", rv.Id);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", rv.RutEmpresa);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                registroVisita.Id = Convert.ToUInt32(rs["RV_ID"]);
                                registroVisita.Rut = Convert.ToString(rs["RV_RUT"]);
                                registroVisita.Fecha = Convert.ToString(rs["RV_FECHA"]);
                                registroVisita.Hora = Convert.ToString(rs["RV_HORA"]);
                                registroVisita.Ip = Convert.ToString(rs["RV_IP"]);
                                registroVisita.NombreApellido = Convert.ToString(rs["RV_NOMBRE_APELLIDO"]);
                                registroVisita.Empresa = Convert.ToString(rs["RV_EMPRESA"]);
                                registroVisita.Patente = Convert.ToString(rs["RV_PATENTE"]);
                                registroVisita.Comentario = Convert.ToString(rs["RV_COMENTARIO"]);
                                registroVisita.Fotografia = BlobToBase64OrNull(rs, "RV_FOTOGRAFIA");
                                registroVisita.EntradaSalida = Convert.ToSByte(rs["RV_ENTRADA_SALIDA"]);
                                registroVisita.RutEmpresa = Convert.ToString(rs["EMP_RUT"]);
                                registroVisita.RutUsuario = Convert.ToString(rs["USU_RUT"]);
                                registroVisita.IdDispositivo = Convert.ToUInt32(rs["DIS_ID"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetRegistroVisita), consulta);

                operacion = -1;
            }

            return registroVisita;
        }

        private static string BlobToBase64OrNull(MySqlDataReader reader, string columnName)
        {
            var value = reader[columnName];

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            var bytes = value as byte[];

            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            return Convert.ToBase64String(bytes);
        }
    }
}
