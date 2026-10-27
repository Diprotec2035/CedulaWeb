using Modelo;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.v1.MFA
{
    public static class MFATotpHelperNegocio
    {
        public static byte[] GenerarSecreto()
        {
            var secreto = new byte[ConstantesModelo.ConsMfaTotpLargoSecretoTotp];

            using (var generador = RandomNumberGenerator.Create())
            {
                generador.GetBytes(secreto);
            }

            return secreto;
        }

        public static List<string> GenerarCodigosRecuperacion()
        {
            var codigos = new HashSet<string>(StringComparer.Ordinal);
            var caracteres = ConstantesModelo.ConsMfaTotpCodigoRecuperacionCaracteres;

            if (string.IsNullOrWhiteSpace(caracteres) || caracteres.Length != ConstantesModelo.ConsMfaTotpCodigoRecuperacionCaracteresCantidad)
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorMfaTotpCodigoRecuperacionCaracteres);
            }

            using (var generador = RandomNumberGenerator.Create())
            {
                while (codigos.Count < ConstantesModelo.ConsMfaTotpCodigoRecuperacionCantidad)
                {
                    var bytes = new byte[ConstantesModelo.ConsMfaTotpCodigoRecuperacionLargo];
                    var caracteresCodigo = new char[ConstantesModelo.ConsMfaTotpCodigoRecuperacionLargo];

                    try
                    {
                        generador.GetBytes(bytes);

                        for (var indice = 0; indice < caracteresCodigo.Length; indice++)
                        {
                            caracteresCodigo[indice] = caracteres[bytes[indice] & (caracteres.Length - 1)];
                        }

                        var codigoPlano = new string(caracteresCodigo);
                        var codigoFormateado = codigoPlano.Substring(0, ConstantesModelo.ConsMfaTotpCodigoRecuperacionBloqueLargo) + ConstantesModelo.ConsMfaTotpCodigoRecuperacionSeparador + codigoPlano.Substring(ConstantesModelo.ConsMfaTotpCodigoRecuperacionBloqueLargo);

                        codigos.Add(codigoFormateado);
                    }
                    finally
                    {
                        Array.Clear(bytes, 0, bytes.Length);
                        Array.Clear(caracteresCodigo, 0, caracteresCodigo.Length);
                    }
                }
            }

            return new List<string>(codigos);
        }

        public static string NormalizarCodigoRecuperacion(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return string.Empty;
            }

            return codigo.Trim().ToUpperInvariant().Replace(ConstantesModelo.ConsMfaTotpCodigoRecuperacionSeparador, string.Empty).Replace(ConstantesModelo.ConsMfaTotpCodigoRecuperacionEspacio, string.Empty);
        }

        public static string CalcularHashCodigoRecuperacion(string codigo)
        {
            var codigoNormalizado = NormalizarCodigoRecuperacion(codigo);

            if (codigoNormalizado.Length != ConstantesModelo.ConsMfaTotpCodigoRecuperacionLargo)
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpCodigoRecuperacionFormatoInvalido, nameof(codigo));
            }

            foreach (var caracter in codigoNormalizado)
            {
                if (ConstantesModelo.ConsMfaTotpCodigoRecuperacionCaracteres.IndexOf(caracter) < 0)
                {
                    throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpCodigoRecuperacionFormatoInvalido, nameof(codigo));
                }
            }

            byte[] codigoBytes = null;
            byte[] hash = null;

            try
            {
                codigoBytes = Encoding.UTF8.GetBytes(codigoNormalizado);

                using (var sha256 = SHA256.Create())
                {
                    hash = sha256.ComputeHash(codigoBytes);
                }

                var hashHex = BitConverter.ToString(hash).Replace(ConstantesModelo.ConsMfaTotpCodigoRecuperacionSeparador, string.Empty);

                if (hashHex.Length != ConstantesModelo.ConsMfaTotpCodigoRecuperacionHashHexLargo)
                {
                    throw new CryptographicException(ConstantesModelo.ConsErrorMfaTotpCodigoRecuperacionHash);
                }

                return hashHex;
            }
            finally
            {
                Limpiar(codigoBytes);
                Limpiar(hash);
            }
        }

        public static string ConvertirSecretoBase32(byte[] secreto)
        {
            if (secreto == null || secreto.Length == 0)
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpSecretoVacio, nameof(secreto));
            }

            return Base32Encoding.ToString(secreto);
        }

        public static byte[] ProtegerSecreto(byte[] secreto)
        {
            if (secreto == null || secreto.Length == 0)
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpSecretoVacio, nameof(secreto));
            }

            byte[] claveMaestra = null;
            byte[] claveCifrado = null;
            byte[] claveIntegridad = null;
            byte[] iv = null;
            byte[] secretoCifrado = null;
            byte[] contenidoProtegido = null;
            byte[] hashIntegridad = null;
            byte[] resultado = null;

            try
            {
                claveMaestra = ObtenerClaveMaestra();
                claveCifrado = new byte[ConstantesModelo.ConsMfaTotpLargoClaveCifrado];
                claveIntegridad = new byte[ConstantesModelo.ConsMfaTotpLargoClaveIntegridad];

                Buffer.BlockCopy(claveMaestra, 0, claveCifrado, 0, ConstantesModelo.ConsMfaTotpLargoClaveCifrado);
                Buffer.BlockCopy(claveMaestra, ConstantesModelo.ConsMfaTotpLargoClaveCifrado, claveIntegridad, 0, ConstantesModelo.ConsMfaTotpLargoClaveIntegridad);

                using (var aes = Aes.Create())
                {
                    aes.KeySize = ConstantesModelo.ConsMfaTotpAesKeySizeBits;
                    aes.BlockSize = ConstantesModelo.ConsMfaTotpAesBlockSizeBits;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = claveCifrado;
                    aes.GenerateIV();

                    iv = aes.IV;

                    using (var cifrador = aes.CreateEncryptor())
                    {
                        secretoCifrado = cifrador.TransformFinalBlock(secreto, 0, secreto.Length);
                    }
                }

                contenidoProtegido = new byte[1 + ConstantesModelo.ConsMfaTotpLargoIv + secretoCifrado.Length];
                contenidoProtegido[0] = ConstantesModelo.ConsMfaTotpVersionProteccion;

                Buffer.BlockCopy(iv, 0, contenidoProtegido, 1, ConstantesModelo.ConsMfaTotpLargoIv);
                Buffer.BlockCopy(secretoCifrado, 0, contenidoProtegido, 1 + ConstantesModelo.ConsMfaTotpLargoIv, secretoCifrado.Length);

                using (var hmac = new HMACSHA256(claveIntegridad))
                {
                    hashIntegridad = hmac.ComputeHash(contenidoProtegido);
                }

                resultado = new byte[contenidoProtegido.Length + hashIntegridad.Length];

                Buffer.BlockCopy(contenidoProtegido, 0, resultado, 0, contenidoProtegido.Length);
                Buffer.BlockCopy(hashIntegridad, 0, resultado, contenidoProtegido.Length, hashIntegridad.Length);

                return resultado;
            }
            finally
            {
                Limpiar(claveMaestra);
                Limpiar(claveCifrado);
                Limpiar(claveIntegridad);
                Limpiar(iv);
                Limpiar(secretoCifrado);
                Limpiar(contenidoProtegido);
                Limpiar(hashIntegridad);
            }
        }

        public static byte[] DesprotegerSecreto(byte[] secretoProtegido)
        {
            if (secretoProtegido == null || secretoProtegido.Length < 1 + ConstantesModelo.ConsMfaTotpLargoIv + ConstantesModelo.ConsMfaTotpLargoBloqueCifradoMinimo + ConstantesModelo.ConsMfaTotpLargoHashIntegridad)
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpSecretoProtegidoInvalido, nameof(secretoProtegido));
            }

            if (secretoProtegido[0] != ConstantesModelo.ConsMfaTotpVersionProteccion)
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorMfaTotpVersionProteccionNoCompatible);
            }

            byte[] claveMaestra = null;
            byte[] claveCifrado = null;
            byte[] claveIntegridad = null;
            byte[] contenidoProtegido = null;
            byte[] hashAlmacenado = null;
            byte[] hashCalculado = null;
            byte[] iv = null;
            byte[] secretoCifrado = null;

            try
            {
                claveMaestra = ObtenerClaveMaestra();
                claveCifrado = new byte[ConstantesModelo.ConsMfaTotpLargoClaveCifrado];
                claveIntegridad = new byte[ConstantesModelo.ConsMfaTotpLargoClaveIntegridad];

                Buffer.BlockCopy(claveMaestra, 0, claveCifrado, 0, ConstantesModelo.ConsMfaTotpLargoClaveCifrado);
                Buffer.BlockCopy(claveMaestra, ConstantesModelo.ConsMfaTotpLargoClaveCifrado, claveIntegridad, 0, ConstantesModelo.ConsMfaTotpLargoClaveIntegridad);

                var largoContenidoProtegido = secretoProtegido.Length - ConstantesModelo.ConsMfaTotpLargoHashIntegridad;
                contenidoProtegido = new byte[largoContenidoProtegido];
                hashAlmacenado = new byte[ConstantesModelo.ConsMfaTotpLargoHashIntegridad];

                Buffer.BlockCopy(secretoProtegido, 0, contenidoProtegido, 0, largoContenidoProtegido);
                Buffer.BlockCopy(secretoProtegido, largoContenidoProtegido, hashAlmacenado, 0, ConstantesModelo.ConsMfaTotpLargoHashIntegridad);

                using (var hmac = new HMACSHA256(claveIntegridad))
                {
                    hashCalculado = hmac.ComputeHash(contenidoProtegido);
                }

                if (!SonIgualesTiempoConstante(hashAlmacenado, hashCalculado))
                {
                    throw new CryptographicException(ConstantesModelo.ConsErrorMfaTotpIntegridadInvalida);
                }

                iv = new byte[ConstantesModelo.ConsMfaTotpLargoIv];
                Buffer.BlockCopy(contenidoProtegido, 1, iv, 0, ConstantesModelo.ConsMfaTotpLargoIv);

                var largoSecretoCifrado = contenidoProtegido.Length - 1 - ConstantesModelo.ConsMfaTotpLargoIv;
                secretoCifrado = new byte[largoSecretoCifrado];

                Buffer.BlockCopy(contenidoProtegido, 1 + ConstantesModelo.ConsMfaTotpLargoIv, secretoCifrado, 0, largoSecretoCifrado);

                using (var aes = Aes.Create())
                {
                    aes.KeySize = ConstantesModelo.ConsMfaTotpAesKeySizeBits;
                    aes.BlockSize = ConstantesModelo.ConsMfaTotpAesBlockSizeBits;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = claveCifrado;
                    aes.IV = iv;

                    using (var descifrador = aes.CreateDecryptor())
                    {
                        return descifrador.TransformFinalBlock(secretoCifrado, 0, secretoCifrado.Length);
                    }
                }
            }
            finally
            {
                Limpiar(claveMaestra);
                Limpiar(claveCifrado);
                Limpiar(claveIntegridad);
                Limpiar(contenidoProtegido);
                Limpiar(hashAlmacenado);
                Limpiar(hashCalculado);
                Limpiar(iv);
                Limpiar(secretoCifrado);
            }
        }

        public static string CrearUriOtpAuth(string emisor, string cuenta, string secretoBase32, string algoritmo, byte digitos, ushort periodoSegundos)
        {
            if (string.IsNullOrWhiteSpace(emisor))
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpEmisorVacio, nameof(emisor));
            }

            if (string.IsNullOrWhiteSpace(cuenta))
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpCuentaVacia, nameof(cuenta));
            }

            if (string.IsNullOrWhiteSpace(secretoBase32))
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionMfaTotpSecretoBase32Vacio, nameof(secretoBase32));
            }

            var emisorNormalizado = emisor.Trim();
            var cuentaNormalizada = cuenta.Trim();
            var etiqueta = Uri.EscapeDataString(emisorNormalizado + ConstantesModelo.ConsMfaTotpUriSeparadorEtiqueta + cuentaNormalizada);

            return ConstantesModelo.ConsMfaTotpUriOtpAuthPrefijo + etiqueta + ConstantesModelo.ConsMfaTotpUriInicioParametros + ConstantesModelo.ConsMfaTotpUriParametroSecret + ConstantesModelo.ConsMfaTotpUriSeparadorValor + Uri.EscapeDataString(secretoBase32) + ConstantesModelo.ConsMfaTotpUriSeparadorParametros + ConstantesModelo.ConsMfaTotpUriParametroIssuer + ConstantesModelo.ConsMfaTotpUriSeparadorValor + Uri.EscapeDataString(emisorNormalizado) + ConstantesModelo.ConsMfaTotpUriSeparadorParametros + ConstantesModelo.ConsMfaTotpUriParametroAlgorithm + ConstantesModelo.ConsMfaTotpUriSeparadorValor + Uri.EscapeDataString(algoritmo.ToUpperInvariant()) + ConstantesModelo.ConsMfaTotpUriSeparadorParametros + ConstantesModelo.ConsMfaTotpUriParametroDigits + ConstantesModelo.ConsMfaTotpUriSeparadorValor + digitos + ConstantesModelo.ConsMfaTotpUriSeparadorParametros + ConstantesModelo.ConsMfaTotpUriParametroPeriod + ConstantesModelo.ConsMfaTotpUriSeparadorValor + periodoSegundos;
        }

        public static bool ValidarCodigo(byte[] secreto, string codigo, ushort periodoSegundos, byte digitos, out long pasoUsado)
        {
            pasoUsado = ConstantesModelo.ConsMfaTotpPasoTotpInicial;

            if (secreto == null || secreto.Length == 0 || string.IsNullOrWhiteSpace(codigo))
            {
                return false;
            }

            var codigoNormalizado = codigo.Trim();

            if (codigoNormalizado.Length != digitos)
            {
                return false;
            }

            foreach (var caracter in codigoNormalizado)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }

            var totp = new Totp(secreto, periodoSegundos, OtpHashMode.Sha1, digitos);

            return totp.VerifyTotp(codigoNormalizado, out pasoUsado, VerificationWindow.RfcSpecifiedNetworkDelay);
        }

        private static byte[] ObtenerClaveMaestra()
        {
            var valor = ConstantesModelo.ConsMfaTotpClaveCifrado;

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorMfaTotpVariableClaveNoConfigurada);
            }

            byte[] clave;

            try
            {
                clave = Convert.FromBase64String(valor.Trim());
            }
            catch
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorMfaTotpClaveBase64Invalida);
            }

            if (clave.Length != ConstantesModelo.ConsMfaTotpLargoClaveMaestra)
            {
                Limpiar(clave);
                throw new InvalidOperationException(ConstantesModelo.ConsErrorMfaTotpClaveLargoInvalido);
            }

            return clave;
        }

        private static bool SonIgualesTiempoConstante(byte[] valorA, byte[] valorB)
        {
            if (valorA == null || valorB == null || valorA.Length != valorB.Length)
            {
                return false;
            }

            var diferencia = ConstantesModelo.ConsMfaTotpValorInicialDiferencia;

            for (var indice = 0; indice < valorA.Length; indice++)
            {
                diferencia |= valorA[indice] ^ valorB[indice];
            }

            return diferencia == ConstantesModelo.ConsMfaTotpValorInicialDiferencia;
        }

        private static void Limpiar(byte[] valor)
        {
            if (valor != null)
            {
                Array.Clear(valor, 0, valor.Length);
            }
        }
    }
}