using Datos.Comunes;
using Modelo.v1.Perfil;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Perfil
{
    public class PerfilDatos
    {
        public static List<PerfilModelo> GetReportePerfiles(ref int operacion, string rutEmpresa, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<PerfilModelo>();
            var sql = @"SP_SEL_REPORTE_PERFILES";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(rutEmpresa) + "," +
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
                        cmd.Parameters.AddWithValue("P_EMP_RUT", rutEmpresa);
                        cmd.Parameters.AddWithValue("P_BUSCADOR", buscador);
                        cmd.Parameters.AddWithValue("P_PER_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new PerfilModelo()
                                {
                                    Id = Convert.ToUInt32(reader["PER_ID"]),
                                    Nombre = Convert.ToString(reader["PER_NOMBRE"]),
                                    RutEmpresa = Convert.ToString(reader["EMP_RUT"]),
                                    IdRegla = Convert.ToUInt32(reader["REG_ID"]),
                                    NomRegla = Convert.ToString(reader["REG_NOMBRE"]),
                                    Estado = Convert.ToSByte(reader["PER_ESTADO"]),
                                    EstadoNombre = Convert.ToString(reader["PER_ESTADO_NOMBRE"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReportePerfiles), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static PerfilModelo GetPerfil(ref int operacion, PerfilModelo per)
        {
            var perfil = new PerfilModelo
            {
                Id = 0,
                Nombre = string.Empty,
                RutEmpresa = string.Empty,
                IdRegla = 0,
                Estado = 0,
                UpdatedAt = DateTime.MinValue
            };

            var sql = @"SP_SEL_PERFIL";

            var consulta = sql + " " +
                per.Id + "," +
                (per.Estado.HasValue ? per.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(per.RutEmpresa);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_PER_ID", per.Id);
                        cmd.Parameters.AddWithValue("P_PER_ESTADO", per.Estado.HasValue ? (object)per.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", per.RutEmpresa);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                perfil.Id = Convert.ToUInt32(rs["PER_ID"]);
                                perfil.RutEmpresa = Convert.ToString(rs["EMP_RUT"]);
                                perfil.Nombre = Convert.ToString(rs["PER_NOMBRE"]);
                                perfil.IdRegla = Convert.ToUInt32(rs["REG_ID"]);
                                perfil.Estado = Convert.ToSByte(rs["PER_ESTADO"]);
                                perfil.UpdatedAt = Convert.ToDateTime(rs["PER_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetPerfil), consulta);

                operacion = -1;
            }

            return perfil;
        }

        public static int UpdatePerfil(PerfilModelo perfil)
        {
            var retorno = 0;
            var sql = @"SP_UPD_PERFIL";

            var consulta = sql + " " +
                perfil.Id + "," +
                FuncionesDatos.TextoSql(perfil.Nombre) + "," +
                perfil.IdRegla + "," +
                (perfil.Estado.HasValue ? perfil.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(perfil.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(perfil.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_PER_ID", perfil.Id);
                        cmd.Parameters.AddWithValue("P_PER_NOMBRE", perfil.Nombre);
                        cmd.Parameters.AddWithValue("P_REG_ID", perfil.IdRegla == 0 ? (object)DBNull.Value : perfil.IdRegla);
                        cmd.Parameters.AddWithValue("P_PER_ESTADO", perfil.Estado.HasValue ? (object)perfil.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_PER_UPDATED_AT_ORIGINAL", perfil.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", perfil.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else if (retornoSp == 2)
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdatePerfil), consulta);

                            retorno = -3;
                        }
                        else if (retornoSp == 3)
                        {
                            retorno = 3;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdatePerfil), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdatePerfil), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int InsertPerfil(PerfilModelo perfil)
        {
            var retorno = 0;
            var sql = @"SP_INS_PERFIL";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(perfil.Nombre) + "," +
                FuncionesDatos.TextoSql(perfil.RutEmpresa) + "," +
                perfil.IdRegla + "," +
                (perfil.Estado.HasValue ? perfil.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(perfil.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_PER_NOMBRE", perfil.Nombre);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", perfil.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_REG_ID", perfil.IdRegla == 0 ? (object)DBNull.Value : perfil.IdRegla);
                        cmd.Parameters.AddWithValue("P_PER_ESTADO", perfil.Estado.HasValue ? (object)perfil.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", perfil.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertPerfil), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertPerfil), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}