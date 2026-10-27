using Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Datos.Comunes;

namespace Datos
{
    public class EmpresaDatos
    {
        public static List<EmpresaModelo> GetReporteEmpresas(ref int operacion, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<EmpresaModelo>();
            var sql = @"SP_SEL_REPORTE_EMPRESAS_V2";
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
                        cmd.Parameters.AddWithValue("P_EMP_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new EmpresaModelo
                                {
                                    Rut = Convert.ToString(reader["EMP_RUT"]),
                                    Nombre = Convert.ToString(reader["EMP_NOMBRE"]),
                                    Direccion = Convert.ToString(reader["EMP_DIRECCION"]),
                                    Comuna = Convert.ToString(reader["EMP_COMUNA"]),
                                    Ciudad = Convert.ToString(reader["EMP_CIUDAD"]),
                                    Contacto = Convert.ToString(reader["EMP_CONTACTO"]),
                                    ContactoTelefono = Convert.ToString(reader["EMP_CONTACTO_TELEFONO"]),
                                    ContactoEmail = Convert.ToString(reader["EMP_CONTACTO_EMAIL"]),
                                    DispositivoExterno = Convert.ToSByte(reader["EMP_DISPOSITIVO_EXTERNO"]),
                                    Estado = Convert.ToSByte(reader["EMP_ESTADO"]),
                                    TokenId = reader["ET_ID"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(reader["ET_ID"]),
                                    TokenCreatedAt = reader["ET_CREATED_AT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ET_CREATED_AT"]),
                                    TokenExpiresAt = reader["ET_EXPIRES_AT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ET_EXPIRES_AT"]),
                                    TokenTotal = Convert.ToInt32(reader["ET_TOTAL_TOKENS"]),
                                    TokenTotalOperativos = Convert.ToInt32(reader["ET_TOTAL_OPERATIVOS"]),
                                    TokenVigenciaCodigo = Convert.ToSByte(reader["ET_VIGENCIA_CODIGO"]),
                                    TokenVigenciaNombre = Convert.ToString(reader["ET_VIGENCIA_NOMBRE"]),
                                    TokenDiasRestantes = reader["ET_DIAS_RESTANTES"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ET_DIAS_RESTANTES"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteEmpresas), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static EmpresaModelo GetEmpresa(ref int operacion, EmpresaModelo emp)
        {
            var empresa = new EmpresaModelo
            {
                Rut = string.Empty,
                Nombre = string.Empty,
                Direccion = string.Empty,
                Comuna = string.Empty,
                Ciudad = string.Empty,
                Contacto = string.Empty,
                ContactoTelefono = string.Empty,
                ContactoEmail = string.Empty,
                DispositivoExterno = 1,
                Estado = 0,
                UpdatedAt = DateTime.MinValue
            };

            var sql = @"SP_SEL_EMPRESA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(emp.Rut) + "," +
                (emp.Estado.HasValue ? emp.Estado.Value.ToString() : "NULL");

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", string.IsNullOrWhiteSpace(emp.Rut) ? (object)DBNull.Value : emp.Rut);
                        cmd.Parameters.AddWithValue("P_EMP_ESTADO", emp.Estado.HasValue ? (object)emp.Estado.Value : DBNull.Value);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                empresa.Rut = Convert.ToString(rs["EMP_RUT"]);
                                empresa.Nombre = Convert.ToString(rs["EMP_NOMBRE"]);
                                empresa.Direccion = Convert.ToString(rs["EMP_DIRECCION"]);
                                empresa.Comuna = Convert.ToString(rs["EMP_COMUNA"]);
                                empresa.Ciudad = Convert.ToString(rs["EMP_CIUDAD"]);
                                empresa.Contacto = Convert.ToString(rs["EMP_CONTACTO"]);
                                empresa.ContactoTelefono = Convert.ToString(rs["EMP_CONTACTO_TELEFONO"]);
                                empresa.ContactoEmail = Convert.ToString(rs["EMP_CONTACTO_EMAIL"]);
                                empresa.DispositivoExterno = Convert.ToSByte(rs["EMP_DISPOSITIVO_EXTERNO"]);
                                empresa.Estado = Convert.ToSByte(rs["EMP_ESTADO"]);
                                empresa.UpdatedAt = Convert.ToDateTime(rs["EMP_UPDATED_AT"]);

                                operacion = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetEmpresa), consulta);

                operacion = -1;
            }

            return empresa;
        }

