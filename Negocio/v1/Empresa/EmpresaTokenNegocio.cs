using Datos.v1.Empresa;
using Microsoft.IdentityModel.Tokens;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Empresa;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.v1.Empresa
{
    public class EmpresaTokenNegocio
    {
        public static RespuestaModelo<EmpresaTokenGeneradoModelo> GenerarEmpresaToken(EmpresaTokenModelo empresaToken)
        {
            return EmitirEmpresaToken(empresaToken, false);
        }

        public static RespuestaModelo<EmpresaTokenGeneradoModelo> RenovarEmpresaToken(EmpresaTokenModelo empresaToken)
        {
            return EmitirEmpresaToken(empresaToken, true);
        }

        public static RespuestaModelo<EmpresaTokenModelo> RevocarEmpresaToken(EmpresaTokenModelo empresaToken)
        {
            var respuesta = new RespuestaModelo<EmpresaTokenModelo>();

            if (empresaToken == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaTokenNoIdentificado;

                return respuesta;
            }

            if (empresaToken.Id == 0)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaTokenNoIdentificado;

                return respuesta;
            }

            if (!TryNormalizarRutEmpresa(empresaToken.RutEmpresa, out var rutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutEmpresa;

                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresaToken.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaTokenUsuarioNoIdentificado;

                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresaToken.RevocationReason))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaTokenMotivoRevocacionVacio;

                return respuesta;
            }

            empresaToken.RevocationReason = empresaToken.RevocationReason.Trim();

            if (empresaToken.RevocationReason.Length > 500)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaTokenMotivoRevocacionLargo;

                return respuesta;
            }

            empresaToken.RutEmpresa = rutEmpresa;
            empresaToken.RutUsuario = empresaToken.RutUsuario.Trim().ToUpperInvariant();

            var resultado = EmpresaTokenDatos.UpdateEmpresaTokenRevocar(empresaToken);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = empresaToken;
                    respuesta.Mensaje = ConstantesModelo.ConsExitoEmpresaTokenRevocar;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaTokenRevocar;
                    return respuesta;
            }
        }

        private static RespuestaModelo<EmpresaTokenGeneradoModelo> EmitirEmpresaToken(EmpresaTokenModelo empresaToken, bool esRenovacion)
        {
            var respuesta = new RespuestaModelo<EmpresaTokenGeneradoModelo>();

            if (empresaToken == null)
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaTokenEmpresaNoIdentificada;

                return respuesta;
            }

            if (!TryNormalizarRutEmpresa(empresaToken.RutEmpresa, out var rutEmpresa))
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorRutEmpresa;

                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(empresaToken.RutUsuario))
            {
                respuesta.Mensaje = ConstantesModelo.ConsValidacionEmpresaTokenUsuarioNoIdentificado;

                return respuesta;
            }

            EmpresaTokenGeneradoModelo tokenGenerado;

            try
            {
                tokenGenerado = GenerarToken(rutEmpresa);
            }
            catch
            {
                respuesta.Mensaje = ConstantesModelo.ConsErrorEmpresaTokenConfiguracionSeguridad;

                return respuesta;
            }

            var registro = new EmpresaTokenModelo
            {
                RutEmpresa = rutEmpresa,
                RutUsuario = empresaToken.RutUsuario.Trim().ToUpperInvariant(),
                Jti = tokenGenerado.Jti,
                TokenHash = CalcularSha256Hex(tokenGenerado.Token),
                ExpiresAt = tokenGenerado.ExpiresAt
            };

            var resultado = esRenovacion ? EmpresaTokenDatos.UpdateEmpresaTokenVigencia(registro) : EmpresaTokenDatos.InsertEmpresaToken(registro);

            switch (resultado)
            {
                case 0:
                    respuesta.Exitoso = true;
                    respuesta.Datos = tokenGenerado;
                    respuesta.Mensaje = esRenovacion ? ConstantesModelo.ConsExitoEmpresaTokenRenovar : ConstantesModelo.ConsExitoEmpresaTokenGenerar;
                    respuesta.AlertaColor = ConstantesModelo.ConsAlertaColorSuccess;
                    respuesta.AlertaTitulo = string.Empty;
                    return respuesta;

                default:
                    respuesta.Mensaje = esRenovacion ? ConstantesModelo.ConsErrorEmpresaTokenRenovar : ConstantesModelo.ConsErrorEmpresaTokenGenerar;
                    return respuesta;
            }
        }

        private static EmpresaTokenGeneradoModelo GenerarToken(string rutEmpresa)
        {
            var issuer = ConstantesModelo.ConsJwtIssuer;
            var audience = ConstantesModelo.ConsJwtAudience;
            var password = ConstantesModelo.ConsJwtPassword;
            var aplicacion = ConstantesModelo.ConsEmpresaTokenAplicacion;

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorEmpresaTokenJwtIssuerNoConfigurado);
            }

            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorEmpresaTokenJwtAudienceNoConfigurado);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorEmpresaTokenJwtPasswordNoConfigurado);
            }

            if (string.IsNullOrWhiteSpace(aplicacion))
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorEmpresaTokenAplicacionNoConfigurada);
            }

            issuer = issuer.Trim();
            audience = audience.Trim();
            password = password.Trim();
            aplicacion = aplicacion.Trim();

            byte[] keyBytes;

            try
            {
                keyBytes = Convert.FromBase64String(password);
            }
            catch
            {
                throw new InvalidOperationException(ConstantesModelo.ConsErrorEmpresaTokenJwtBase64Invalido);
            }

            if (keyBytes.Length < 32)
            {
                Array.Clear(keyBytes, 0, keyBytes.Length);

                throw new InvalidOperationException(ConstantesModelo.ConsErrorEmpresaTokenJwtLongitudInvalida);
            }

            try
            {
                var emitidoAt = DateTime.Now;
                var expiresAt = emitidoAt.AddYears(1);
                var emitidoUtc = emitidoAt.ToUniversalTime();
                var expiresUtc = expiresAt.ToUniversalTime();
                var jti = Guid.NewGuid().ToString("N");
                var securityKey = new SymmetricSecurityKey(keyBytes);
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, rutEmpresa),
                    new Claim(JwtRegisteredClaimNames.Jti, jti),
                    new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(emitidoUtc).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                    new Claim(ConstantesModelo.ConsEmpresaTokenClaimAplicacion, aplicacion)
                };

                var token = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims, notBefore: emitidoUtc, expires: expiresUtc, signingCredentials: signingCredentials);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenString = tokenHandler.WriteToken(token);

                return new EmpresaTokenGeneradoModelo
                {
                    Token = tokenString,
                    Jti = jti,
                    EmitidoAt = emitidoAt,
                    ExpiresAt = expiresAt,
                    RutEmpresa = rutEmpresa
                };
            }
            finally
            {
                Array.Clear(keyBytes, 0, keyBytes.Length);
            }
        }

        private static string CalcularSha256Hex(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                throw new ArgumentException(ConstantesModelo.ConsValidacionEmpresaTokenVacio, nameof(texto));
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(texto);
                var hash = sha256.ComputeHash(bytes);
                var resultado = new StringBuilder(hash.Length * 2);

                foreach (var valor in hash)
                {
                    resultado.Append(valor.ToString("x2"));
                }

                Array.Clear(bytes, 0, bytes.Length);
                Array.Clear(hash, 0, hash.Length);

                return resultado.ToString();
            }
        }

        private static bool TryNormalizarRutEmpresa(string rutEmpresa, out string rutNormalizado)
        {
            rutNormalizado = string.Empty;

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                return false;
            }

            try
            {
                rutNormalizado = FuncionesNegocio.ValidateRut(rutEmpresa.Trim());

                return rutNormalizado != "0";
            }
            catch
            {
                rutNormalizado = string.Empty;

                return false;
            }
        }

        public static EmpresaTokenCabeceraModelo GetEmpresaTokenCabecera(ref int operacion, string rutEmpresa)
        {
            return EmpresaTokenDatos.GetEmpresaTokenCabecera(ref operacion, rutEmpresa);
        }
    }
}