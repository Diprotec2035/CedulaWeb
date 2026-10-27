using Datos.Comunes;
using Modelo.v1.Log;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Log
{
    public class LogAccesoWebDatos
    {
        public static List<LogAccesoWebModelo> GetReporteLogAccesoWeb(ref int operacion, string rutEmpresa, string buscador, string desde, string hasta, short orden, short orientacion)
        {
            var lista = new List<LogAccesoWebModelo>();
            var sql = "SP_SEL_REPORTE_LOG_ACCESO_WEB";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(rutEmpresa) + "," +
                FuncionesDatos.TextoSql(buscador) + "," +
                FuncionesDatos.TextoSql(desde) + "," +
                FuncionesDatos.TextoSql(hasta) + "," +
                orden + "," +
                orientacion;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", rutEmpresa);
                        cmd.Parameters.AddWithValue("P_BUSCADOR", buscador ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_AWL_FECHA_DESDE", desde ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_AWL_FECHA_HASTA", hasta ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new LogAccesoWebModelo()
                                {
                                    Id = Convert.ToUInt32(reader["AWL_ID"]),
                                    Fecha = Convert.ToString(reader["AWL_FECHA"]),
                                    Hora = Convert.ToString(reader["AWL_HORA"]),
                                    Ip = Convert.ToString(reader["AWL_IP"]),
                                    RutUsuario = Convert.ToString(reader["USU_RUT"]),
                                    NombreUsuario = Convert.ToString(reader["USU_NOMBRE"]),
                                    RutEmpresa = Convert.ToString(reader["EMP_RUT"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteLogAccesoWeb), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static int InsertLogAccesoWeb(LogAccesoWebModelo law)
        {
            var retorno = 0;
            var sql = "SP_INS_LOG_ACCESO_WEB";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(law.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(law.RutUsuario) + "," +
                FuncionesDatos.TextoSql(law.Ip);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", law.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_USU_RUT", law.RutUsuario);
                        cmd.Parameters.AddWithValue("P_AWL_IP", law.Ip);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertLogAccesoWeb), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertLogAccesoWeb), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}
