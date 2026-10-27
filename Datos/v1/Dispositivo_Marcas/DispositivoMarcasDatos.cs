using Datos.Comunes;
using Modelo.v1.DispositivoMarcas;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.DispositivoMarcas
{
    public class DispositivoMarcasDatos
    {
        public static List<DispositivoMarcasModelo> GetReporteDispositivoMarcas(ref int operacion, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<DispositivoMarcasModelo>();
            var sql = @"SP_SEL_REPORTE_DISPOSITIVO_MARCAS";

            var consulta = sql + " " +
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
                        cmd.Parameters.AddWithValue("P_BUSCADOR", buscador ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_DMAR_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new DispositivoMarcasModelo()
                                {
                                    Id = Convert.ToUInt32(reader["DMAR_ID"]),
                                    Nombre = Convert.ToString(reader["DMAR_NOMBRE"]),
                                    Estado = Convert.ToSByte(reader["DMAR_ESTADO"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteDispositivoMarcas), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static int InsertDispositivoMarca(DispositivoMarcasModelo marca)
        {
            var retorno = 0;
            var sql = @"SP_INS_DISPOSITIVO_MARCA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(marca.Nombre) + "," +
                marca.Estado + "," +
                FuncionesDatos.TextoSql(marca.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DMAR_NOMBRE", marca.Nombre);
                        cmd.Parameters.AddWithValue("P_DMAR_ESTADO", marca.Estado.HasValue ? (object)marca.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", marca.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertDispositivoMarca), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertDispositivoMarca), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static DispositivoMarcasModelo GetDispositivoMarca(ref int operacion, DispositivoMarcasModelo mar)
        {
            var marca = new DispositivoMarcasModelo
            {
                Id = 0,
                Nombre = string.Empty,
                Estado = 0,
                UpdatedAt = DateTime.MinValue
            };

            var sql = @"SP_SEL_DISPOSITIVO_MARCA";

            var consulta = sql + " " +
                mar.Id + "," +
                (mar.Estado.HasValue ? mar.Estado.Value.ToString() : "NULL");

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DMAR_ID", mar.Id);
                        cmd.Parameters.AddWithValue("P_DMAR_ESTADO", mar.Estado.HasValue ? (object)mar.Estado.Value : DBNull.Value);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                marca.Id = Convert.ToUInt32(rs["DMAR_ID"]);
                                marca.Nombre = Convert.ToString(rs["DMAR_NOMBRE"]);
                                marca.Estado = Convert.ToSByte(rs["DMAR_ESTADO"]);
                                marca.UpdatedAt = Convert.ToDateTime(rs["DMAR_UPDATED_AT"]);

                                operacion = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetDispositivoMarca), consulta);

                operacion = -1;
            }

            return marca;
        }

        public static int UpdateDispositivoMarca(DispositivoMarcasModelo dispositivoMarca)
        {
            var retorno = 0;
            var sql = @"SP_UPD_DISPOSITIVO_MARCA";

            var consulta = sql + " " +
                dispositivoMarca.Id + "," +
                FuncionesDatos.TextoSql(dispositivoMarca.Nombre) + "," +
                dispositivoMarca.Estado + "," +
                FuncionesDatos.TextoSql(dispositivoMarca.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(dispositivoMarca.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_DMAR_ID", dispositivoMarca.Id);
                        cmd.Parameters.AddWithValue("P_DMAR_NOMBRE", dispositivoMarca.Nombre);
                        cmd.Parameters.AddWithValue("P_DMAR_ESTADO", dispositivoMarca.Estado);
                        cmd.Parameters.AddWithValue("P_DMAR_UPDATED_AT_ORIGINAL", dispositivoMarca.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", dispositivoMarca.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateDispositivoMarca), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateDispositivoMarca), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}