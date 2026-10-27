using Datos.Comunes;
using Modelo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos
{
    public class EmpresaUsuarioDatos
    {
        public static List<EmpresaUsuarioModelo> GetReporteUsuarios(ref int operacion, string rutEmpresa, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<EmpresaUsuarioModelo>();
            var sql = @"SP_SEL_REPORTE_EMPRESA_USUARIO";

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
                        cmd.Parameters.AddWithValue("P_BUSCADOR", buscador ?? string.Empty);
                        cmd.Parameters.AddWithValue("P_EU_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new EmpresaUsuarioModelo
                                {
                                    Id = Convert.ToUInt32(reader["EU_ID"]),
                                    RutUsuario = Convert.ToString(reader["USU_RUT"]),
                                    NombreUsuario = Convert.ToString(reader["USU_NOMBRE"]).ToUpper(),
                                    RutEmpresa = Convert.ToString(reader["EMP_RUT"]),
                                    IdPerfil = Convert.ToUInt32(reader["PER_ID"]),
                                    NombrePerfil = Convert.ToString(reader["PER_NOMBRE"]),
                                    Telefono = Convert.ToString(reader["EU_TELEFONO"]),
                                    Email = Convert.ToString(reader["EU_EMAIL"]).ToLower(),
                                    Estado = Convert.ToSByte(reader["EU_ESTADO"]),
                                    EstadoNombre = Convert.ToString(reader["EU_ESTADO_NOMBRE"]),
                                    MFARequerido = Convert.ToSByte(reader["EU_MFA_REQUERIDO"]),
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteUsuarios), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static EmpresaUsuarioModelo GetEmpresaUsuario(ref int operacion, EmpresaUsuarioModelo user)
        {
            var empresaUsuario = new EmpresaUsuarioModelo
            {
                Id = 0,
                RutUsuario = string.Empty,
                NombreUsuario = string.Empty,
                ApellidoUsuario = string.Empty,
                Password = string.Empty,
                Email = string.Empty,
                Telefono = string.Empty,
                NombreEmpresa = string.Empty,
                RutEmpresa = string.Empty,
                EstadoEmpresa = 0,
                IdPerfil = 0,
                NombrePerfil = string.Empty,
                PasswordHash = string.Empty,
                PasswordSalt = string.Empty,
                PasswordAlgoritmo = string.Empty,
                RutUsuarioBitacora = string.Empty,
                Estado = 0,
                UpdatedAt = DateTime.MinValue,
                MFARequerido = 1
            };

            var sql = @"SP_SEL_EMPRESA_USUARIO_ESPECIFICO";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(user.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(user.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", user.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_USU_RUT", user.RutUsuario);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                empresaUsuario.Id = Convert.ToUInt32(rs["EU_ID"]);
                                empresaUsuario.RutUsuario = Convert.ToString(rs["USU_RUT"]);
                                empresaUsuario.NombreUsuario = Convert.ToString(rs["USU_NOMBRE"]);
                                empresaUsuario.ApellidoUsuario = Convert.ToString(rs["USU_APELLIDO"]);
                                empresaUsuario.Email = Convert.ToString(rs["EU_EMAIL"]);
                                empresaUsuario.Telefono = Convert.ToString(rs["EU_TELEFONO"]);
                                empresaUsuario.NombreEmpresa = Convert.ToString(rs["EMP_NOMBRE"]);
                                empresaUsuario.RutEmpresa = Convert.ToString(rs["EMP_RUT"]);
                                empresaUsuario.EstadoEmpresa = Convert.ToSByte(rs["EMP_ESTADO"]);
                                empresaUsuario.IdPerfil = Convert.ToUInt32(rs["PER_ID"]);
                                empresaUsuario.NombrePerfil = Convert.ToString(rs["PER_NOMBRE"]);
                                empresaUsuario.PasswordHash = Convert.ToString(rs["EU_PASSWORD_HASH"]);
                                empresaUsuario.PasswordSalt = Convert.ToString(rs["EU_PASSWORD_SALT"]);
                                empresaUsuario.PasswordAlgoritmo = Convert.ToString(rs["EU_PASSWORD_ALGORITMO"]);
                                empresaUsuario.Estado = Convert.ToSByte(rs["EU_ESTADO"]);
                                empresaUsuario.MFARequerido = Convert.ToSByte(rs["EU_MFA_REQUERIDO"]);
                                empresaUsuario.UpdatedAt = Convert.ToDateTime(rs["EU_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetEmpresaUsuario), consulta);

                operacion = -1;
            }

            return empresaUsuario;
        }

        public static int UpdateEmpresaUsuario(EmpresaUsuarioModelo empresaUsuario)
        {
            var retorno = 0;
            var sql = @"SP_UPD_EMPRESA_USUARIO";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresaUsuario.RutUsuario) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.NombreUsuario) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.ApellidoUsuario) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.Email) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.Telefono) + "," +
                empresaUsuario.IdPerfil + "," +
                (empresaUsuario.Estado.HasValue ? empresaUsuario.Estado.Value.ToString() : "NULL") + "," +
                "'[HASH_OMITIDO]'," +
                "'[SALT_OMITIDO]'," +
                "'[ALGORITMO_OMITIDO]'," +
                FuncionesDatos.TextoSql(empresaUsuario.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.RutUsuarioBitacora);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresaUsuario.RutUsuario);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresaUsuario.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_USU_NOMBRE", empresaUsuario.NombreUsuario);
                        cmd.Parameters.AddWithValue("P_USU_APELLIDO", empresaUsuario.ApellidoUsuario);
                        cmd.Parameters.AddWithValue("P_EU_EMAIL", empresaUsuario.Email);
                        cmd.Parameters.AddWithValue("P_EU_TELEFONO", empresaUsuario.Telefono);
                        cmd.Parameters.AddWithValue("P_PER_ID", empresaUsuario.IdPerfil);
                        cmd.Parameters.AddWithValue("P_EU_ESTADO", empresaUsuario.Estado.HasValue ? (object)empresaUsuario.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_HASH", string.IsNullOrWhiteSpace(empresaUsuario.PasswordHash) ? (object)DBNull.Value : empresaUsuario.PasswordHash);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_SALT", string.IsNullOrWhiteSpace(empresaUsuario.PasswordSalt) ? (object)DBNull.Value : empresaUsuario.PasswordSalt);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_ALGORITMO", string.IsNullOrWhiteSpace(empresaUsuario.PasswordAlgoritmo) ? (object)DBNull.Value : empresaUsuario.PasswordAlgoritmo);
                        cmd.Parameters.AddWithValue("P_EU_UPDATED_AT_ORIGINAL", empresaUsuario.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT_BITACORA", empresaUsuario.RutUsuarioBitacora);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateEmpresaUsuario), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateEmpresaUsuario), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int InsertEmpresaUsuario(EmpresaUsuarioModelo empresaUsuario)
        {
            var retorno = 0;
            var sql = @"SP_INS_EMPRESA_USUARIO";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresaUsuario.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.RutUsuario) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.NombreUsuario) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.ApellidoUsuario) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.Email) + "," +
                FuncionesDatos.TextoSql(empresaUsuario.Telefono) + "," +
                empresaUsuario.IdPerfil + "," +
                (empresaUsuario.Estado.HasValue ? empresaUsuario.Estado.Value.ToString() : "NULL") + "," +
                "'[HASH_OMITIDO]'," +
                "'[SALT_OMITIDO]'," +
                "'[ALGORITMO_OMITIDO]'," +
                FuncionesDatos.TextoSql(empresaUsuario.RutUsuarioBitacora);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresaUsuario.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresaUsuario.RutUsuario);
                        cmd.Parameters.AddWithValue("P_USU_NOMBRE", empresaUsuario.NombreUsuario);
                        cmd.Parameters.AddWithValue("P_USU_APELLIDO", empresaUsuario.ApellidoUsuario);
                        cmd.Parameters.AddWithValue("P_EU_EMAIL", empresaUsuario.Email);
                        cmd.Parameters.AddWithValue("P_EU_TELEFONO", empresaUsuario.Telefono);
                        cmd.Parameters.AddWithValue("P_PER_ID", empresaUsuario.IdPerfil);
                        cmd.Parameters.AddWithValue("P_EU_ESTADO", empresaUsuario.Estado.HasValue ? (object)empresaUsuario.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_HASH", empresaUsuario.PasswordHash);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_SALT", empresaUsuario.PasswordSalt);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_ALGORITMO", empresaUsuario.PasswordAlgoritmo);
                        cmd.Parameters.AddWithValue("P_USU_RUT_BITACORA", empresaUsuario.RutUsuarioBitacora);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertEmpresaUsuario), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertEmpresaUsuario), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateUsuarioPassword(EmpresaUsuarioModelo usuario)
        {
            var retorno = 0;
            var sql = @"SP_UPD_EMPRESA_USUARIO_PASSWORD";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(usuario.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(usuario.RutUsuario) + "," +
                "'[HASH_OMITIDO]'," +
                "'[SALT_OMITIDO]'," +
                "'[ALGORITMO_OMITIDO]'";

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", usuario.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_USU_RUT", usuario.RutUsuario);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_HASH", usuario.PasswordHash);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_SALT", usuario.PasswordSalt);
                        cmd.Parameters.AddWithValue("P_EU_PASSWORD_ALGORITMO", usuario.PasswordAlgoritmo);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateUsuarioPassword), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateUsuarioPassword), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}