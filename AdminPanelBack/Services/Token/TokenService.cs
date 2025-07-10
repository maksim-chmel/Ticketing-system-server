using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AdminPanelBack.Services.Token
{
    public class TokenService:ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresInMinutes;

        public TokenService()
        {
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                         ?? throw new Exception("JWT_SECRET_KEY not set");
            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                      ?? throw new Exception("JWT_ISSUER not set");
            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                        ?? throw new Exception("JWT_AUDIENCE not set");
            var expiresStr = Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES")
                             ?? throw new Exception("JWT_EXPIRES_IN_MINUTES not set");
            _expiresInMinutes = int.Parse(expiresStr);
        }

        public string GenerateToken(string userId, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiresInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}