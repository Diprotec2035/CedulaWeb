using Datos.Comunes;
using Modelo.v1.Empresa;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Datos.v1.Empresa
{
    public class EmpresaTokenDatos
    {
        public static int InsertEmpresaToken(EmpresaTokenModelo empresaToken)
        {
            var retorno = 0;
            var sql = @"SP_INS_EMPRESA_TOKEN_V2";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresaToken.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(empresaToken.Jti) + "," +
                FuncionesDatos.TextoSql("[PROTEGIDO]") + "," +
                (
                    empresaToken.ExpiresAt.HasValue
                        ? FuncionesDatos.TextoSql(
                            empresaToken.ExpiresAt.Value.ToString(
                                "yyyy-MM-dd HH:mm:ss.ffffff"
                            )
                        )
                        : "NULL"
                ) + "," +
                FuncionesDatos.TextoSql(empresaToken.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresaToken.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_ET_JTI", empresaToken.Jti);
                        cmd.Parameters.AddWithValue("P_ET_TOKEN_HASH", empresaToken.TokenHash);
                        cmd.Parameters.AddWithValue("P_ET_EXPIRES_AT", empresaToken.ExpiresAt.HasValue ? (object)empresaToken.ExpiresAt.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresaToken.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(
                                new Exception("El SP retornó un código no exitoso: " + retornoSp),
                                nameof(InsertEmpresaToken),
                                consulta
                            );

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertEmpresaToken), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateEmpresaTokenVigencia(EmpresaTokenModelo empresaToken)
        {
            var retorno = 0;
            var sql = @"SP_UPD_EMPRESA_TOKEN_VIGENCIA_V2";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresaToken.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(empresaToken.Jti) + "," +
                FuncionesDatos.TextoSql("[PROTEGIDO]") + "," +
                (
                    empresaToken.ExpiresAt.HasValue
                        ? FuncionesDatos.TextoSql(
                            empresaToken.ExpiresAt.Value.ToString(
                                "yyyy-MM-dd HH:mm:ss.ffffff"
                            )
                        )
                        : "NULL"
                ) + "," +
                FuncionesDatos.TextoSql(empresaToken.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresaToken.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_ET_JTI", empresaToken.Jti);
                        cmd.Parameters.AddWithValue("P_ET_TOKEN_HASH", empresaToken.TokenHash);
                        cmd.Parameters.AddWithValue("P_ET_EXPIRES_AT", empresaToken.ExpiresAt.HasValue ? (object)empresaToken.ExpiresAt.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresaToken.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(
                                new Exception("El SP retornó un código no exitoso: " + retornoSp),
                                nameof(UpdateEmpresaTokenVigencia),
                                consulta
                            );

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateEmpresaTokenVigencia), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateEmpresaTokenRevocar(EmpresaTokenModelo empresaToken)
        {
            var retorno = 0;
            var sql = @"SP_UPD_EMPRESA_TOKEN_REVOCAR";

            var consulta = sql + " " +
                empresaToken.Id + "," +
                FuncionesDatos.TextoSql(empresaToken.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(empresaToken.RutUsuario) + "," +
                FuncionesDatos.TextoSql(empresaToken.RevocationReason);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_ET_ID", empresaToken.Id);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresaToken.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresaToken.RutUsuario);
                        cmd.Parameters.AddWithValue("P_ET_REVOCATION_REASON", empresaToken.RevocationReason);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateEmpresaTokenRevocar), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateEmpresaTokenRevocar), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static EmpresaTokenCabeceraModelo GetEmpresaTokenCabecera(ref int operacion, string rutEmpresa)
        {
            var token = new EmpresaTokenCabeceraModelo
            {
                Id = null,
                RutEmpresa = string.Empty,
                CreatedAt = null,
                ExpiresAt = null,
                Revocado = null,
                TokenTotal = 0,
                TokenTotalOperativos = 0,
                TokenVigenciaCodigo = 6,
                TokenVigenciaNombre = string.Empty,
                TokenDiasRestantes = null,
                TokenAnteriorExpiresAt = null,
                TokenAnteriorDiasRestantes = null
            };

            var sql = @"SP_SEL_EMPRESA_TOKEN_CABECERA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(rutEmpresa);

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

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                token.Id = reader["ET_ID"] == DBNull.Value
                                    ? (uint?)null
                                    : Convert.ToUInt32(reader["ET_ID"]);

                                token.RutEmpresa = Convert.ToString(reader["EMP_RUT"]);

                                token.CreatedAt = reader["ET_CREATED_AT"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader["ET_CREATED_AT"]);

                                token.ExpiresAt = reader["ET_EXPIRES_AT"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader["ET_EXPIRES_AT"]);

                                token.Revocado = reader["ET_REVOCADO"] == DBNull.Value
                                    ? (sbyte?)null
                                    : Convert.ToSByte(reader["ET_REVOCADO"]);

                                token.TokenTotal = Convert.ToUInt32(reader["TOKEN_TOTAL"]);
                                token.TokenTotalOperativos = Convert.ToUInt32(reader["TOKEN_TOTAL_OPERATIVOS"]);
                                token.TokenVigenciaCodigo = Convert.ToSByte(reader["TOKEN_VIGENCIA_CODIGO"]);
                                token.TokenVigenciaNombre = Convert.ToString(reader["TOKEN_VIGENCIA_NOMBRE"]);

                                token.TokenDiasRestantes = reader["TOKEN_DIAS_RESTANTES"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["TOKEN_DIAS_RESTANTES"]);

                                token.TokenAnteriorExpiresAt = reader["TOKEN_ANTERIOR_EXPIRES_AT"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader["TOKEN_ANTERIOR_EXPIRES_AT"]);

                                token.TokenAnteriorDiasRestantes = reader["TOKEN_ANTERIOR_DIAS_RESTANTES"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["TOKEN_ANTERIOR_DIAS_RESTANTES"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetEmpresaTokenCabecera), consulta);

                operacion = -1;
            }

            return token;
        }
    }
}