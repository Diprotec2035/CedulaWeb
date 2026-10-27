using MySql.Data.MySqlClient;
using Modelo;

namespace Datos
{
    public static class ConexionDatos
    {
        private static string BuildConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = ConstantesModelo.ConsServidor,
                Database = ConstantesModelo.ConsBaseDatos,
                UserID = ConstantesModelo.ConsUsuario,
                Password = ConstantesModelo.ConsPassword,
            };

            return builder.ConnectionString;
        }

        public static MySqlConnection CrearConexionAbierta()
        {
            var connectionString = BuildConnectionString();
            var conexion = new MySqlConnection(connectionString);

            conexion.Open();
            return conexion;
        }
    }
}