        public static int UpdateEmpresa(EmpresaModelo empresa)
        {
            var retorno = 0;
            var sql = @"SP_UPD_EMPRESA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresa.Rut) + "," +
                FuncionesDatos.TextoSql(empresa.Nombre) + "," +
                FuncionesDatos.TextoSql(empresa.Direccion) + "," +
                FuncionesDatos.TextoSql(empresa.Comuna) + "," +
                FuncionesDatos.TextoSql(empresa.Ciudad) + "," +
                FuncionesDatos.TextoSql(empresa.Contacto) + "," +
                FuncionesDatos.TextoSql(empresa.ContactoTelefono) + "," +
                FuncionesDatos.TextoSql(empresa.ContactoEmail) + "," +
                empresa.DispositivoExterno + "," +
                empresa.Estado + "," +
                FuncionesDatos.TextoSql(empresa.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(empresa.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresa.Rut);
                        cmd.Parameters.AddWithValue("P_EMP_NOMBRE", empresa.Nombre);
                        cmd.Parameters.AddWithValue("P_EMP_DIRECCION", empresa.Direccion);
                        cmd.Parameters.AddWithValue("P_EMP_COMUNA", empresa.Comuna);
                        cmd.Parameters.AddWithValue("P_EMP_CIUDAD", empresa.Ciudad);
                        cmd.Parameters.AddWithValue("P_EMP_CONTACTO", empresa.Contacto);
                        cmd.Parameters.AddWithValue("P_EMP_CONTACTO_TELEFONO", empresa.ContactoTelefono);
                        cmd.Parameters.AddWithValue("P_EMP_CONTACTO_EMAIL", empresa.ContactoEmail);
                        cmd.Parameters.AddWithValue("P_EMP_DISPOSITIVO_EXTERNO", empresa.DispositivoExterno);
                        cmd.Parameters.AddWithValue("P_EMP_ESTADO", empresa.Estado);
                        cmd.Parameters.AddWithValue("P_EMP_UPDATED_AT_ORIGINAL", empresa.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresa.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateEmpresa), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateEmpresa), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int InsertEmpresa(EmpresaModelo empresa)
        {
            var retorno = 0;
            var sql = @"SP_INS_EMPRESA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresa.Rut) + "," +
                FuncionesDatos.TextoSql(empresa.Nombre) + "," +
                FuncionesDatos.TextoSql(empresa.Direccion) + "," +
                FuncionesDatos.TextoSql(empresa.Comuna) + "," +
                FuncionesDatos.TextoSql(empresa.Ciudad) + "," +
                FuncionesDatos.TextoSql(empresa.Contacto) + "," +
                FuncionesDatos.TextoSql(empresa.ContactoTelefono) + "," +
                FuncionesDatos.TextoSql(empresa.ContactoEmail) + "," +
                (empresa.DispositivoExterno.HasValue ? empresa.DispositivoExterno.Value.ToString() : "NULL") + "," +
                (empresa.Estado.HasValue ? empresa.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(empresa.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresa.Rut);
                        cmd.Parameters.AddWithValue("P_EMP_NOMBRE", empresa.Nombre);
                        cmd.Parameters.AddWithValue("P_EMP_DIRECCION", empresa.Direccion);
                        cmd.Parameters.AddWithValue("P_EMP_COMUNA", empresa.Comuna);
                        cmd.Parameters.AddWithValue("P_EMP_CIUDAD", empresa.Ciudad);
                        cmd.Parameters.AddWithValue("P_EMP_CONTACTO", empresa.Contacto);
                        cmd.Parameters.AddWithValue("P_EMP_CONTACTO_TELEFONO", empresa.ContactoTelefono);
                        cmd.Parameters.AddWithValue("P_EMP_CONTACTO_EMAIL", empresa.ContactoEmail);
                        cmd.Parameters.AddWithValue("P_EMP_DISPOSITIVO_EXTERNO", empresa.DispositivoExterno.HasValue ? (object)empresa.DispositivoExterno.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_EMP_ESTADO", empresa.Estado.HasValue ? (object)empresa.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresa.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertEmpresa), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertEmpresa), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}