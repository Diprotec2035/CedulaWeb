using System;
using System.Security.Cryptography;
using System.Text;
using Modelo;

namespace Negocio
{
    public class PasswordHelperNegocio
    {
        private const int DefaultSaltSizeBytes = 16;      // 128 bits
        private const int DefaultHashSizeBytes = 32;      // 256 bits
        private const int DefaultIterations = 100000;

        static readonly int SaltSizeBytes = ParseIntFromConfig(ConstantesModelo.ConsPasswordSalt, DefaultSaltSizeBytes, "EV-Cedula-Password-Salt");
        static readonly int HashSizeBytes = ParseIntFromConfig(ConstantesModelo.ConsPasswordHash, DefaultHashSizeBytes, "EV-Cedula-Password-Hash");
        static readonly int Iterations = ParseIntFromConfig(ConstantesModelo.ConsPasswordIterations, DefaultIterations, "EV-Cedula-Password-Iterations");
        static readonly string AlgoritmoId = string.IsNullOrWhiteSpace(ConstantesModelo.ConsPasswordAlgoritmoId) ? $"PBKDF2-SHA256-{Iterations}" : ConstantesModelo.ConsPasswordAlgoritmoId;

        private static int ParseIntFromConfig(string rawValue, int defaultValue, string envName)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return defaultValue;
            }

            if (!int.TryParse(rawValue, out var parsed) || parsed <= 0)
            {
                return defaultValue;
            }

            return parsed;
        }

        public static (string hashHex, string saltHex, string algoritmo) GenerarPasswordUsuario(string passwordPlano)
        {
            if (string.IsNullOrWhiteSpace(passwordPlano))
            {
                throw new ArgumentException("Password vacío.", nameof(passwordPlano));
            }

            byte[] salt = new byte[SaltSizeBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var deriveBytes = new Rfc2898DeriveBytes(passwordPlano, salt, Iterations, HashAlgorithmName.SHA256))
            {
                hash = deriveBytes.GetBytes(HashSizeBytes);
            }

            string hashHex = BitConverter.ToString(hash).Replace("-", string.Empty);
            string saltHex = BitConverter.ToString(salt).Replace("-", string.Empty);

            return (hashHex, saltHex, AlgoritmoId);
        }

        public static bool VerificarPassword(string passwordIngresado, string passwordHashHex, string passwordSaltHex, string passwordAlgoritmo)
        {
            var partes = passwordAlgoritmo.Split('-');
            if (partes.Length == 3)
            {
                var algoritmo = partes[0] + '-' + partes[1];
                var iteraciones = int.Parse(partes[2]);

                if (algoritmo != "PBKDF2-SHA256")
                {
                    throw new NotSupportedException($"Algoritmo no soportado: {algoritmo}");
                }

                var saltBytes = HexToBytes(passwordSaltHex);
                var hashAlmacenado = HexToBytes(passwordHashHex);

                using (var pbkdf2 = new Rfc2898DeriveBytes(passwordIngresado, saltBytes, iteraciones, HashAlgorithmName.SHA256))
                {
                    var hashCalculado = pbkdf2.GetBytes(hashAlmacenado.Length);
                    return CompararBytesConstante(hashAlmacenado, hashCalculado);
                }
            }
            else
            {
                return false;
            }
        }

        private static bool CompararBytesConstante(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;

            var diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        private static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex inválido, longitud impar.", nameof(hex));

            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        public static string CalculateHash(string activationCode)
        {
            if (string.IsNullOrEmpty(activationCode))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(activationCode);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                var sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                return sb.ToString();
            }
        }
    }
}
