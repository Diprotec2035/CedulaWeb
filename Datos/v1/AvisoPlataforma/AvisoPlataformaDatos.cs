using Datos.Comunes;
using Modelo.v1.AvisoPlataforma;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Datos.v1.AvisoPlataforma
{
    public class AvisoPlataformaDatos
    {
        public static List<AvisoPlataformaModelo> GetReporteAvisosPlataforma(ref int operacion, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<AvisoPlataformaModelo>();
            var sql = @"SP_SEL_REPORTE_AVISO_PLATAFORMA";

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
                        cmd.Parameters.AddWithValue("P_AP_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearAvisoPlataforma(reader));
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteAvisosPlataforma), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static AvisoPlataformaModelo GetAvisoPlataforma(ref int operacion, AvisoPlataformaModelo aviso)
        {
            var avisoPlataforma = new AvisoPlataformaModelo();

            var sql = @"SP_SEL_AVISO_PLATAFORMA";

            var consulta = sql + " " +
                aviso.Id + "," +
                (aviso.Estado.HasValue ? aviso.Estado.Value.ToString() : "NULL");

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_AP_ID", aviso.Id == 0 ? (object)DBNull.Value : aviso.Id);
                        cmd.Parameters.AddWithValue("P_AP_ESTADO", aviso.Estado.HasValue ? (object)aviso.Estado.Value : DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                avisoPlataforma = MapearAvisoPlataforma(reader);
                                avisoPlataforma.UpdatedAt = Convert.ToDateTime(reader["AP_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetAvisoPlataforma), consulta);

                operacion = -1;
            }

            return avisoPlataforma;
        }

        public static AvisoPlataformaModelo GetAvisoPlataformaVigente(ref int operacion)
        {
            var avisoPlataforma = new AvisoPlataformaModelo();

            var sql = @"SP_SEL_AVISO_PLATAFORMA_VIGENTE";
            var consulta = sql;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                avisoPlataforma = MapearAvisoPlataforma(reader);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetAvisoPlataformaVigente), consulta);

                operacion = -1;
            }

            return avisoPlataforma;
        }

        public static int InsertAvisoPlataforma(AvisoPlataformaModelo aviso)
        {
            var retorno = 0;
            var sql = @"SP_INS_AVISO_PLATAFORMA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(aviso.Titulo) + "," +
                FuncionesDatos.TextoSql(aviso.Mensaje) + "," +
                FuncionesDatos.TextoSql(aviso.Version) + "," +
                FuncionesDatos.TextoSql(aviso.FechaInicio) + "," +
                FuncionesDatos.TextoSql(aviso.FechaTermino) + "," +
                (aviso.Estado.HasValue ? aviso.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(aviso.RutUsuario);

            try
            {
                var fechaInicio = DateTime.ParseExact(aviso.FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var fechaTermino = DateTime.ParseExact(aviso.FechaTermino, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_AP_TITULO", aviso.Titulo);
                        cmd.Parameters.AddWithValue("P_AP_MENSAJE", aviso.Mensaje);
                        cmd.Parameters.AddWithValue("P_AP_VERSION", aviso.Version);
                        cmd.Parameters.Add("P_AP_FECHA_INICIO", MySqlDbType.Date).Value = fechaInicio;
                        cmd.Parameters.Add("P_AP_FECHA_TERMINO", MySqlDbType.Date).Value = fechaTermino;
                        cmd.Parameters.AddWithValue("P_AP_ESTADO", aviso.Estado.HasValue ? (object)aviso.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", aviso.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertAvisoPlataforma), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertAvisoPlataforma), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateAvisoPlataforma(AvisoPlataformaModelo aviso)
        {
            var retorno = 0;
            var sql = @"SP_UPD_AVISO_PLATAFORMA";

            var consulta = sql + " " +
                aviso.Id + "," +
                FuncionesDatos.TextoSql(aviso.Titulo) + "," +
                FuncionesDatos.TextoSql(aviso.Mensaje) + "," +
                FuncionesDatos.TextoSql(aviso.Version) + "," +
                FuncionesDatos.TextoSql(aviso.FechaInicio) + "," +
                FuncionesDatos.TextoSql(aviso.FechaTermino) + "," +
                (aviso.Estado.HasValue ? aviso.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(aviso.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(aviso.RutUsuario);

            try
            {
                var fechaInicio = DateTime.ParseExact(aviso.FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var fechaTermino = DateTime.ParseExact(aviso.FechaTermino, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_AP_ID", aviso.Id);
                        cmd.Parameters.AddWithValue("P_AP_TITULO", aviso.Titulo);
                        cmd.Parameters.AddWithValue("P_AP_MENSAJE", aviso.Mensaje);
                        cmd.Parameters.AddWithValue("P_AP_VERSION", aviso.Version);
                        cmd.Parameters.Add("P_AP_FECHA_INICIO", MySqlDbType.Date).Value = fechaInicio;
                        cmd.Parameters.Add("P_AP_FECHA_TERMINO", MySqlDbType.Date).Value = fechaTermino;
                        cmd.Parameters.AddWithValue("P_AP_ESTADO", aviso.Estado.HasValue ? (object)aviso.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_AP_UPDATED_AT_ORIGINAL", aviso.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", aviso.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateAvisoPlataforma), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateAvisoPlataforma), consulta);

                retorno = -1;
            }

            return retorno;
        }

        private static AvisoPlataformaModelo MapearAvisoPlataforma(IDataRecord reader)
        {
            return new AvisoPlataformaModelo
            {
                Id = Convert.ToUInt32(reader["AP_ID"]),
                Titulo = Convert.ToString(reader["AP_TITULO"]),
                Mensaje = Convert.ToString(reader["AP_MENSAJE"]),
                Version = Convert.ToString(reader["AP_VERSION"]),
                FechaInicio = Convert.ToString(reader["AP_FECHA_INICIO"]),
                FechaTermino = Convert.ToString(reader["AP_FECHA_TERMINO"]),
                Estado = Convert.ToSByte(reader["AP_ESTADO"]),
                EstadoNombre = Convert.ToString(reader["AP_ESTADO_NOMBRE"])
            };
        }
    }
}