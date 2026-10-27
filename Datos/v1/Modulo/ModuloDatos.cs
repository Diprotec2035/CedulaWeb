using Datos.Comunes;
using Modelo;
using Modelo.v1.Modulo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Modulo
{
    public class ModuloDatos
    {
        public static List<ModuloModelo> GetReporteModulos(ref int operacion, string buscador, short? estado, uint? categoriaId, short orden, short orientacion)
        {
            var lista = new List<ModuloModelo>();
            var sql = @"SP_SEL_REPORTE_MODULOS";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(buscador) + "," +
                (estado.HasValue ? estado.Value.ToString() : "NULL") + "," +
                (categoriaId.HasValue ? categoriaId.Value.ToString() : "NULL") + "," +
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
                        cmd.Parameters.AddWithValue("P_MOD_ESTADO", estado.HasValue ? (object)estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAT_ID", categoriaId.HasValue ? (object)categoriaId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_CAMPO", orden);
                        cmd.Parameters.AddWithValue("P_ORDEN", orientacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearModuloMantenedor(reader));
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetReporteModulos), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static ModuloModelo GetModuloMantenedor(ref int operacion, uint moduloId)
        {
            var modulo = new ModuloModelo();

            var sql = @"SP_SEL_MODULO_MANTENEDOR";

            var consulta = sql + " " + moduloId;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID", moduloId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                modulo = MapearModuloMantenedor(reader);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModuloMantenedor), consulta);

                operacion = -1;
            }

            return modulo;
        }

        public static int InsertModulo(ModuloModelo modulo)
        {
            var retorno = 0;
            var sql = @"SP_INS_MODULO";

            var consulta = sql + " " +
                modulo.CategoriaId + "," +
                FuncionesDatos.TextoSql(modulo.Nombre) + "," +
                FuncionesDatos.TextoSql(modulo.TituloPagina) + "," +
                FuncionesDatos.TextoSql(modulo.TituloContenido) + "," +
                (modulo.Orden.HasValue ? modulo.Orden.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(modulo.Url) + "," +
                FuncionesDatos.TextoSql(modulo.UrlPublica) + "," +
                (modulo.Estado.HasValue ? modulo.Estado.Value.ToString() : "NULL") + "," +
                (modulo.Administrador.HasValue ? modulo.Administrador.Value.ToString() : "NULL") + "," +
                (modulo.MenuVisible.HasValue ? modulo.MenuVisible.Value.ToString() : "NULL") + "," +
                (modulo.BitacoraEstado.HasValue ? modulo.BitacoraEstado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(modulo.BitacoraSp) + "," +
                FuncionesDatos.TextoSql(modulo.BitacoraParametro) + "," +
                FuncionesDatos.TextoSql(modulo.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_CAT_ID", modulo.CategoriaId);
                        cmd.Parameters.AddWithValue("P_MOD_NOMBRE", modulo.Nombre);
                        cmd.Parameters.AddWithValue("P_MOD_TITULO_PAGINA", modulo.TituloPagina);
                        cmd.Parameters.AddWithValue("P_MOD_TITULO_CONTENIDO", modulo.TituloContenido);
                        cmd.Parameters.AddWithValue("P_MOD_ORDEN", modulo.Orden.HasValue ? (object)modulo.Orden.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_URL", modulo.Url);
                        cmd.Parameters.AddWithValue("P_MOD_URL_PUBLICA", string.IsNullOrWhiteSpace(modulo.UrlPublica) ? (object)DBNull.Value : modulo.UrlPublica);
                        cmd.Parameters.AddWithValue("P_MOD_ESTADO", modulo.Estado.HasValue ? (object)modulo.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_ADMINISTRADOR", modulo.Administrador.HasValue ? (object)modulo.Administrador.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_MENU_VISIBLE", modulo.MenuVisible.HasValue ? (object)modulo.MenuVisible.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_BITACORA_ESTADO", modulo.BitacoraEstado.HasValue ? (object)modulo.BitacoraEstado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_BITACORA_SP", string.IsNullOrWhiteSpace(modulo.BitacoraSp) ? (object)DBNull.Value : modulo.BitacoraSp);
                        cmd.Parameters.AddWithValue("P_MOD_BITACORA_PARAMETRO", string.IsNullOrWhiteSpace(modulo.BitacoraParametro) ? (object)DBNull.Value : modulo.BitacoraParametro);
                        cmd.Parameters.AddWithValue("P_USU_RUT", modulo.RutUsuario);
                        cmd.Parameters.Add("P_RETORNO", MySqlDbType.UByte).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToByte(cmd.Parameters["P_RETORNO"].Value ?? (byte)255);

                        if (retornoSp == 0)
                        {
                            retorno = 0;
                        }
                        else
                        {
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(InsertModulo), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(InsertModulo), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static int UpdateModulo(ModuloModelo modulo)
        {
            var retorno = 0;
            var sql = @"SP_UPD_MODULO";

            var consulta = sql + " " +
                modulo.Id + "," +
                modulo.CategoriaId + "," +
                FuncionesDatos.TextoSql(modulo.Nombre) + "," +
                FuncionesDatos.TextoSql(modulo.TituloPagina) + "," +
                FuncionesDatos.TextoSql(modulo.TituloContenido) + "," +
                (modulo.Orden.HasValue ? modulo.Orden.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(modulo.Url) + "," +
                FuncionesDatos.TextoSql(modulo.UrlPublica) + "," +
                (modulo.Estado.HasValue ? modulo.Estado.Value.ToString() : "NULL") + "," +
                (modulo.Administrador.HasValue ? modulo.Administrador.Value.ToString() : "NULL") + "," +
                (modulo.MenuVisible.HasValue ? modulo.MenuVisible.Value.ToString() : "NULL") + "," +
                (modulo.BitacoraEstado.HasValue ? modulo.BitacoraEstado.Value.ToString() : "NULL") + "," +
                FuncionesDatos.TextoSql(modulo.BitacoraSp) + "," +
                FuncionesDatos.TextoSql(modulo.BitacoraParametro) + "," +
                FuncionesDatos.TextoSql(modulo.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss.ffffff")) + "," +
                FuncionesDatos.TextoSql(modulo.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID", modulo.Id);
                        cmd.Parameters.AddWithValue("P_CAT_ID", modulo.CategoriaId);
                        cmd.Parameters.AddWithValue("P_MOD_NOMBRE", modulo.Nombre);
                        cmd.Parameters.AddWithValue("P_MOD_TITULO_PAGINA", modulo.TituloPagina);
                        cmd.Parameters.AddWithValue("P_MOD_TITULO_CONTENIDO", modulo.TituloContenido);
                        cmd.Parameters.AddWithValue("P_MOD_ORDEN", modulo.Orden.HasValue ? (object)modulo.Orden.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_URL", modulo.Url);
                        cmd.Parameters.AddWithValue("P_MOD_URL_PUBLICA", string.IsNullOrWhiteSpace(modulo.UrlPublica) ? (object)DBNull.Value : modulo.UrlPublica);
                        cmd.Parameters.AddWithValue("P_MOD_ESTADO", modulo.Estado.HasValue ? (object)modulo.Estado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_ADMINISTRADOR", modulo.Administrador.HasValue ? (object)modulo.Administrador.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_MENU_VISIBLE", modulo.MenuVisible.HasValue ? (object)modulo.MenuVisible.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_BITACORA_ESTADO", modulo.BitacoraEstado.HasValue ? (object)modulo.BitacoraEstado.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_BITACORA_SP", string.IsNullOrWhiteSpace(modulo.BitacoraSp) ? (object)DBNull.Value : modulo.BitacoraSp);
                        cmd.Parameters.AddWithValue("P_MOD_BITACORA_PARAMETRO", string.IsNullOrWhiteSpace(modulo.BitacoraParametro) ? (object)DBNull.Value : modulo.BitacoraParametro);
                        cmd.Parameters.AddWithValue("P_MOD_UPDATED_AT_ORIGINAL", modulo.UpdatedAt);
                        cmd.Parameters.AddWithValue("P_USU_RUT", modulo.RutUsuario);
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
                            FuncionesDatos.RegistrarErrorDatos(new Exception("El SP retornó un código no exitoso: " + retornoSp), nameof(UpdateModulo), consulta);

                            retorno = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(UpdateModulo), consulta);

                retorno = -1;
            }

            return retorno;
        }

        public static List<ModuloModelo> GetModuloUsuario(ref int operacion, EmpresaUsuarioModelo usuario)
        {
            var lista = new List<ModuloModelo>();
            var sql = @"SP_SEL_MODULO_USUARIO";

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
                                var fila = new ModuloModelo()
                                {
                                    Id = Convert.ToUInt32(reader["MOD_ID"]),
                                    Nombre = Convert.ToString(reader["MOD_NOMBRE"]),
                                    Url = Convert.ToString(reader["MOD_URL"]),
                                    UrlPublica = Convert.ToString(reader["MOD_URL_PUBLICA"]),
                                    Orden = Convert.ToSByte(reader["MOD_ORDEN"]),
                                    CategoriaId = Convert.ToUInt32(reader["CAT_ID"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModuloUsuario), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static List<ModuloModelo> GetModulosMenu(ref int operacion)
        {
            var lista = new List<ModuloModelo>();
            var sql = @"SP_SEL_MODULO_MENU";

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
                                var fila = new ModuloModelo()
                                {
                                    Id = Convert.ToUInt32(reader["MOD_ID"]),
                                    Nombre = Convert.ToString(reader["MOD_NOMBRE"]),
                                    Url = Convert.ToString(reader["MOD_URL"]),
                                    UrlPublica = Convert.ToString(reader["MOD_URL_PUBLICA"]),
                                    Orden = Convert.ToSByte(reader["MOD_ORDEN"]),
                                    Estado = Convert.ToSByte(reader["MOD_ESTADO"]),
                                    CategoriaId = Convert.ToUInt32(reader["CAT_ID"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModulosMenu), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static List<ModuloModelo> GetModulos(ref int operacion, int? visible)
        {
            var lista = new List<ModuloModelo>();
            var sql = @"SP_SEL_MODULO";

            var consulta = sql + " " +
                "NULL," +
                (visible.HasValue ? visible.Value.ToString() : "NULL");

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID", DBNull.Value);
                        cmd.Parameters.AddWithValue("P_MOD_MENU_VISIBLE", visible.HasValue ? (object)visible.Value : DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new ModuloModelo()
                                {
                                    Id = Convert.ToUInt32(reader["MOD_ID"]),
                                    Nombre = Convert.ToString(reader["MOD_NOMBRE"]),
                                    Url = Convert.ToString(reader["MOD_URL"]),
                                    UrlPublica = Convert.ToString(reader["MOD_URL_PUBLICA"]),
                                    Orden = Convert.ToSByte(reader["MOD_ORDEN"]),
                                    Estado = Convert.ToSByte(reader["MOD_ESTADO"]),
                                    BitacoraEstado = reader["MOD_BITACORA_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_BITACORA_ESTADO"]),
                                    BitacoraSp = reader["MOD_BITACORA_SP"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_BITACORA_SP"]),
                                    BitacoraParametro = reader["MOD_BITACORA_PARAMETRO"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_BITACORA_PARAMETRO"]),
                                    CategoriaId = Convert.ToUInt32(reader["CAT_ID"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModulos), consulta);

                operacion = -1;
            }

            return lista;
        }

        public static ModuloContextoModelo GetModuloContexto(ref int operacion, uint moduloId, string urlReal, string urlPublica)
        {
            var modulo = new ModuloContextoModelo();

            var sql = @"SP_SEL_MODULO_CONTEXTO";

            var consulta = sql + " " +
                moduloId + ", " +
                FuncionesDatos.TextoSql(urlReal) + ", " +
                FuncionesDatos.TextoSql(urlPublica);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID", moduloId);
                        cmd.Parameters.AddWithValue("P_MOD_URL_REAL", urlReal);
                        cmd.Parameters.AddWithValue("P_MOD_URL_PUBLICA", string.IsNullOrWhiteSpace(urlPublica) ? (object)DBNull.Value : urlPublica);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                modulo.ModuloId = Convert.ToUInt32(reader["MOD_ID"]);
                                modulo.Nombre = Convert.ToString(reader["MOD_NOMBRE"]);
                                modulo.TituloPagina = Convert.ToString(reader["MOD_TITULO_PAGINA"]);
                                modulo.TituloContenido = Convert.ToString(reader["MOD_TITULO_CONTENIDO"]);
                                modulo.UrlActual = Convert.ToString(reader["MOD_URL"]);
                                modulo.Estado = reader["MOD_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_ESTADO"]);
                                modulo.BitacoraEstado = reader["MOD_BITACORA_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_BITACORA_ESTADO"]);
                                modulo.BitacoraSp = Convert.ToString(reader["MOD_BITACORA_SP"]);
                                modulo.BitacoraParametro = Convert.ToString(reader["MOD_BITACORA_PARAMETRO"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModuloContexto), consulta);

                operacion = -1;
            }

            return modulo;
        }

        public static List<ModuloNavegacionModelo> GetModuloNavegacion(ref int operacion, string urlReal, string urlPublica)
        {
            var modulos = new List<ModuloNavegacionModelo>();

            var sql = @"SP_SEL_MODULO_NAVEGACION";

            var consulta = sql + " " +
                FuncionesDatos.TextoSql(urlReal) + ", " +
                FuncionesDatos.TextoSql(urlPublica);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_URL_REAL", string.IsNullOrWhiteSpace(urlReal) ? (object)DBNull.Value : urlReal);
                        cmd.Parameters.AddWithValue("P_MOD_URL_PUBLICA", string.IsNullOrWhiteSpace(urlPublica) ? (object)DBNull.Value : urlPublica);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                modulos.Add(new ModuloNavegacionModelo
                                {
                                    Id = Convert.ToUInt32(reader["MOD_ID"]),
                                    Url = Convert.ToString(reader["MOD_URL"]),
                                    UrlPublica = Convert.ToString(reader["MOD_URL_PUBLICA"]),
                                    Estado = reader["MOD_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_ESTADO"])
                                });
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModuloNavegacion), consulta);
                operacion = -1;
            }

            return modulos;
        }

        public static ModuloNavegacionAccionModelo GetModuloNavegacionAccion(ref int operacion, uint moduloIdOrigen, string accion)
        {
            var navegacion = new ModuloNavegacionAccionModelo();

            var sql = @"SP_SEL_MODULO_NAVEGACION_ACCION";

            var consulta = sql + " " +
                moduloIdOrigen + ", " +
                FuncionesDatos.TextoSql(accion);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID_ORIGEN", moduloIdOrigen);
                        cmd.Parameters.AddWithValue("P_MN_ACCION", accion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                navegacion.NavegacionId = Convert.ToUInt32(reader["MN_ID"]);
                                navegacion.ModuloIdOrigen = Convert.ToUInt32(reader["MOD_ID_ORIGEN"]);
                                navegacion.Accion = Convert.ToString(reader["MN_ACCION"]);
                                navegacion.ModuloIdDestino = Convert.ToUInt32(reader["MOD_ID_DESTINO"]);
                                navegacion.NavegacionEstado = reader["MN_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MN_ESTADO"]);
                                navegacion.ModuloId = Convert.ToUInt32(reader["MOD_ID"]);
                                navegacion.TituloPagina = Convert.ToString(reader["MOD_TITULO_PAGINA"]);
                                navegacion.Url = Convert.ToString(reader["MOD_URL"]);
                                navegacion.UrlPublica = Convert.ToString(reader["MOD_URL_PUBLICA"]);
                                navegacion.ModuloEstado = reader["MOD_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_ESTADO"]);
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModuloNavegacionAccion), consulta);
                operacion = -1;
            }

            return navegacion;
        }

        public static List<ModuloNavegacionModelo> GetModuloUrlDuplicada(ref int operacion, uint moduloId, string url, string urlPublica)
        {
            var modulos = new List<ModuloNavegacionModelo>();

            var sql = @"SP_SEL_MODULO_URL_DUPLICADA";

            var consulta = sql + " " +
                moduloId + ", " +
                FuncionesDatos.TextoSql(url) + ", " +
                FuncionesDatos.TextoSql(urlPublica);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID", moduloId);
                        cmd.Parameters.AddWithValue("P_MOD_URL", string.IsNullOrWhiteSpace(url) ? (object)DBNull.Value : url);
                        cmd.Parameters.AddWithValue("P_MOD_URL_PUBLICA", string.IsNullOrWhiteSpace(urlPublica) ? (object)DBNull.Value : urlPublica);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                modulos.Add(new ModuloNavegacionModelo
                                {
                                    Id = Convert.ToUInt32(reader["MOD_ID"]),
                                    Url = Convert.ToString(reader["MOD_URL"]),
                                    UrlPublica = Convert.ToString(reader["MOD_URL_PUBLICA"]),
                                    Estado = reader["MOD_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_ESTADO"])
                                });
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModuloUrlDuplicada), consulta);
                operacion = -1;
            }

            return modulos;
        }

        public static List<ModuloNavegacionConfigModelo> GetModuloNavegacionConfig(ref int operacion, uint moduloIdOrigen)
        {
            var navegaciones = new List<ModuloNavegacionConfigModelo>();

            var sql = @"SP_SEL_MODULO_NAVEGACION_CONFIG";

            var consulta = sql + " " + moduloIdOrigen;

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID_ORIGEN", moduloIdOrigen);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                navegaciones.Add(new ModuloNavegacionConfigModelo
                                {
                                    Id = Convert.ToUInt32(reader["MN_ID"]),

                                    ModuloIdOrigen = Convert.ToUInt32(reader["MOD_ID_ORIGEN"]),
                                    ModuloNombreOrigen = Convert.ToString(reader["MOD_NOMBRE_ORIGEN"]),

                                    Accion = Convert.ToString(reader["MN_ACCION"]),

                                    ModuloIdDestino = Convert.ToUInt32(reader["MOD_ID_DESTINO"]),
                                    ModuloNombreDestino = Convert.ToString(reader["MOD_NOMBRE_DESTINO"]),
                                    ModuloUrlDestino = Convert.ToString(reader["MOD_URL_DESTINO"]),
                                    ModuloUrlPublicaDestino = Convert.ToString(reader["MOD_URL_PUBLICA_DESTINO"]),

                                    Estado = reader["MN_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MN_ESTADO"]),
                                    EstadoNombre = Convert.ToString(reader["MN_ESTADO_NOMBRE"])
                                });
                            }
                        }

                        operacion = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetModuloNavegacionConfig), consulta);
                operacion = -1;
            }

            return navegaciones;
        }

        public static int RegistrarModuloNavegacion(ModuloNavegacionConfigModelo navegacion)
        {
            var sql = @"SP_INS_MODULO_NAVEGACION";

            var consulta = sql + " " +
                navegacion.ModuloIdOrigen + ", " +
                FuncionesDatos.TextoSql(navegacion.Accion) + ", " +
                navegacion.ModuloIdDestino + ", " +
                navegacion.Estado + ", " +
                FuncionesDatos.TextoSql(navegacion.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MOD_ID_ORIGEN", navegacion.ModuloIdOrigen);
                        cmd.Parameters.AddWithValue("P_MN_ACCION", navegacion.Accion);
                        cmd.Parameters.AddWithValue("P_MOD_ID_DESTINO", navegacion.ModuloIdDestino);
                        cmd.Parameters.AddWithValue("P_MN_ESTADO", navegacion.Estado);
                        cmd.Parameters.AddWithValue("P_USU_RUT", navegacion.RutUsuario);

                        var retorno = cmd.CreateParameter();
                        retorno.ParameterName = "P_RETORNO";
                        retorno.Direction = ParameterDirection.Output;
                        retorno.DbType = DbType.Byte;
                        cmd.Parameters.Add(retorno);

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToInt32(retorno.Value);

                        if (retornoSp == 0)
                        {
                            return 0;
                        }

                        FuncionesDatos.RegistrarErrorDatos(
                            new Exception("El SP retornó un código no exitoso: " + retornoSp),
                            nameof(RegistrarModuloNavegacion),
                            consulta
                        );

                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(RegistrarModuloNavegacion), consulta);
                return -1;
            }
        }

        public static int ActualizarModuloNavegacion(ModuloNavegacionConfigModelo navegacion)
        {
            var sql = @"SP_UPD_MODULO_NAVEGACION";

            var consulta = sql + " " +
                navegacion.Id + ", " +
                navegacion.ModuloIdOrigen + ", " +
                FuncionesDatos.TextoSql(navegacion.Accion) + ", " +
                navegacion.ModuloIdDestino + ", " +
                navegacion.Estado + ", " +
                FuncionesDatos.TextoSql(navegacion.RutUsuario);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("P_MN_ID", navegacion.Id);
                        cmd.Parameters.AddWithValue("P_MOD_ID_ORIGEN", navegacion.ModuloIdOrigen);
                        cmd.Parameters.AddWithValue("P_MN_ACCION", navegacion.Accion);
                        cmd.Parameters.AddWithValue("P_MOD_ID_DESTINO", navegacion.ModuloIdDestino);
                        cmd.Parameters.AddWithValue("P_MN_ESTADO", navegacion.Estado);
                        cmd.Parameters.AddWithValue("P_USU_RUT", navegacion.RutUsuario);

                        var retorno = cmd.CreateParameter();
                        retorno.ParameterName = "P_RETORNO";
                        retorno.Direction = ParameterDirection.Output;
                        retorno.DbType = DbType.Byte;
                        cmd.Parameters.Add(retorno);

                        cmd.ExecuteNonQuery();

                        var retornoSp = Convert.ToInt32(retorno.Value);

                        if (retornoSp == 0)
                        {
                            return 0;
                        }

                        FuncionesDatos.RegistrarErrorDatos(
                            new Exception("El SP retornó un código no exitoso: " + retornoSp),
                            nameof(ActualizarModuloNavegacion),
                            consulta
                        );

                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(ActualizarModuloNavegacion), consulta);
                return -1;
            }
        }

        private static ModuloModelo MapearModuloMantenedor(IDataRecord reader)
        {
            return new ModuloModelo
            {
                Id = Convert.ToUInt32(reader["MOD_ID"]),
                Nombre = Convert.ToString(reader["MOD_NOMBRE"]),
                TituloPagina = reader["MOD_TITULO_PAGINA"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_TITULO_PAGINA"]),
                TituloContenido = reader["MOD_TITULO_CONTENIDO"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_TITULO_CONTENIDO"]),
                Orden = Convert.ToSByte(reader["MOD_ORDEN"]),
                Url = Convert.ToString(reader["MOD_URL"]),
                UrlPublica = reader["MOD_URL_PUBLICA"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_URL_PUBLICA"]),
                Estado = Convert.ToSByte(reader["MOD_ESTADO"]),
                EstadoNombre = Convert.ToString(reader["MOD_ESTADO_NOMBRE"]),
                Administrador = Convert.ToSByte(reader["MOD_ADMINISTRADOR"]),
                AdministradorNombre = Convert.ToString(reader["MOD_ADMINISTRADOR_NOMBRE"]),
                MenuVisible = reader["MOD_MENU_VISIBLE"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_MENU_VISIBLE"]),
                MenuVisibleNombre = Convert.ToString(reader["MOD_MENU_VISIBLE_NOMBRE"]),
                BitacoraEstado = reader["MOD_BITACORA_ESTADO"] == DBNull.Value ? (sbyte?)null : Convert.ToSByte(reader["MOD_BITACORA_ESTADO"]),
                BitacoraEstadoNombre = reader["MOD_BITACORA_ESTADO_NOMBRE"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_BITACORA_ESTADO_NOMBRE"]),
                BitacoraSp = reader["MOD_BITACORA_SP"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_BITACORA_SP"]),
                BitacoraParametro = reader["MOD_BITACORA_PARAMETRO"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MOD_BITACORA_PARAMETRO"]),
                CategoriaId = Convert.ToUInt32(reader["CAT_ID"]),
                CategoriaNombre = Convert.ToString(reader["CAT_NOMBRE"]),
                UpdatedAt = Convert.ToDateTime(reader["MOD_UPDATED_AT"])
            };
        }
    }
}