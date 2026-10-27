using Datos.Comunes;
using Modelo.v1.Dispositivo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Dispositivo
{
    public class DispositivoDatos
    {
        public static List<DispositivoModelo> GetReporteDispositivos(ref int operacion, string emp_rut, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<DispositivoModelo>();
            var sql = "SP_SEL_REPORTE_DISPOSITIVOS";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(emp_rut) + "," +
                FuncionesDatos.TextoSql(buscador) + "," +
                (estado.HasValue ? estado.Value.ToString() : "NULL") + "," +
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
                        cmd.Parameters.AddWithValue("P_EMP_RUT", emp_rut);
                        cmd.Parameters.AddWithValue("P_BUSCADOR", buscador ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_DIS_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new DispositivoModelo()
                                {
                                    Id = Convert.ToUInt32(reader["DIS_ID"]),
                                    ModeloId = reader["DMOD_ID"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(reader["DMOD_ID"]),
                                    MarcaId = reader["DMAR_ID"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(reader["DMAR_ID"]),
                                    Marca = Convert.ToString(reader["DIS_MARCA"]),
                                    Modelo = Convert.ToString(reader["DIS_MODELO"]),
                                    MarcaNombre = Convert.ToString(reader["DIS_MARCA"]),
                                    ModeloNombre = Convert.ToString(reader["DIS_MODELO"]),
                                    PartNumber = Convert.ToString(reader["DIS_PARTNUMBER"]),
                                    SerialNumber = Convert.ToString(reader["DIS_SERIALNUMBER"]),
                                    Comentario = Convert.ToString(reader["DIS_COMENTARIO"]),
                                    RutEmpresa = Convert.ToString(reader["EMP_RUT"]),
                                    Estado = Convert.ToSByte(reader["DIS_ESTADO"]),
                                    EstadoNombre = Convert.ToString(reader["DIS_ESTADO_NOMBRE"]),
                                    ActivationCodeHash = Convert.ToString(reader["DIS_ACTIVATION_CODE_HASH"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteDispositivos), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static DispositivoModelo GetDispositivo(ref int operacion, DispositivoModelo disp)
        {
            var dispositivo = new DispositivoModelo
            {
                Id = 0,
                Marca = string.Empty,
                Modelo = string.Empty,
                MarcaId = null,
                MarcaNombre = string.Empty,
                ModeloId = null,
                ModeloNombre = string.Empty,
                PartNumber = string.Empty,
                SerialNumber = string.Empty,
                Comentario = string.Empty,
                RutEmpresa = string.Empty,
                Estado = 0,
                ActivationCodeHash = string.Empty,
                RutUsuario = string.Empty,
                UpdatedAt = DateTime.MinValue
            };

            var sql = "SP_SEL_DISPOSITIVO";

            var consulta = sql + " " +
                disp.Id + "," +
                (disp.Estado.HasValue ? disp.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(disp.RutEmpresa);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DIS_ID", disp.Id == 0 ? (object)DBNull.Value : disp.Id);
                        cmd.Parameters.AddWithValue("P_DIS_ESTADO", disp.Estado.HasValue ? (object)disp.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", disp.RutEmpresa);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                dispositivo.Id = Convert.ToUInt32(rs["DIS_ID"]);
                                dispositivo.Marca = rs["DIS_MARCA"] == DBNull.Value ? string.Empty : rs["DIS_MARCA"].ToString();
                                dispositivo.Modelo = rs["DIS_MODELO"] == DBNull.Value ? string.Empty : rs["DIS_MODELO"].ToString();
                                dispositivo.MarcaId = rs["DMAR_ID"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(rs["DMAR_ID"]);
                                dispositivo.MarcaNombre = rs["DMAR_NOMBRE"] == DBNull.Value ? dispositivo.Marca : rs["DMAR_NOMBRE"].ToString();
                                dispositivo.ModeloId = rs["DMOD_ID"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(rs["DMOD_ID"]);
                                dispositivo.ModeloNombre = rs["DMOD_NOMBRE"] == DBNull.Value ? dispositivo.Modelo : rs["DMOD_NOMBRE"].ToString();
                                dispositivo.PartNumber = rs["DIS_PARTNUMBER"] == DBNull.Value ? string.Empty : rs["DIS_PARTNUMBER"].ToString();
                                dispositivo.SerialNumber = rs["DIS_SERIALNUMBER"] == DBNull.Value ? string.Empty : rs["DIS_SERIALNUMBER"].ToString();
                                dispositivo.Comentario = rs["DIS_COMENTARIO"] == DBNull.Value ? string.Empty : rs["DIS_COMENTARIO"].ToString();
                                dispositivo.RutEmpresa = rs["EMP_RUT"] == DBNull.Value ? string.Empty : rs["EMP_RUT"].ToString();
                                dispositivo.Estado = rs["DIS_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(rs["DIS_ESTADO"]);
                                dispositivo.ActivationCodeHash = rs["DIS_ACTIVATION_CODE_HASH"] == DBNull.Value ? string.Empty : rs["DIS_ACTIVATION_CODE_HASH"].ToString();
                                dispositivo.UpdatedAt = Convert.ToDateTime(rs["DIS_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetDispositivo), consulta);

                operacion = -1;
            }

            return dispositivo;
        }

        public static bool GetDispositivoPorSerie(ref int operacion, DispositivoModelo dispositivo)
        {
            var sql = "SP_SEL_DISPOSITIVO_SERIALNUMBER_EXISTE";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(dispositivo.PartNumber) + "," +
                FuncionesDatos.TextoSql(dispositivo.SerialNumber);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DIS_PARTNUMBER", dispositivo.PartNumber);
                        cmd.Parameters.AddWithValue("P_DIS_SERIALNUMBER", dispositivo.SerialNumber);

                        var resultado = cmd.ExecuteScalar();

                        operacion = 0;

                        return resultado != null && Convert.ToInt32(resultado) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetDispositivoPorSerie), consulta);

                operacion = -1;
                return false;
            }
        }

        public static int UpdateDispositivo(DispositivoModelo dispositivo)
        {
            var retorno = 0;
            var sql = @"SP_UPD_DISPOSITIVO";

            var consulta = sql + " " +
                dispositivo.Id + "," +
                (dispositivo.ModeloId.HasValue ? dispositivo.ModeloId.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(dispositivo.Comentario) + "," +
                (dispositivo.Estado.HasValue ? dispositivo.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(dispositivo.ActivationCodeHash) + "," +
                FuncionesDatos.TextoSql(dispositivo.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(dispositivo.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DIS_ID", dispositivo.Id);
                        cmd.Parameters.AddWithValue("P_DMOD_ID", dispositivo.ModeloId.HasValue ? (object)dispositivo.ModeloId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_DIS_COMENTARIO", dispositivo.Comentario);
                        cmd.Parameters.AddWithValue("P_DIS_ESTADO", dispositivo.Estado.HasValue ? (object)dispositivo.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_DIS_ACTIVATION_CODE_HASH", dispositivo.ActivationCodeHash);
                        cmd.Parameters.AddWithValue("P_DIS_UPDATED_AT_ORIGINAL", dispositivo.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", dispositivo.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else if (retornoSp == 3)
                        {
                            retorno = 3;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateDispositivo), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateDispositivo), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int InsertDispositivo(DispositivoModelo dispositivo)
        {
            var retorno = 0;
            var sql = "SP_INS_DISPOSITIVO";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(dispositivo.RutEmpresa) + "," +
                (dispositivo.ModeloId.HasValue ? dispositivo.ModeloId.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(dispositivo.PartNumber) + "," +
                FuncionesDatos.TextoSql(dispositivo.SerialNumber) + "," +
                FuncionesDatos.TextoSql(dispositivo.Comentario) + "," +
                (dispositivo.Estado.HasValue ? dispositivo.Estado.Value.ToString() : "NULL") + "," +
                "'[HASH_OMITIDO]'," +
                FuncionesDatos.TextoSql(dispositivo.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", dispositivo.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_DMOD_ID", dispositivo.ModeloId.HasValue ? (object)dispositivo.ModeloId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_DIS_PARTNUMBER", dispositivo.PartNumber);
                        cmd.Parameters.AddWithValue("P_DIS_SERIALNUMBER", dispositivo.SerialNumber);
                        cmd.Parameters.AddWithValue("P_DIS_COMENTARIO", dispositivo.Comentario);
                        cmd.Parameters.AddWithValue("P_DIS_ESTADO", dispositivo.Estado.HasValue ? (object)dispositivo.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_DIS_ACTIVATION_CODE_HASH", dispositivo.ActivationCodeHash);
                        cmd.Parameters.AddWithValue("P_USU_RUT", dispositivo.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertDispositivo), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertDispositivo), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateDispositivoActivationCodeHash(DispositivoModelo dispositivo)
        {
            var retorno = 0;
            var sql = "SP_UPD_DISPOSITIVO_ACTIVATION_CODE_HASH";

            var consulta = sql + " " +
                dispositivo.Id + "," +
                FuncionesDatos.TextoSql(dispositivo.ActivationCodeHash);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DIS_ID", dispositivo.Id);
                        cmd.Parameters.AddWithValue("P_DIS_ACTIVATION_CODE_HASH", dispositivo.ActivationCodeHash);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateDispositivoActivationCodeHash), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateDispositivoActivationCodeHash), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}