using Datos.Comunes;
using Modelo;
using Modelo.v1.Categoria;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Categoria
{
    public class CategoriaDatos
    {
        public static List<CategoriaModelo> GetReporteCategorias(ref int operacion, string buscador, short? estado, short orden, short orientacion)
        {
            var lista = new List<CategoriaModelo>();
            var sql = @"SP_SEL_REPORTE_CATEGORIAS";

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
                        cmd.Parameters.AddWithValue("P_CAT_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new CategoriaModelo()
                                {
                                    Id = Convert.ToUInt32(reader["CAT_ID"]),
                                    Nombre = Convert.ToString(reader["CAT_NOMBRE"]),
                                    Orden = Convert.ToSByte(reader["CAT_ORDEN"]),
                                    Estado = Convert.ToSByte(reader["CAT_ESTADO"]),
                                    EstadoNombre = Convert.ToString(reader["CAT_ESTADO_NOMBRE"]),
                                    Icono = Convert.ToString(reader["CAT_ICONO"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteCategorias), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static CategoriaModelo GetCategoria(ref int operacion, CategoriaModelo categoria)
        {
            var cat = new CategoriaModelo
            {
                Id = 0,
                Nombre = string.Empty,
                Orden = 0,
                Estado = 0,
                EstadoNombre = string.Empty,
                Icono = string.Empty,
                UpdatedAt = DateTime.MinValue
            };

            var sql = @"SP_SEL_CATEGORIA";

            var consulta = sql + " " +
                categoria.Id + "," +
                (categoria.Estado.HasValue ? categoria.Estado.Value.ToString() : "NULL");

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_CAT_ID", categoria.Id);
                        cmd.Parameters.AddWithValue("P_CAT_ESTADO", categoria.Estado.HasValue ? (object)categoria.Estado.Value : DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cat.Id = Convert.ToUInt32(reader["CAT_ID"]);
                                cat.Nombre = Convert.ToString(reader["CAT_NOMBRE"]);
                                cat.Orden = Convert.ToSByte(reader["CAT_ORDEN"]);
                                cat.Estado = Convert.ToSByte(reader["CAT_ESTADO"]);
                                cat.EstadoNombre = Convert.ToString(reader["CAT_ESTADO_NOMBRE"]);
                                cat.Icono = Convert.ToString(reader["CAT_ICONO"]);
                                cat.UpdatedAt = Convert.ToDateTime(reader["CAT_UPDATED_AT"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetCategoria), consulta);

                operacion = -1;
            }

            return cat;
        }

        public static int InsertCategoria(CategoriaModelo categoria)
        {
            var retorno = 0;
            var sql = @"SP_INS_CATEGORIA";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(categoria.Nombre) + "," +
                (categoria.Orden.HasValue ? categoria.Orden.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(categoria.Icono) + "," +
                (categoria.Estado.HasValue ? categoria.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(categoria.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_CAT_NOMBRE", categoria.Nombre);
                        cmd.Parameters.AddWithValue("P_CAT_ORDEN", categoria.Orden.HasValue ? (object)categoria.Orden.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAT_ICONO", string.IsNullOrWhiteSpace(categoria.Icono) ? (object)DBNull.Value : categoria.Icono);
                        cmd.Parameters.AddWithValue("P_CAT_ESTADO", categoria.Estado.HasValue ? (object)categoria.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_USU_RUT", categoria.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertCategoria), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertCategoria), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateCategoria(CategoriaModelo categoria)
        {
            var retorno = 0;
            var sql = @"SP_UPD_CATEGORIA";

            var consulta = sql + " " +
                categoria.Id + "," +
                FuncionesDatos.TextoSql(categoria.Nombre) + "," +
                (categoria.Orden.HasValue ? categoria.Orden.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(categoria.Icono) + "," +
                (categoria.Estado.HasValue ? categoria.Estado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(categoria.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(categoria.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_CAT_ID", categoria.Id);
                        cmd.Parameters.AddWithValue("P_CAT_NOMBRE", categoria.Nombre);
                        cmd.Parameters.AddWithValue("P_CAT_ORDEN", categoria.Orden.HasValue ? (object)categoria.Orden.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAT_ICONO", string.IsNullOrWhiteSpace(categoria.Icono) ? (object)DBNull.Value : categoria.Icono);
                        cmd.Parameters.AddWithValue("P_CAT_ESTADO", categoria.Estado.HasValue ? (object)categoria.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAT_UPDATED_AT_ORIGINAL", categoria.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", categoria.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateCategoria), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateCategoria), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static List<CategoriaModelo> GetCategoriaUsuario(ref int operacion, EmpresaUsuarioModelo usuario)
        {
            var lista = new List<CategoriaModelo>();
            var sql = @"SP_SEL_CATEGORIA_USUARIO";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(usuario.RutEmpresa) + "," +
                FuncionesDatos.TextoSql(usuario.RutUsuario);

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

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new CategoriaModelo()
                                {
                                    Id = Convert.ToUInt32(reader["CAT_ID"]),
                                    Nombre = Convert.ToString(reader["CAT_NOMBRE"]),
                                    Orden = Convert.ToSByte(reader["CAT_ORDEN"]),
                                    Estado = Convert.ToSByte(reader["CAT_ESTADO"]),
                                    Icono = Convert.ToString(reader["CAT_ICONO"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetCategoriaUsuario), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static List<CategoriaModelo> GetCategoriasMenu(ref int operacion)
        {
            var lista = new List<CategoriaModelo>();
            var sql = @"SP_SEL_CATEGORIA_MENU";

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
                                var fila = new CategoriaModelo()
                                {
                                    Id = Convert.ToUInt32(reader["CAT_ID"]),
                                    Nombre = Convert.ToString(reader["CAT_NOMBRE"]),
                                    Orden = Convert.ToSByte(reader["CAT_ORDEN"]),
                                    Estado = Convert.ToSByte(reader["CAT_ESTADO"]),
                                    Icono = Convert.ToString(reader["CAT_ICONO"])
                                };

                                lista.Add(fila);
                            }
                        }
                    }
                }

                operacion = 0;
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetCategoriasMenu), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static List<CategoriaModelo> GetCategoriasModulosPerfil(ref int operacion)
        {
            var lista = new List<CategoriaModelo>();
            var sql = @"SP_SEL_CATEGORIA_MODULOS_PERFIL";

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
                                var fila = new CategoriaModelo()
                                {
                                    Id = Convert.ToUInt32(reader["CAT_ID"]),
                                    Nombre = Convert.ToString(reader["CAT_NOMBRE"]),
                                    Orden = Convert.ToSByte(reader["CAT_ORDEN"]),
                                    Estado = Convert.ToSByte(reader["CAT_ESTADO"]),
                                    Icono = Convert.ToString(reader["CAT_ICONO"])
                                };

                                lista.Add(fila);
                            }
                        }
                    }
                }

                operacion = 0;
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetCategoriasModulosPerfil), consulta);

                operacion = -1;
            }

            return lista;
        }
    }
}