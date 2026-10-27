using Datos.Comunes;
using Modelo.v1.VersionAplicacion;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.VersionAplicacion
{
    public class VersionAplicacionDatos
    {
        public static List<VersionAplicacionModelo> GetReporteVersionesAplicacion(ref int operacion, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<VersionAplicacionModelo>();
            var sql = "SP_SEL_REPORTE_APP_VERSION";

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
                        cmd.Parameters.AddWithValue("P_AV_IS_ACTIVE", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new VersionAplicacionModelo()
                                {
                                    Id = Convert.ToUInt32(reader["AV_ID"]),
                                    VersionCode = Convert.ToInt32(reader["AV_VERSION_CODE"]),
                                    VersionName = Convert.ToString(reader["AV_VERSION_NAME"]),
                                    MinSupportedVersionCode = Convert.ToInt32(reader["AV_MIN_SUPPORTED_VERSION_CODE"]),
                                    ForceUpdate = Convert.ToSByte(reader["AV_FORCE_UPDATE"]),
                                    IsActive = Convert.ToSByte(reader["AV_IS_ACTIVE"]),
                                    Ispublished = Convert.ToSByte(reader["AV_IS_PUBLISHED"]),
                                    Fabricante = Convert.ToString(reader["AV_FABRICANTE"]),
                                    AndroidVersion = Convert.ToInt32(reader["AV_ANDROID_VERSION"]),
                                    Modelo = Convert.ToString(reader["AV_MODELO"]),
                                    ModeloKey = Convert.ToString(reader["AV_MODELO_KEY"]),
                                    ApkFileName = Convert.ToString(reader["AV_APK_FILE_NAME"]),
                                    ApkRelativePath = Convert.ToString(reader["AV_APK_RELATIVE_PATH"]),
                                    ApkSha256 = Convert.ToString(reader["AV_APK_SHA256"]),
                                    FileSizeBytes = Convert.ToInt64(reader["AV_FILE_SIZE_BYTES"]),
                                    ReleaseNotes = Convert.ToString(reader["AV_RELEASE_NOTES"]),
                                    CreatedAt = Convert.ToString(reader["AV_CREATED_AT"]),
                                    CreatedBy = Convert.ToString(reader["AV_CREATED_BY"]),
                                    PublishedAt = Convert.ToString(reader["AV_PUBLISHED_AT"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteVersionesAplicacion), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static VersionAplicacionModelo GetUltimaVersion(ref int operacion, VersionAplicacionModelo ver)
        {
            var version = new VersionAplicacionModelo
            {
                Id = 0,
                VersionCode = 0,
                VersionName = string.Empty,
                MinSupportedVersionCode = 0,
                ForceUpdate = 0,
                IsActive = 0,
                Ispublished = 0,
                Fabricante = string.Empty,
                AndroidVersion = 0,
                Modelo = string.Empty,
                ModeloKey = string.Empty,
                ApkFileName = string.Empty,
                ApkRelativePath = string.Empty,
                ApkSha256 = string.Empty,
                FileSizeBytes = 0,
                ReleaseNotes = string.Empty,
                CreatedAt = string.Empty,
                CreatedBy = string.Empty,
                PublishedAt = string.Empty
            };

            var sql = "SP_SEL_APP_VERSION_ULTIMA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(ver.Fabricante) + "," +
                FuncionesDatos.TextoSql(ver.Modelo) + "," +
                ver.AndroidVersion;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_AV_FABRICANTE", ver.Fabricante);
                        cmd.Parameters.AddWithValue("P_AV_MODELO", ver.Modelo);
                        cmd.Parameters.AddWithValue("P_AV_ANDROID_VERSION", ver.AndroidVersion);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                version.Id = Convert.ToUInt32(rs["AV_ID"]);
                                version.VersionCode = Convert.ToInt32(rs["AV_VERSION_CODE"]);
                                version.VersionName = Convert.ToString(rs["AV_VERSION_NAME"]);
                                version.Fabricante = Convert.ToString(rs["AV_FABRICANTE"]);
                                version.AndroidVersion = Convert.ToInt32(rs["AV_ANDROID_VERSION"]);
                                version.Modelo = Convert.ToString(rs["AV_MODELO"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetUltimaVersion), consulta);

                operacion = -1;
            }

            return version;
        }

        public static VersionAplicacionModelo GetVersionAplicacion(ref int operacion, VersionAplicacionModelo ver)
        {
            var version = new VersionAplicacionModelo
            {
                Id = 0,
                VersionCode = 0,
                VersionName = string.Empty,
                MinSupportedVersionCode = 0,
                ForceUpdate = 0,
                IsActive = 0,
                Ispublished = 0,
                Fabricante = string.Empty,
                AndroidVersion = 0,
                Modelo = string.Empty,
                ModeloKey = string.Empty,
                ApkFileName = string.Empty,
                ApkRelativePath = string.Empty,
                ApkSha256 = string.Empty,
                FileSizeBytes = 0,
                ReleaseNotes = string.Empty,
                CreatedAt = string.Empty,
                CreatedBy = string.Empty,
                PublishedAt = string.Empty,
                UpdatedAt = DateTime.MinValue
            };

            var sql = "SP_SEL_APP_VERSION_ID";

            var consulta = sql + " " +
                ver.Id;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_AV_ID", ver.Id);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                version.Id = Convert.ToUInt32(rs["AV_ID"]);
                                version.VersionCode = Convert.ToInt32(rs["AV_VERSION_CODE"]);
                                version.VersionName = Convert.ToString(rs["AV_VERSION_NAME"]);
                                version.MinSupportedVersionCode = Convert.ToInt32(rs["AV_MIN_SUPPORTED_VERSION_CODE"]);
                                version.ForceUpdate = Convert.ToSByte(rs["AV_FORCE_UPDATE"]);
                                version.IsActive = Convert.ToSByte(rs["AV_IS_ACTIVE"]);
                                version.Ispublished = Convert.ToSByte(rs["AV_IS_PUBLISHED"]);
                                version.Fabricante = Convert.ToString(rs["AV_FABRICANTE"]);
                                version.AndroidVersion = Convert.ToInt32(rs["AV_ANDROID_VERSION"]);
                                version.Modelo = Convert.ToString(rs["AV_MODELO"]);
                                version.ModeloKey = Convert.ToString(rs["AV_MODELO_KEY"]);
                                version.ApkFileName = Convert.ToString(rs["AV_APK_FILE_NAME"]);
                                version.ApkRelativePath = Convert.ToString(rs["AV_APK_RELATIVE_PATH"]);
                                version.ApkSha256 = Convert.ToString(rs["AV_APK_SHA256"]);
                                version.FileSizeBytes = Convert.ToInt64(rs["AV_FILE_SIZE_BYTES"]);
                                version.ReleaseNotes = Convert.ToString(rs["AV_RELEASE_NOTES"]);
                                version.CreatedAt = Convert.ToString(rs["AV_CREATED_AT"]);
                                version.CreatedBy = Convert.ToString(rs["AV_CREATED_BY"]);
                                version.PublishedAt = Convert.ToString(rs["AV_PUBLISHED_AT"]);
                                version.UpdatedAt = Convert.ToDateTime(rs["AV_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetVersionAplicacion), consulta);

                operacion = -1;
            }

            return version;
        }

        public static int UpdateVersionAplicacion(VersionAplicacionModelo version)
        {
            var retorno = 0;
            var sql = @"SP_UPD_APP_VERSION";

            var consulta = sql + " " +
                version.Id + "," +
                version.ForceUpdate + "," +
                version.IsActive + "," +
                version.Ispublished + "," +
                FuncionesDatos.TextoSql(version.Fabricante) + "," +
                FuncionesDatos.TextoSql(version.Modelo) + "," +
                version.AndroidVersion + "," +
                FuncionesDatos.TextoSql(version.ReleaseNotes) + "," +
                FuncionesDatos.TextoSql(version.PublishedAt) + "," +
                FuncionesDatos.TextoSql(version.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(version.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_AV_ID", version.Id);
                        cmd.Parameters.AddWithValue("P_AV_FORCE_UPDATE", version.ForceUpdate);
                        cmd.Parameters.AddWithValue("P_AV_IS_ACTIVE", version.IsActive);
                        cmd.Parameters.AddWithValue("P_AV_IS_PUBLISHED", version.Ispublished);
                        cmd.Parameters.AddWithValue("P_AV_FABRICANTE", version.Fabricante);
                        cmd.Parameters.AddWithValue("P_AV_MODELO", version.Modelo);
                        cmd.Parameters.AddWithValue("P_AV_ANDROID_VERSION", version.AndroidVersion);
                        cmd.Parameters.AddWithValue("P_AV_RELEASE_NOTES", version.ReleaseNotes);
                        cmd.Parameters.AddWithValue("P_AV_PUBLISHED_AT", string.IsNullOrWhiteSpace(version.PublishedAt) ? (object)DBNull.Value : version.PublishedAt);
                        cmd.Parameters.AddWithValue("P_AV_UPDATED_AT_ORIGINAL", version.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", version.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateVersionAplicacion), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateVersionAplicacion), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int InsertVersionAplicacion(VersionAplicacionModelo version)
        {
            var retorno = 0;
            var sql = "SP_INS_APP_VERSION";

            var consulta = sql + " " +
                version.VersionCode + "," +
                FuncionesDatos.TextoSql(version.VersionName) + "," +
                version.MinSupportedVersionCode + "," +
                version.ForceUpdate + "," +
                version.IsActive + "," +
                version.Ispublished + "," +
                FuncionesDatos.TextoSql(version.Fabricante) + "," +
                version.AndroidVersion + "," +
                FuncionesDatos.TextoSql(version.Modelo) + "," +
                FuncionesDatos.TextoSql(version.ApkFileName) + "," +
                FuncionesDatos.TextoSql(version.ApkRelativePath) + "," +
                FuncionesDatos.TextoSql(version.ApkSha256) + "," +
                version.FileSizeBytes + "," +
                FuncionesDatos.TextoSql(version.ReleaseNotes) + "," +
                FuncionesDatos.TextoSql(version.CreatedAt) + "," +
                FuncionesDatos.TextoSql(version.CreatedBy) + "," +
                FuncionesDatos.TextoSql(version.PublishedAt) + "," +
                FuncionesDatos.TextoSql(version.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_AV_VERSION_CODE", version.VersionCode);
                        cmd.Parameters.AddWithValue("P_AV_VERSION_NAME", version.VersionName);
                        cmd.Parameters.AddWithValue("P_AV_MIN_SUPPORTED_VERSION_CODE", version.MinSupportedVersionCode);
                        cmd.Parameters.AddWithValue("P_AV_FORCE_UPDATE", version.ForceUpdate);
                        cmd.Parameters.AddWithValue("P_AV_IS_ACTIVE", version.IsActive);
                        cmd.Parameters.AddWithValue("P_AV_IS_PUBLISHED", version.Ispublished);
                        cmd.Parameters.AddWithValue("P_AV_FABRICANTE", version.Fabricante);
                        cmd.Parameters.AddWithValue("P_AV_ANDROID_VERSION", version.AndroidVersion);
                        cmd.Parameters.AddWithValue("P_AV_MODELO", version.Modelo);
                        cmd.Parameters.AddWithValue("P_AV_APK_FILE_NAME", version.ApkFileName);
                        cmd.Parameters.AddWithValue("P_AV_APK_RELATIVE_PATH", version.ApkRelativePath);
                        cmd.Parameters.AddWithValue("P_AV_APK_SHA256", version.ApkSha256);
                        cmd.Parameters.AddWithValue("P_AV_FILE_SIZE_BYTES", version.FileSizeBytes);
                        cmd.Parameters.AddWithValue("P_AV_RELEASE_NOTES", string.IsNullOrWhiteSpace(version.ReleaseNotes) ? (object)DBNull.Value : version.ReleaseNotes);
                        cmd.Parameters.AddWithValue("P_AV_CREATED_AT", version.CreatedAt);
                        cmd.Parameters.AddWithValue("P_AV_CREATED_BY", version.CreatedBy);
                        cmd.Parameters.AddWithValue("P_AV_PUBLISHED_AT", string.IsNullOrWhiteSpace(version.PublishedAt) ? (object)DBNull.Value : version.PublishedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", version.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertVersionAplicacion), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertVersionAplicacion), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}