using Datos.Comunes;
using Modelo.v1.BaseDatos;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.BaseDatos
{
    public class BaseDatosDatos
    {
        public static List<BaseDatosTablaModelo> GetTablas(ref int operacion)
        {
            var lista = new List<BaseDatosTablaModelo>();
            var sql = @"SP_SEL_BD_TABLAS";
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
                            while (reader.Read())
                            {
                                lista.Add(new BaseDatosTablaModelo
                                {
                                    Nombre = Convert.ToString(reader["TAB_NOMBRE"])
                                });
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetTablas), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static List<BaseDatosProcedimientoModelo> GetProcedimientosTabla(ref int operacion, string tablaNombre)
        {
            var lista = new List<BaseDatosProcedimientoModelo>();
            var sql = @"SP_SEL_BD_PROCEDIMIENTOS_TABLA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(tablaNombre);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_TAB_NOMBRE", tablaNombre);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new BaseDatosProcedimientoModelo
                                {
                                    Nombre = Convert.ToString(reader["SP_NOMBRE"])
                                });
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetProcedimientosTabla), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static List<BaseDatosColumnaModelo> GetColumnasTabla(ref int operacion, string tablaNombre)
        {
            var lista = new List<BaseDatosColumnaModelo>();
            var sql = @"SP_SEL_BD_COLUMNAS_TABLA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(tablaNombre);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_TAB_NOMBRE", tablaNombre);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new BaseDatosColumnaModelo
                                {
                                    Nombre = Convert.ToString(reader["COL_NOMBRE"])
                                });
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetColumnasTabla), consulta);

                operacion = -1;
            }

            return lista;
        }
    }
}