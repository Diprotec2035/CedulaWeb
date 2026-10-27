using Datos.Comunes;
using Modelo.v1.Bitacora;
using System;
using System.Collections.Generic;
using System.Data;

namespace Datos.v1.Bitacora
{
    public class BitacoraDatos
    {
        public static List<BitacoraModelo> GetBitacora(ref int operacion, string storedProcedure, string parametro, string idRegistro)
        {
            var lista = new List<BitacoraModelo>();

            operacion = -1;

            var sql = Convert.ToString(storedProcedure).Trim();
            var parametros = Convert.ToString(parametro).Trim();
            var valores = Convert.ToString(idRegistro).Trim();

            if (string.IsNullOrWhiteSpace(sql) || string.IsNullOrWhiteSpace(parametros) || string.IsNullOrWhiteSpace(valores))
            {
                return lista;
            }

            if (!EsNombreSqlValido(sql) || !SonNombresSqlValidos(parametros))
            {
                return lista;
            }

            var nombresParametros = parametros.Split(new[] { '|' }, StringSplitOptions.None);
            var valoresParametros = valores.Split(new[] { '|' }, StringSplitOptions.None);

            if (nombresParametros.Length == 0 || nombresParametros.Length != valoresParametros.Length)
            {
                return lista;
            }

            for (var indice = 0; indice < nombresParametros.Length; indice++)
            {
                nombresParametros[indice] = Convert.ToString(nombresParametros[indice]).Trim();
                valoresParametros[indice] = Convert.ToString(valoresParametros[indice]).Trim();

                if (!EsNombreSqlValido(nombresParametros[indice]) || string.IsNullOrWhiteSpace(valoresParametros[indice]))
                {
                    return lista;
                }
            }

            var consulta = ConstruirConsulta(sql, nombresParametros, valoresParametros);

            try
            {
                using (var conn = ConexionDatos.CrearConexionAbierta())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Clear();

                        for (var indice = 0; indice < nombresParametros.Length; indice++)
                        {
                            cmd.Parameters.AddWithValue(nombresParametros[indice], valoresParametros[indice]);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fila = new BitacoraModelo
                                {
                                    Id = Convert.ToUInt32(reader["BIT_ID"]),
                                    Fecha = Convert.ToString(reader["BIT_FECHA"]),
                                    Hora = Convert.ToString(reader["BIT_HORA"]),
                                    RutUsuario = Convert.ToString(reader["USU_RUT"]),
                                    NombreUsuario = Convert.ToString(reader["USU_NOMBRE"]),
                                    Accion = Convert.ToString(reader["BIT_ACCION"]),
                                    Comentarios = Convert.ToString(reader["BIT_COMENTARIOS"]),
                                    Detalle = Convert.ToString(reader["BIT_DETALLE"]),
                                    IdRegistro = Convert.ToString(reader["BIT_REGISTRO_ID"])
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
                FuncionesDatos.RegistrarErrorDatos(ex, nameof(GetBitacora), consulta);

                operacion = -1;
            }

            return lista;
        }

        private static bool EsNombreSqlValido(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            foreach (var caracter in valor)
            {
                if (!char.IsLetterOrDigit(caracter) && caracter != '_')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SonNombresSqlValidos(string parametros)
        {
            if (string.IsNullOrWhiteSpace(parametros))
            {
                return false;
            }

            var nombres = parametros.Split(
                new[] { '|' },
                StringSplitOptions.None
            );

            if (nombres.Length == 0)
            {
                return false;
            }

            foreach (var nombre in nombres)
            {
                if (!EsNombreSqlValido(Convert.ToString(nombre).Trim()))
                {
                    return false;
                }
            }

            return true;
        }

        private static string ConstruirConsulta(string storedProcedure, string[] nombresParametros, string[] valoresParametros)
        {
            var consulta = storedProcedure;

            for (var indice = 0; indice < nombresParametros.Length; indice++)
            {
                consulta += indice == 0
                    ? " "
                    : ", ";

                consulta +=
                    nombresParametros[indice] +
                    "=" +
                    FuncionesDatos.TextoSql(valoresParametros[indice]);
            }

            return consulta;
        }
    }
}