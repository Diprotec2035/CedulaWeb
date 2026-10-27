using Datos.Comunes;
using Modelo.v1.Regla;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Regla
{
    public class ReglaDatos
    {
        public static List<ReglaModelo> GetReporteReglas(ref int operacion, string rutEmpresa, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<ReglaModelo>();
            var sql = @"SP_SEL_REPORTE_REGLAS";

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
                        cmd.Parameters.AddWithValue("P_REG_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new ReglaModelo
                                {
                                    Id = Convert.ToUInt32(reader["REG_ID"]),
                                    Nombre = Convert.ToString(reader["REG_NOMBRE"]),
                                    NombreApellido = Convert.ToSByte(reader["REG_NOMBRE_APELLIDO"]),
                                    Empresa = Convert.ToSByte(reader["REG_EMPRESA"]),
                                    Patente = Convert.ToSByte(reader["REG_PATENTE"]),
                                    Comentario = Convert.ToSByte(reader["REG_COMENTARIO"]),
                                    Fotografia = Convert.ToSByte(reader["REG_FOTOGRAFIA"]),
                                    EntradaSalida = Convert.ToSByte(reader["REG_ENTRADA_SALIDA"]),
                                    ListaBlancaNegra = Convert.ToSByte(reader["REG_LISTA_BLANCA_NEGRA"]),
                                    EliminaEnviados = Convert.ToSByte(reader["REG_ELIMINA_ENVIADOS"]),
                                    RutEmpresa = Convert.ToString(reader["EMP_RUT"]),
                                    Estado = Convert.ToSByte(reader["REG_ESTADO"]),
                                    EstadoNombre = Convert.ToString(reader["REG_ESTADO_NOMBRE"]),
                                    UpdatedAt = Convert.ToDateTime(reader["REG_UPDATED_AT"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteReglas), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static ReglaModelo GetRegla(ref int operacion, ReglaModelo reg)
        {
            var regla = new ReglaModelo
            {
                Id = 0,
                Nombre = string.Empty,
                NombreApellido = 0,
                Empresa = 0,
                Patente = 0,
                Comentario = 0,
                Fotografia = 0,
                EntradaSalida = 0,
                ListaBlancaNegra = 0,
                EliminaEnviados = 0,
                RutEmpresa = string.Empty,
                Estado = 0,
                UpdatedAt = DateTime.MinValue
            };

            var sql = @"SP_SEL_REGLA";

            var consulta = sql + " " +
                reg.Id + "," +
                (reg.Estado.HasValue ? reg.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(reg.RutEmpresa);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_REG_ID", reg.Id);
                        cmd.Parameters.AddWithValue("P_REG_ESTADO", reg.Estado.HasValue ? (object)reg.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_EMP_RUT", reg.RutEmpresa);

                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                regla.Id = Convert.ToUInt32(rs["REG_ID"]);
                                regla.RutEmpresa = Convert.ToString(rs["EMP_RUT"]);
                                regla.Nombre = Convert.ToString(rs["REG_NOMBRE"]);
                                regla.NombreApellido = Convert.ToSByte(rs["REG_NOMBRE_APELLIDO"]);
                                regla.Empresa = Convert.ToSByte(rs["REG_EMPRESA"]);
                                regla.Patente = Convert.ToSByte(rs["REG_PATENTE"]);
                                regla.Comentario = Convert.ToSByte(rs["REG_COMENTARIO"]);
                                regla.Fotografia = Convert.ToSByte(rs["REG_FOTOGRAFIA"]);
                                regla.EntradaSalida = Convert.ToSByte(rs["REG_ENTRADA_SALIDA"]);
                                regla.ListaBlancaNegra = Convert.ToSByte(rs["REG_LISTA_BLANCA_NEGRA"]);
                                regla.EliminaEnviados = Convert.ToSByte(rs["REG_ELIMINA_ENVIADOS"]);
                                regla.Estado = Convert.ToSByte(rs["REG_ESTADO"]);
                                regla.UpdatedAt = Convert.ToDateTime(rs["REG_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetRegla), consulta);

                operacion = -1;
            }

            return regla;
        }

        public static int UpdateRegla(ReglaModelo regla)
        {
            var retorno = 0;
            var sql = @"SP_UPD_REGLA";

            var consulta = sql + " " +
                regla.Id + "," +
                FuncionesDatos.TextoSql(regla.Nombre) + "," +
                regla.NombreApellido + "," +
                regla.Empresa + "," +
                regla.Patente + "," +
                regla.Comentario + "," +
                regla.Fotografia + "," +
                regla.EntradaSalida + "," +
                regla.ListaBlancaNegra + "," +
                regla.EliminaEnviados + "," +
                (regla.Estado.HasValue ? regla.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(regla.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(regla.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_REG_ID", regla.Id);
                        cmd.Parameters.AddWithValue("P_REG_NOMBRE", regla.Nombre);
                        cmd.Parameters.AddWithValue("P_REG_NOMBRE_APELLIDO", regla.NombreApellido);
                        cmd.Parameters.AddWithValue("P_REG_EMPRESA", regla.Empresa);
                        cmd.Parameters.AddWithValue("P_REG_PATENTE", regla.Patente);
                        cmd.Parameters.AddWithValue("P_REG_COMENTARIO", regla.Comentario);
                        cmd.Parameters.AddWithValue("P_REG_FOTOGRAFIA", regla.Fotografia);
                        cmd.Parameters.AddWithValue("P_REG_ENTRADA_SALIDA", regla.EntradaSalida);
                        cmd.Parameters.AddWithValue("P_REG_LISTA_BLANCA_NEGRA", regla.ListaBlancaNegra);
                        cmd.Parameters.AddWithValue("P_REG_ELIMINA_ENVIADOS", regla.EliminaEnviados);
                        cmd.Parameters.AddWithValue("P_REG_ESTADO", regla.Estado.HasValue ? (object)regla.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_REG_UPDATED_AT_ORIGINAL", regla.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", regla.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else if (retornoSp == 2)
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateRegla), consulta);

                            retorno = -3;
                        }
                        else if (retornoSp == 3)
                        {
                            retorno = 3;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateRegla), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateRegla), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int InsertRegla(ReglaModelo regla)
        {
            var retorno = 0;
            var sql = @"SP_INS_REGLA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(regla.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(regla.Nombre) + "," +
                regla.NombreApellido + "," +
                regla.Empresa + "," +
                regla.Patente + "," +
                regla.Comentario + "," +
                regla.Fotografia + "," +
                regla.EntradaSalida + "," +
                regla.ListaBlancaNegra + "," +
                regla.EliminaEnviados + "," +
                (regla.Estado.HasValue ? regla.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(regla.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_EMP_RUT", regla.RutEmpresa);
                        cmd.Parameters.AddWithValue("P_REG_NOMBRE", regla.Nombre);
                        cmd.Parameters.AddWithValue("P_REG_NOMBRE_APELLIDO", regla.NombreApellido);
                        cmd.Parameters.AddWithValue("P_REG_EMPRESA", regla.Empresa);
                        cmd.Parameters.AddWithValue("P_REG_PATENTE", regla.Patente);
                        cmd.Parameters.AddWithValue("P_REG_COMENTARIO", regla.Comentario);
                        cmd.Parameters.AddWithValue("P_REG_FOTOGRAFIA", regla.Fotografia);
                        cmd.Parameters.AddWithValue("P_REG_ENTRADA_SALIDA", regla.EntradaSalida);
                        cmd.Parameters.AddWithValue("P_REG_LISTA_BLANCA_NEGRA", regla.ListaBlancaNegra);
                        cmd.Parameters.AddWithValue("P_REG_ELIMINA_ENVIADOS", regla.EliminaEnviados);
                        cmd.Parameters.AddWithValue("P_REG_ESTADO", regla.Estado.HasValue ? (object)regla.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", regla.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertRegla), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertRegla), consulta);

                retorno = -1;
            }

            return retorno;
        }
    }
}
