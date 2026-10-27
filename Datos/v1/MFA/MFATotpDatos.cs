using Datos.Comunes;
using Modelo;
using Modelo.v1.MFA;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Datos.v1.MFA
{
    public class MFATotpDatos
    {
        public static MFATotpModelo GetMfaTotp(ref int operacion, uint? empresaUsuarioId, string rutSuperAdmin)
        {
            MFATotpModelo mfaTotp = null;
            const string sql = "SP_SEL_MFA_TOTP";
            var consulta = sql + " " + (empresaUsuarioId.HasValue ? empresaUsuarioId.Value.ToString() : "NULL") + "," + FuncionesDatos.TextoSql(rutSuperAdmin);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EU_ID", empresaUsuarioId.HasValue ? (object)empresaUsuarioId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_SA_RUT", string.IsNullOrWhiteSpace(rutSuperAdmin) ? (object)DBNull.Value : rutSuperAdmin);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mfaTotp = MapearMfaTotp(reader);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetMfaTotp), consulta);
                operacion = -1;
            }

            return mfaTotp;
        }

        public static int InsertMfaTotp(MFATotpModelo mfaTotp, out uint mfaTotpId)
        {
            mfaTotpId = 0;

            const string sql = "SP_INS_MFA_TOTP";
            var consulta = sql + " '[SECRET_PROTEGIDO_OMITIDO]'," + FuncionesDatos.TextoSql(mfaTotp.Algoritmo) + "," + mfaTotp.Digitos + "," + mfaTotp.PeriodoSegundos + "," + (mfaTotp.EmpresaUsuarioId.HasValue ? mfaTotp.EmpresaUsuarioId.Value.ToString() : "NULL") + "," + FuncionesDatos.TextoSql(mfaTotp.RutSuperAdmin);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("P_MT_SECRET_PROTEGIDO", MySqlDbType.VarBinary, 512).Value = mfaTotp.SecretProtegido;
                        cmd.Parameters.Add("P_MT_ALGORITMO", MySqlDbType.VarChar, 10).Value = mfaTotp.Algoritmo;
                        cmd.Parameters.Add("P_MT_DIGITOS", MySqlDbType.UByte).Value = mfaTotp.Digitos;
                        cmd.Parameters.Add("P_MT_PERIODO_SEGUNDOS", MySqlDbType.UInt16).Value = mfaTotp.PeriodoSegundos;
                        cmd.Parameters.Add("P_EU_ID", MySqlDbType.UInt32).Value = mfaTotp.EmpresaUsuarioId.HasValue ? (object)mfaTotp.EmpresaUsuarioId.Value : DBNull.Value;
                        cmd.Parameters.Add("P_SA_RUT", MySqlDbType.VarChar, 10).Value = string.IsNullOrWhiteSpace(mfaTotp.RutSuperAdmin) ? (object)DBNull.Value : mfaTotp.RutSuperAdmin;
                        cmd.Parameters.Add("P_MT_ID", MySqlDbType.UInt32).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? byte.MaxValue);

                        if (retornoSp == 0)
                        {
                            if (cmd.Parameters["P_MT_ID"].Value != null && cmd.Parameters["P_MT_ID"].Value != DBNull.Value)
                            {
                                mfaTotpId = Convert.ToUInt32(cmd.Parameters["P_MT_ID"].Value);
                            }

                            return 0;
                        }

                        FuncionesDatos.RegistrarErrorDatos(new Exception(ConstantesModelo.ConsErrorMfaTotpSpRetornoNoExitoso + retornoSp), nameof(InsertMfaTotp), consulta);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertMfaTotp), consulta);
                return -1;
            }
        }

        public static int UpdateMFATotpConfirmar(uint mfaTotpId, long ultimoPasoUsado, string codigosRecuperacionHash)
        {
            const string sql = "SP_UPD_MFA_TOTP_CONFIRMAR";
            var consulta = sql + " " + mfaTotpId + "," + ultimoPasoUsado;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("P_MT_ID", MySqlDbType.UInt32).Value = mfaTotpId;
                        cmd.Parameters.Add("P_MT_ULTIMO_PASO_USADO", MySqlDbType.Int64).Value = ultimoPasoUsado;
                        cmd.Parameters.Add("P_MCR_CODIGOS_HASH", MySqlDbType.Text).Value = codigosRecuperacionHash;
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? byte.MaxValue);

                        if (retornoSp == 0)
                        {
                            return 0;
                        }

                        FuncionesDatos.RegistrarErrorDatos(new Exception(ConstantesModelo.ConsErrorMfaTotpSpRetornoNoExitoso + retornoSp), nameof(UpdateMFATotpConfirmar), consulta);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateMFATotpConfirmar), consulta);
                return -1;
            }
        }

        public static int UpdateMFATotpIntentoFallido(uint mfaTotpId, ushort intentosMaximos, ushort minutosBloqueo)
        {
            const string sql = "SP_UPD_MFA_TOTP_INTENTO_FALLIDO";
            var consulta = sql + " " + mfaTotpId + "," + intentosMaximos + "," + minutosBloqueo;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("P_MT_ID", MySqlDbType.UInt32).Value = mfaTotpId;
                        cmd.Parameters.Add("P_MT_INTENTOS_MAXIMOS", MySqlDbType.UInt16).Value = intentosMaximos;
                        cmd.Parameters.Add("P_MT_MINUTOS_BLOQUEO", MySqlDbType.UInt16).Value = minutosBloqueo;
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? byte.MaxValue);

                        if (retornoSp == 0)
                        {
                            return 0;
                        }

                        FuncionesDatos.RegistrarErrorDatos(new Exception(ConstantesModelo.ConsErrorMfaTotpSpRetornoNoExitoso + retornoSp), nameof(UpdateMFATotpIntentoFallido), consulta);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateMFATotpIntentoFallido), consulta);
                return -1;
            }
        }

        public static int UpdateMFATotpValidacionExitosa(uint mfaTotpId, long ultimoPasoUsado)
        {
            const string sql = "SP_UPD_MFA_TOTP_VALIDACION_EXITOSA";
            var consulta = sql + " " + mfaTotpId + "," + ultimoPasoUsado;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("P_MT_ID", MySqlDbType.UInt32).Value = mfaTotpId;
                        cmd.Parameters.Add("P_MT_ULTIMO_PASO_USADO", MySqlDbType.Int64).Value = ultimoPasoUsado;
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? byte.MaxValue);

                        if (retornoSp == 0)
                        {
                            return 0;
                        }

                        FuncionesDatos.RegistrarErrorDatos(new Exception(ConstantesModelo.ConsErrorMfaTotpSpRetornoNoExitoso + retornoSp), nameof(UpdateMFATotpValidacionExitosa), consulta);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateMFATotpValidacionExitosa), consulta);
                return -1;
            }
        }

        public static int UpdateMFACodigoRecuperacionUsar(uint mfaTotpId, string codigoHash)
        {
            const string sql = "SP_UPD_MFA_CODIGO_RECUPERACION_USAR";
            var consulta = sql + " " + mfaTotpId + ",'[HASH_CODIGO_RECUPERACION_OMITIDO]'";

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("P_MT_ID", MySqlDbType.UInt32).Value = mfaTotpId;
                        cmd.Parameters.Add("P_MCR_CODIGO_HASH", MySqlDbType.VarChar, ConstantesModelo.ConsMfaTotpCodigoRecuperacionHashHexLargo).Value = codigoHash;
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? byte.MaxValue);

                        if (retornoSp == 0 || retornoSp == 1)
                        {
                            return retornoSp;
                        }

                        FuncionesDatos.RegistrarErrorDatos(new Exception(ConstantesModelo.ConsErrorMfaTotpSpRetornoNoExitoso + retornoSp), nameof(UpdateMFACodigoRecuperacionUsar), consulta);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateMFACodigoRecuperacionUsar), consulta);
                return -1;
            }
        }

        public static int UpdateMFACodigoRecuperacionRegenerar(uint mfaTotpId, string codigosRecuperacionHash, string rutUsuario)
        {
            const string sql = "SP_UPD_MFA_CODIGO_RECUPERACION_REGENERAR";
            var consulta = sql + " " + mfaTotpId + ",'[CODIGOS_HASH_OMITIDOS]'," + FuncionesDatos.TextoSql(rutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("P_MT_ID", MySqlDbType.UInt32).Value = mfaTotpId;
                        cmd.Parameters.Add("P_MCR_CODIGOS_HASH", MySqlDbType.Text).Value = codigosRecuperacionHash;
                        cmd.Parameters.Add("P_USU_RUT", MySqlDbType.VarChar, 10).Value = rutUsuario;
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? byte.MaxValue);

                        if (retornoSp == 0 || retornoSp == 1 || retornoSp == 2)
                        {
                            return retornoSp;
                        }

                        FuncionesDatos.RegistrarErrorDatos(new Exception(ConstantesModelo.ConsErrorMfaTotpSpRetornoNoExitoso + retornoSp), nameof(UpdateMFACodigoRecuperacionRegenerar), consulta);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateMFACodigoRecuperacionRegenerar), consulta);
                return -1;
            }
        }

        public static int UpdateMFATotpRestablecer(uint mfaTotpId, byte[] secretProtegido, string rutUsuario)
        {
            const string sql = "SP_UPD_MFA_TOTP_RESTABLECER";
            var consulta = sql + " " + mfaTotpId + ",'[SECRET_PROTEGIDO_OMITIDO]'," + FuncionesDatos.TextoSql(rutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("P_MT_ID", MySqlDbType.UInt32).Value = mfaTotpId;
                        cmd.Parameters.Add("P_MT_SECRET_PROTEGIDO", MySqlDbType.VarBinary, 512).Value = secretProtegido;
                        cmd.Parameters.Add("P_USU_RUT", MySqlDbType.VarChar, 10).Value = rutUsuario;
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? byte.MaxValue);

                        if (retornoSp == 0 || retornoSp == 1)
                        {
                            return retornoSp;
                        }

                        FuncionesDatos.RegistrarErrorDatos(new Exception(ConstantesModelo.ConsErrorMfaTotpSpRetornoNoExitoso + retornoSp), nameof(UpdateMFATotpRestablecer), consulta);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateMFATotpRestablecer), consulta);
                return -1;
            }
        }

        private static MFATotpModelo MapearMfaTotp(IDataRecord reader)
        {
            return new MFATotpModelo
            {
                Id = Convert.ToUInt32(reader["MT_ID"]),
                SecretProtegido = (byte[])reader["MT_SECRET_PROTEGIDO"],
                Algoritmo = Convert.ToString(reader["MT_ALGORITMO"]),
                Digitos = Convert.ToByte(reader["MT_DIGITOS"]),
                PeriodoSegundos = Convert.ToUInt16(reader["MT_PERIODO_SEGUNDOS"]),
                Configurado = Convert.ToSByte(reader["MT_CONFIGURADO"]),
                Estado = Convert.ToSByte(reader["MT_ESTADO"]),
                IntentosFallidos = Convert.ToUInt16(reader["MT_INTENTOS_FALLIDOS"]),
                BloqueadoHasta = reader["MT_BLOQUEADO_HASTA"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["MT_BLOQUEADO_HASTA"]),
                UltimoPasoUsado = reader["MT_ULTIMO_PASO_USADO"] == DBNull.Value ? (long?)null : Convert.ToInt64(reader["MT_ULTIMO_PASO_USADO"]),
                CreatedAt = Convert.ToDateTime(reader["MT_CREATED_AT"]),
                UpdatedAt = Convert.ToDateTime(reader["MT_UPDATED_AT"]),
                ConfirmedAt = reader["MT_CONFIRMED_AT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["MT_CONFIRMED_AT"]),
                LastUsedAt = reader["MT_LAST_USED_AT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["MT_LAST_USED_AT"]),
                EmpresaUsuarioId = reader["EU_ID"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(reader["EU_ID"]),
                RutSuperAdmin = reader["SA_RUT"] == DBNull.Value ? string.Empty : Convert.ToString(reader["SA_RUT"])
            };
        }
    }
}