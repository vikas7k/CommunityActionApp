using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace CommunityFunctions.Security
{
    public class JwtValidationResult
    {
        public bool IsValid { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public string FailureMessage { get; set; }
    }

    public interface IJwtValidator
    {
        Task<JwtValidationResult> ValidateTokenAsync(string bearerToken);
    }

    public class JwtValidator : IJwtValidator
    {
        private readonly TokenValidationParameters _validationParameters;

        public JwtValidator(IConfiguration config)
        {
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];
            var signingKey = config["Jwt:SigningKey"]; // symmetric key for HMAC (base64 or plain)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey ?? throw new ArgumentNullException("Jwt:SigningKey")));
            _validationParameters = new TokenValidationParameters
            {
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        }

        public Task<JwtValidationResult> ValidateTokenAsync(string bearerToken)
        {
            if (string.IsNullOrEmpty(bearerToken))
                return Task.FromResult(new JwtValidationResult { IsValid = false, FailureMessage = "No token provided" });

            string token = bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? bearerToken.Substring(7).Trim()
                : bearerToken;

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, _validationParameters, out var validatedToken);
                return Task.FromResult(new JwtValidationResult { IsValid = true, Principal = principal });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new JwtValidationResult { IsValid = false, FailureMessage = ex.Message });
            }
        }
    }
}
