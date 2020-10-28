using System;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using RaspSecure.Models.Auth;
using System.Security.Claims;
using System.Security.Principal;
using RaspSecure.Models.Exceptions;

namespace RaspSecure.Helpers
{
    public sealed class JwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<AccessToken> GenerateAccessToken(int id, string userName, string email, RolesEnum role)
        {
            var identity = GenerateClaimsIdentity(id, userName);

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, userName),
                 new Claim(JwtRegisteredClaimNames.Email, email),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst("id"),
                 new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())
             };

            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            return new AccessToken(_jwtSecurityTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);
        }

        public string GenerateRefreshToken()
        {
            var tmpToken = Convert.ToBase64String(SecurityHelper.GetRandomBytes());
            tmpToken = tmpToken.Trim('=');
            tmpToken = tmpToken.Replace('/', '_');
            return tmpToken.Replace('+', '-');
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            return ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtOptions.SigningCredentials.Key,
                ValidateLifetime = false,
            });
        }

        public int GetUserIdFromToken(string accessToken)
        {
            var claimsPrincipal = GetPrincipalFromToken(accessToken);

            if (claimsPrincipal is null)
                throw new InvalidTokenException("access");

            return int.Parse(claimsPrincipal.Claims.First(c => c.Type == "id").Value);
        }

        private ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static ClaimsIdentity GenerateClaimsIdentity(int id, string userName)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim("id", id.ToString())
            });
        }

        private static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() -
                                          new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                                         .TotalSeconds);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials is null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator is null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }
}
