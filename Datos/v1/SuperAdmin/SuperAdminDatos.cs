using Datos.Comunes;
using Modelo.v1.SuperAdmin;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.SuperAdmin
{
    public class SuperAdminDatos
    {
        public static SuperAdminModelo GetSuperAdmin(ref int operacion, SuperAdminModelo superAdmin)
        {
            var resultado = new SuperAdminModelo
            {
                RutAdmin = string.Empty,
                NombreAdmin = string.Empty,
                ApellidoAdmin = string.Empty,
                Email = string.Empty,
                Telefono = string.Empty,
                Password = string.Empty,
                PasswordConfirmar = string.Empty,
                PasswordHash = string.Empty,
                PasswordSalt = string.Empty,
                PasswordAlgoritmo = string.Empty,
                Estado = null,
                EstadoNombre = string.Empty,
                UpdatedAt = DateTime.MinValue,
                MFARequerido = 1
            };

            var sql = @"SP_SEL_SUPER_ADMIN_ESPECIFICO";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(superAdmin.RutAdmin);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_SA_RUT", superAdmin.RutAdmin);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                resultado.RutAdmin = Convert.ToString(reader["SA_RUT"]);
                                resultado.NombreAdmin = Convert.ToString(reader["USU_NOMBRE"]);
                                resultado.ApellidoAdmin = Convert.ToString(reader["USU_APELLIDO"]);
                                resultado.PasswordHash = Convert.ToString(reader["SA_PASSWORD_HASH"]);
                                resultado.PasswordSalt = Convert.ToString(reader["SA_PASSWORD_SALT"]);
                                resultado.PasswordAlgoritmo = Convert.ToString(reader["SA_PASSWORD_ALGORITMO"]);
                                resultado.Email = Convert.ToString(reader["SA_EMAIL"]);
                                resultado.Telefono = Convert.ToString(reader["SA_TELEFONO"]);
                                resultado.Estado = Convert.ToSByte(reader["SA_ESTADO"]);
                                resultado.EstadoNombre = resultado.Estado == 0 ? "Vigente" : "Anulado";
                                resultado.UpdatedAt = Convert.ToDateTime(reader["SA_UPDATED_AT"]);
                                resultado.MFARequerido = Convert.ToSByte(reader["SA_MFA_REQUERIDO"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetSuperAdmin), consulta);

                operacion = -1;
            }

            return resultado;
        }

        public static List<SuperAdminModelo> GetReporteSuperAdmin(ref int operacion, string buscador, sbyte? estado, int campo, int orden)
        {
            var lista = new List<SuperAdminModelo>();
            var sql = @"SP_SEL_REPORTE_SUPER_ADMIN";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(buscador) + "," +
                (estado.HasValue ? estado.Value.ToString() : "NULL") + "," +
                campo + "," +
                orden;

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
                        cmd.Parameters.AddWithValue("P_SA_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", campo);
                        cmd.Parameters.AddWithValue("P_ORDEN", orden);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new SuperAdminModelo
                                {
                                    RutAdmin = Convert.ToString(reader["SA_RUT"]),
                                    NombreAdmin = Convert.ToString(reader["USU_NOMBRE"]),
                                    ApellidoAdmin = Convert.ToString(reader["USU_APELLIDO"]),
                                    Email = Convert.ToString(reader["SA_EMAIL"]),
                                    Telefono = Convert.ToString(reader["SA_TELEFONO"]),
                                    Estado = Convert.ToSByte(reader["SA_ESTADO"]),
                                    EstadoNombre = Convert.ToString(reader["SA_ESTADO_NOMBRE"]),
                                    MFARequerido = Convert.ToSByte(reader["SA_MFA_REQUERIDO"]),
                                    MFATotpId = reader["MT_ID"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(reader["MT_ID"]),
                                    MFAConfigurado = reader["MT_CONFIGURADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MT_CONFIGURADO"]),
                                    MFAEstado = reader["MT_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MT_ESTADO"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteSuperAdmin), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static int InsertSuperAdmin(SuperAdminModelo superAdmin)
        {
            var retorno = 0;
            var sql = @"SP_INS_SUPER_ADMIN";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(superAdmin.RutAdmin) + "," +
                FuncionesDatos.TextoSql(superAdmin.NombreAdmin) + "," +
                FuncionesDatos.TextoSql(superAdmin.ApellidoAdmin) + "," +
                "'[HASH_OMITIDO]'," +
                "'[SALT_OMITIDO]'," +
                "'[ALGORITMO_OMITIDO]'," +
                FuncionesDatos.TextoSql(superAdmin.Email) + "," +
                FuncionesDatos.TextoSql(superAdmin.Telefono) + "," +
                (superAdmin.Estado.HasValue ? superAdmin.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(superAdmin.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_SA_RUT", superAdmin.RutAdmin);
                        cmd.Parameters.AddWithValue("P_USU_NOMBRE", superAdmin.NombreAdmin);
                        cmd.Parameters.AddWithValue("P_USU_APELLIDO", superAdmin.ApellidoAdmin);
                        cmd.Parameters.AddWithValue("P_SA_PASSWORD_HASH", superAdmin.PasswordHash);
                        cmd.Parameters.AddWithValue("P_SA_PASSWORD_SALT", superAdmin.PasswordSalt);
                        cmd.Parameters.AddWithValue("P_SA_PASSWORD_ALGORITMO", superAdmin.PasswordAlgoritmo);
                        cmd.Parameters.AddWithValue("P_SA_EMAIL", superAdmin.Email);
                        cmd.Parameters.AddWithValue("P_SA_TELEFONO", superAdmin.Telefono);
                        cmd.Parameters.AddWithValue("P_SA_ESTADO", superAdmin.Estado.HasValue ? (object)superAdmin.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", superAdmin.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.Int16).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToInt32(cmd.Parameters["P_RETORNO"].Value ?? -1);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertSuperAdmin), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertSuperAdmin), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateSuperAdmin(SuperAdminModelo superAdmin)
        {
            var retorno = 0;
            var sql = @"SP_UPD_SUPER_ADMIN";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(superAdmin.RutAdmin) + "," +
                FuncionesDatos.TextoSql(superAdmin.NombreAdmin) + "," +
                FuncionesDatos.TextoSql(superAdmin.ApellidoAdmin) + "," +
                "'[HASH_OMITIDO]'," +
                "'[SALT_OMITIDO]'," +
                "'[ALGORITMO_OMITIDO]'," +
                FuncionesDatos.TextoSql(superAdmin.Email) + "," +
                FuncionesDatos.TextoSql(superAdmin.Telefono) + "," +
                (superAdmin.Estado.HasValue ? superAdmin.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(superAdmin.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(superAdmin.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_SA_RUT", superAdmin.RutAdmin);
                        cmd.Parameters.AddWithValue("P_USU_NOMBRE", superAdmin.NombreAdmin);
                        cmd.Parameters.AddWithValue("P_USU_APELLIDO", superAdmin.ApellidoAdmin);
                        cmd.Parameters.AddWithValue("P_SA_PASSWORD_HASH", string.IsNullOrWhiteSpace(superAdmin.PasswordHash) ? (object)DBNull.Value : superAdmin.PasswordHash);
                        cmd.Parameters.AddWithValue("P_SA_PASSWORD_SALT", string.IsNullOrWhiteSpace(superAdmin.PasswordSalt) ? (object)DBNull.Value : superAdmin.PasswordSalt);
                        cmd.Parameters.AddWithValue("P_SA_PASSWORD_ALGORITMO", string.IsNullOrWhiteSpace(superAdmin.PasswordAlgoritmo) ? (object)DBNull.Value : superAdmin.PasswordAlgoritmo);
                        cmd.Parameters.AddWithValue("P_SA_EMAIL", superAdmin.Email);
                        cmd.Parameters.AddWithValue("P_SA_TELEFONO", superAdmin.Telefono);
                        cmd.Parameters.AddWithValue("P_SA_ESTADO", superAdmin.Estado.HasValue ? (object)superAdmin.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_SA_UPDATED_AT_ORIGINAL", superAdmin.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", superAdmin.RutUsuario);
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
                        else if (retornoSp == 3)
                        {
                            retorno = 3;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateSuperAdmin), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateSuperAdmin), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}