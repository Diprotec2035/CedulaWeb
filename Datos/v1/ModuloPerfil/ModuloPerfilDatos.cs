using Datos.Comunes;
using Modelo.v1.ModuloPerfil;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.ModuloPerfil
{
    public class ModuloPerfilDatos
    {
        public static List<ModuloPerfilModelo> GetModulosPerfil(ref int operacion, string rutEmpresa, uint? perfil, uint? categoria)
        {
            var lista = new List<ModuloPerfilModelo>();
            var sql = "SP_SEL_MODULOS_PERFIL";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(rutEmpresa) + "," +
                (perfil.HasValue ? perfil.Value.ToString() : "NULL") + "," +
                (categoria.HasValue ? categoria.Value.ToString() : "NULL");

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
                        cmd.Parameters.AddWithValue("P_PER_ID", perfil.HasValue ? (object)perfil.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAT_ID", categoria.HasValue ? (object)categoria.Value : DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new ModuloPerfilModelo
                                {
                                    Id = Convert.ToUInt32(reader["MP_ID"]),
                                    IdPerfil = Convert.ToUInt32(reader["PER_ID"]),
                                    IdModulo = Convert.ToUInt32(reader["MOD_ID"]),
                                    NombreModulo = Convert.ToString(reader["MOD_NOMBRE"]),
                                    Consulta = Convert.ToSByte(reader["MP_CONSULTA"]),
                                    Nuevo = Convert.ToSByte(reader["MP_NUEVO"]),
                                    Modifica = Convert.ToSByte(reader["MP_MODIFICA"]),
                                    Estado = Convert.ToSByte(reader["MP_ESTADO"]),
                                    UpdatedAt = reader["MP_UPDATED_AT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["MP_UPDATED_AT"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModulosPerfil), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static PermisoModuloUsuarioModelo GetPermisoModuloUsuario(ref int operacion, string rutEmpresa, string rutUsuario, string urlModuloPublica)
        {
            PermisoModuloUsuarioModelo permiso = null;
            var sql = "SP_SEL_MODULO_PERFIL_USUARIO";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(rutEmpresa) + "," +
                FuncionesDatos.TextoSql(rutUsuario) + "," +
                FuncionesDatos.TextoSql(urlModuloPublica);

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
                        cmd.Parameters.AddWithValue("P_USU_RUT", rutUsuario);
                        cmd.Parameters.AddWithValue("P_MOD_URL", urlModuloPublica);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                permiso = new PermisoModuloUsuarioModelo
                                {
                                    IdModulo = Convert.ToUInt32(reader["MOD_ID"]),
                                    NombreModulo = Convert.ToString(reader["MOD_NOMBRE"]),
                                    Estado = Convert.ToSByte(reader["MP_ESTADO"]),
                                    Consulta = Convert.ToSByte(reader["MP_CONSULTA"]),
                                    Nuevo = Convert.ToSByte(reader["MP_NUEVO"]),
                                    Modifica = Convert.ToSByte(reader["MP_MODIFICA"])
                                };
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetPermisoModuloUsuario), consulta);

                operacion = -1;
            }

            return permiso;
        }

        public static int UpdateModuloPerfilEstado(ModuloPerfilModelo moduloPerfil)
        {
            var retorno = 0;
            var sql = @"SP_UPS_MODULO_PERFIL_ESTADO";

            var consulta = sql + " " +
                moduloPerfil.Id + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutEmpresa) + "," +
                moduloPerfil.IdPerfil + "," +
                moduloPerfil.IdModulo + "," +
                moduloPerfil.Estado + "," +
                (moduloPerfil.UpdatedAt.HasValue ? FuncionesDatos.TextoSql(moduloPerfil.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) : "NULL") + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MP_ID", moduloPerfil.Id);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", moduloPerfil.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_PER_ID", moduloPerfil.IdPerfil);
                        cmd.Parameters.AddWithValue("P_MOD_ID", moduloPerfil.IdModulo);
                        cmd.Parameters.AddWithValue("P_MP_ESTADO", moduloPerfil.Estado);
                        cmd.Parameters.AddWithValue("P_MP_UPDATED_AT_ORIGINAL", moduloPerfil.UpdatedAt.HasValue ? (object)moduloPerfil.UpdatedAt.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", moduloPerfil.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateModuloPerfilEstado), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateModuloPerfilEstado), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateModuloPerfilNuevo(ModuloPerfilModelo moduloPerfil)
        {
            var retorno = 0;
            var sql = @"SP_UPS_MODULO_PERFIL_NUEVO";

            var consulta = sql + " " +
                moduloPerfil.Id + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutEmpresa) + "," +
                moduloPerfil.IdPerfil + "," +
                moduloPerfil.IdModulo + "," +
                moduloPerfil.Nuevo + "," +
                (moduloPerfil.UpdatedAt.HasValue ? FuncionesDatos.TextoSql(moduloPerfil.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) : "NULL") + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MP_ID", moduloPerfil.Id);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", moduloPerfil.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_PER_ID", moduloPerfil.IdPerfil);
                        cmd.Parameters.AddWithValue("P_MOD_ID", moduloPerfil.IdModulo);
                        cmd.Parameters.AddWithValue("P_MP_NUEVO", moduloPerfil.Nuevo);
                        cmd.Parameters.AddWithValue("P_MP_UPDATED_AT_ORIGINAL", moduloPerfil.UpdatedAt.HasValue ? (object)moduloPerfil.UpdatedAt.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", moduloPerfil.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateModuloPerfilNuevo), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateModuloPerfilNuevo), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateModuloPerfilConsulta(ModuloPerfilModelo moduloPerfil)
        {
            var retorno = 0;
            var sql = @"SP_UPS_MODULO_PERFIL_CONSULTA";

            var consulta = sql + " " +
                moduloPerfil.Id + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutEmpresa) + "," +
                moduloPerfil.IdPerfil + "," +
                moduloPerfil.IdModulo + "," +
                moduloPerfil.Consulta + "," +
                (moduloPerfil.UpdatedAt.HasValue ? FuncionesDatos.TextoSql(moduloPerfil.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) : "NULL") + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MP_ID", moduloPerfil.Id);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", moduloPerfil.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_PER_ID", moduloPerfil.IdPerfil);
                        cmd.Parameters.AddWithValue("P_MOD_ID", moduloPerfil.IdModulo);
                        cmd.Parameters.AddWithValue("P_MP_CONSULTA", moduloPerfil.Consulta);
                        cmd.Parameters.AddWithValue("P_MP_UPDATED_AT_ORIGINAL", moduloPerfil.UpdatedAt.HasValue ? (object)moduloPerfil.UpdatedAt.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", moduloPerfil.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateModuloPerfilConsulta), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateModuloPerfilConsulta), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateModuloPerfilModifica(ModuloPerfilModelo moduloPerfil)
        {
            var retorno = 0;
            var sql = @"SP_UPS_MODULO_PERFIL_MODIFICA";

            var consulta = sql + " " +
                moduloPerfil.Id + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutEmpresa) + "," +
                moduloPerfil.IdPerfil + "," +
                moduloPerfil.IdModulo + "," +
                moduloPerfil.Modifica + "," +
                (moduloPerfil.UpdatedAt.HasValue ? FuncionesDatos.TextoSql(moduloPerfil.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) : "NULL") + "," +
                FuncionesDatos.TextoSql(moduloPerfil.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MP_ID", moduloPerfil.Id);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", moduloPerfil.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_PER_ID", moduloPerfil.IdPerfil);
                        cmd.Parameters.AddWithValue("P_MOD_ID", moduloPerfil.IdModulo);
                        cmd.Parameters.AddWithValue("P_MP_MODIFICA", moduloPerfil.Modifica);
                        cmd.Parameters.AddWithValue("P_MP_UPDATED_AT_ORIGINAL", moduloPerfil.UpdatedAt.HasValue ? (object)moduloPerfil.UpdatedAt.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", moduloPerfil.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateModuloPerfilModifica), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateModuloPerfilModifica), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}