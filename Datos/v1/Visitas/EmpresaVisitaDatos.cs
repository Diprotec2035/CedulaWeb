using Datos.Comunes;
using Modelo.v1.Visitas;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Visitas
{
    public class EmpresaVisitaDatos
    {
        public static List<EmpresaVisitaModelo> GetReporteEmpresaVisitas(ref int operacion, string rutEmpresa, string buscador, short? vigente, short orden, short orientacion)
        {
            var lista = new List<EmpresaVisitaModelo>();
            var sql = @"SP_SEL_REPORTE_EMPRESA_VISITA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(rutEmpresa) + "," +
                FuncionesDatos.TextoSql(buscador) + "," +
                (vigente.HasValue ? vigente.Value.ToString() : "NULL") + "," +
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
                        cmd.Parameters.AddWithValue("P_EV_VIGENTE", vigente.HasValue ? (object)vigente.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new EmpresaVisitaModelo
                                {
                                    Rut = Convert.ToString(reader["EV_RUT"]),
                                    Nombre = Convert.ToString(reader["EV_NOMBRE"]),
                                    Empresa = Convert.ToString(reader["EV_EMPRESA"]),
                                    RutEmpresa = Convert.ToString(reader["EMP_RUT"]),
                                    Vigente = Convert.ToSByte(reader["EV_VIGENTE"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteEmpresaVisitas), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static EmpresaVisitaModelo GetEmpresaVisita(ref int operacion, EmpresaVisitaModelo ev)
        {
            var empresaVisita = new EmpresaVisitaModelo
            {
                Rut = string.Empty,
                Nombre = string.Empty,
                Empresa = string.Empty,
                RutEmpresa = string.Empty,
                Vigente = 0,
                UpdatedAt = DateTime.MinValue
            };

            var sql = @"SP_SEL_EMPRESA_VISITA_ESPECIFICO_V2";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(ev.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(ev.Rut);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", ev.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_EV_RUT", ev.Rut);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                empresaVisita.Rut = Convert.ToString(rs["EV_RUT"]);
                                empresaVisita.Nombre = Convert.ToString(rs["EV_NOMBRE"]);
                                empresaVisita.Empresa = Convert.ToString(rs["EV_EMPRESA"]);
                                empresaVisita.RutEmpresa = Convert.ToString(rs["EMP_RUT"]);
                                empresaVisita.Vigente = Convert.ToSByte(rs["EV_VIGENTE"]);
                                empresaVisita.UpdatedAt = Convert.ToDateTime(rs["EV_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetEmpresaVisita), consulta);

                operacion = -1;
            }

            return empresaVisita;
        }

        public static int InsertEmpresaVisita(EmpresaVisitaModelo empresaVisita)
        {
            var retorno = 0;
            var sql = @"SP_INS_EMPRESA_VISITA_V2";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresaVisita.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(empresaVisita.Rut) + "," +
                FuncionesDatos.TextoSql(empresaVisita.Nombre) + "," +
                FuncionesDatos.TextoSql(empresaVisita.Empresa) + "," +
                empresaVisita.Vigente + "," +
                FuncionesDatos.TextoSql(empresaVisita.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresaVisita.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_EV_RUT", empresaVisita.Rut);
                        cmd.Parameters.AddWithValue("P_EV_NOMBRE", empresaVisita.Nombre);
                        cmd.Parameters.AddWithValue("P_EV_EMPRESA", empresaVisita.Empresa);
                        cmd.Parameters.AddWithValue("P_EV_VIGENTE", empresaVisita.Vigente);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresaVisita.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertEmpresaVisita), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertEmpresaVisita), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateEmpresaVisita(EmpresaVisitaModelo empresaVisita)
        {
            var retorno = 0;
            var sql = @"SP_UPD_EMPRESA_VISITA_V2";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(empresaVisita.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(empresaVisita.Rut) + "," +
                FuncionesDatos.TextoSql(empresaVisita.Nombre) + "," +
                FuncionesDatos.TextoSql(empresaVisita.Empresa) + "," +
                empresaVisita.Vigente + "," +
                FuncionesDatos.TextoSql(empresaVisita.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(empresaVisita.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", empresaVisita.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_EV_RUT", empresaVisita.Rut);
                        cmd.Parameters.AddWithValue("P_EV_NOMBRE", empresaVisita.Nombre);
                        cmd.Parameters.AddWithValue("P_EV_EMPRESA", empresaVisita.Empresa);
                        cmd.Parameters.AddWithValue("P_EV_VIGENTE", empresaVisita.Vigente);
                        cmd.Parameters.AddWithValue("P_EV_UPDATED_AT_ORIGINAL", empresaVisita.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", empresaVisita.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateEmpresaVisita), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateEmpresaVisita), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}
