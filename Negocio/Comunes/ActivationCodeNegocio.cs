using System;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.Comunes
{
    public class ActivationCodeNegocio
    {
        public sealed class ActivationCodeResult
        {
            public string Code { get; private set; }
            public string Hash { get; private set; }

            public ActivationCodeResult(string code, string hash)
            {
                if (string.IsNullOrEmpty(code))
                    throw new ArgumentNullException("code");

                if (string.IsNullOrEmpty(hash))
                    throw new ArgumentNullException("hash");

                Code = code;
                Hash = hash;
            }
        }

        public static class ActivationCodeService
        {
            private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            public static ActivationCodeResult Generate(int length = 12, int groupSize = 4, char separator = '-')
            {
                if (length <= 0)
                    throw new ArgumentOutOfRangeException("length", "La longitud debe ser mayor a cero.");

                if (groupSize <= 0)
                    throw new ArgumentOutOfRangeException("groupSize", "El tamaño de grupo debe ser mayor a cero.");

                string rawCode = GenerateRawCode(length);
                string formattedCode = FormatCode(rawCode, groupSize, separator);
                string hash = CalculateHash(formattedCode);

                return new ActivationCodeResult(formattedCode, hash);
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

            public static bool Verify(string activationCode, string expectedHash)
            {
                if (string.IsNullOrEmpty(activationCode) || string.IsNullOrEmpty(expectedHash))
                    return false;

                string computedHash = CalculateHash(activationCode);
                return string.Equals(computedHash, expectedHash, StringComparison.OrdinalIgnoreCase);
            }

            private static string GenerateRawCode(int length)
            {
                var chars = new char[length];
                var buffer = new byte[1];

                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % AllowedChars.Length);

                using (var rng = new RNGCryptoServiceProvider())
                {
                    for (int i = 0; i < length; i++)
                    {
                        byte value;

                        do
                        {
                            rng.GetBytes(buffer);
                            value = buffer[0];
                        }
                        while (value > maxRandom);

                        chars[i] = AllowedChars[value % AllowedChars.Length];
                    }
                }

                return new string(chars);
            }

            private static string FormatCode(string rawCode, int groupSize, char separator)
            {
                if (string.IsNullOrEmpty(rawCode))
                    throw new ArgumentException("El código no puede ser nulo o vacío.", "rawCode");

                if (groupSize >= rawCode.Length)
                    return rawCode;

                var sb = new StringBuilder(rawCode.Length + (rawCode.Length / groupSize));

                for (int i = 0; i < rawCode.Length; i++)
                {
                    if (i > 0 && i % groupSize == 0)
                        sb.Append(separator);

                    sb.Append(rawCode[i]);
                }

                return sb.ToString();
            }
        }
    }
}
