using Datos.Comunes;
using Modelo.v1.DispositivoModelos;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.DispositivoModelos
{
    public class DispositivoModelosDatos
    {
        public static List<DispositivoModelosModelo> GetReporteDispositivoModelos(ref int operacion, string buscador, uint? idMarca, short? estado, short orden, short orientacion)
        {
            var lista = new List<DispositivoModelosModelo>();
            var sql = @"SP_SEL_REPORTE_DISPOSITIVO_MODELOS";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(buscador) + "," +
                (idMarca.HasValue ? idMarca.Value.ToString() : "NULL") + "," +
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
                        cmd.Parameters.AddWithValue("P_BUSCADOR", buscador ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_DMAR_ID", idMarca.HasValue ? (object)idMarca.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_DMOD_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new DispositivoModelosModelo
                                {
                                    Id = Convert.ToUInt32(reader["DMOD_ID"]),
                                    MarcaId = Convert.ToUInt32(reader["DMAR_ID"]),
                                    MarcaNombre = Convert.ToString(reader["DMAR_NOMBRE"]),
                                    Nombre = Convert.ToString(reader["DMOD_NOMBRE"]),
                                    Estado = Convert.ToSByte(reader["DMOD_ESTADO"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteDispositivoModelos), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static int InsertDispositivoModelo(DispositivoModelosModelo modelo)
        {
            var retorno = 0;
            var sql = @"SP_INS_DISPOSITIVO_MODELO";

            var consulta = sql + " " +
                modelo.MarcaId + "," +
                FuncionesDatos.TextoSql(modelo.Nombre) + "," +
                modelo.Estado + "," +
                FuncionesDatos.TextoSql(modelo.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DMAR_ID", modelo.MarcaId);
                        cmd.Parameters.AddWithValue("P_DMOD_NOMBRE", modelo.Nombre);
                        cmd.Parameters.AddWithValue("P_DMOD_ESTADO", modelo.Estado.HasValue ? (object)modelo.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", modelo.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertDispositivoModelo), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertDispositivoModelo), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static DispositivoModelosModelo GetDispositivoModelo(ref int operacion, DispositivoModelosModelo mod)
        {
            var modelo = new DispositivoModelosModelo
            {
                Id = 0,
                MarcaId = 0,
                MarcaNombre = string.Empty,
                Nombre = string.Empty,
                Estado = 0,
                UpdatedAt = DateTime.MinValue
            };

            var sql = @"SP_SEL_DISPOSITIVO_MODELO";

            var consulta = sql + " " +
                mod.Id + "," +
                (mod.Estado.HasValue ? mod.Estado.Value.ToString() : "NULL");

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DMOD_ID", mod.Id);
                        cmd.Parameters.AddWithValue("P_DMOD_ESTADO", mod.Estado.HasValue ? (object)mod.Estado.Value : DBNull.Value);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                modelo.Id = Convert.ToUInt32(rs["DMOD_ID"]);
                                modelo.MarcaId = Convert.ToUInt32(rs["DMAR_ID"]);
                                modelo.MarcaNombre = Convert.ToString(rs["DMAR_NOMBRE"]);
                                modelo.Nombre = Convert.ToString(rs["DMOD_NOMBRE"]);
                                modelo.Estado = Convert.ToSByte(rs["DMOD_ESTADO"]);
                                modelo.UpdatedAt = Convert.ToDateTime(rs["DMOD_UPDATED_AT"]);

                                operacion = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetDispositivoModelo), consulta);

                operacion = -1;
            }

            return modelo;
        }

        public static int UpdateDispositivoModelo(DispositivoModelosModelo dispositivoModelo)
        {
            var retorno = 0;
            var sql = @"SP_UPD_DISPOSITIVO_MODELO";

            var consulta = sql + " " +
                dispositivoModelo.Id + "," +
                dispositivoModelo.MarcaId + "," +
                FuncionesDatos.TextoSql(dispositivoModelo.Nombre) + "," +
                dispositivoModelo.Estado + "," +
                FuncionesDatos.TextoSql(dispositivoModelo.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(dispositivoModelo.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DMOD_ID", dispositivoModelo.Id);
                        cmd.Parameters.AddWithValue("P_DMAR_ID", dispositivoModelo.MarcaId);
                        cmd.Parameters.AddWithValue("P_DMOD_NOMBRE", dispositivoModelo.Nombre);
                        cmd.Parameters.AddWithValue("P_DMOD_ESTADO", dispositivoModelo.Estado);
                        cmd.Parameters.AddWithValue("P_DMOD_UPDATED_AT_ORIGINAL", dispositivoModelo.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", dispositivoModelo.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateDispositivoModelo), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateDispositivoModelo), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}