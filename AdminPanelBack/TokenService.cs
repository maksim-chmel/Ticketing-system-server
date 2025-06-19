using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace AdminPanelBack
{
    public class TokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresInMinutes;

        public TokenService(IConfiguration configuration)
        {
            // Сначала пробуем взять секретный ключ из переменной окружения
            var envKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            // Если переменная окружения пуста — берём из конфигурации
            _secretKey = !string.IsNullOrWhiteSpace(envKey)
                ? envKey
                : configuration["JwtSettings:SecretKey"]
                  ?? throw new ArgumentNullException("SecretKey", "JWT_SECRET_KEY и JwtSettings:SecretKey не заданы");

            _issuer = configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("Issuer");
            _audience = configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("Audience");
            _expiresInMinutes = int.TryParse(configuration["JwtSettings:ExpiresInMinutes"], out int minutes)
                ? minutes
                : 60; // Значение по умолчанию
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