using Datos.Comunes;
using Modelo.v1.Log;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Log
{
    public class LogAccesoDispositivoDatos
    {
        public static List<LogAccesoDispositivoModulo> GetReporteLogAccesoDispositivo(ref int operacion, string rutEmpresa, string buscador, string desde, string hasta, short orden, short orientacion)
        {
            var lista = new List<LogAccesoDispositivoModulo>();
            var sql = "SP_SEL_REPORTE_LOG_ACCESO_DISPOSITIVO";

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
                        cmd.Parameters.AddWithValue("P_ADL_FECHA_DESDE", desde ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_ADL_FECHA_HASTA", hasta ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new LogAccesoDispositivoModulo()
                                {
                                    Id = Convert.ToUInt32(reader["ADL_ID"]),
                                    Fecha = Convert.ToString(reader["ADL_FECHA"]),
                                    Hora = Convert.ToString(reader["ADL_HORA"]),
                                    Ip = Convert.ToString(reader["ADL_IP"]),
                                    DispositivoId = reader["DIS_ID"] == DBNull.Value ? 0 : Convert.ToUInt32(reader["DIS_ID"]),
                                    EndPoint = Convert.ToString(reader["ADL_ENDPOINT"]),
                                    StatusCode = Convert.ToInt16(reader["ADL_STATUS_CODE"]),
                                    CorrelationId = Convert.ToString(reader["ADL_CORRELATION_ID"]),
                                    Duracion = Convert.ToUInt32(reader["ADL_DURACION_MS"]),
                                    Metodo = Convert.ToString(reader["ADL_METODO_HTTP"]),
                                    Agente = Convert.ToString(reader["ADL_USER_AGENT"]),
                                    Resultado = Convert.ToString(reader["ADL_RESULTADO"]),
                                    Token = Convert.ToString(reader["ADL_DEVICE_SESSION_TOKEN"]),
                                    RutEmpresa = Convert.ToString(reader["EMP_RUT"]),
                                    SerialNumber = Convert.ToString(reader["DIS_SERIALNUMBER"]),
                                    Marca = Convert.ToString(reader["DIS_MARCA"]),
                                    Modelo = Convert.ToString(reader["DIS_MODELO"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteLogAccesoDispositivo), consulta);

                operacion = -1;
            }

            return lista;
        }
    }
}
