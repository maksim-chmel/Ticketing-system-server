using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace AdminPanelBack
{
    public class TokenService(IConfiguration configuration)
    {
        private readonly string _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                                             ?? configuration["JwtSettings:SecretKey"] 
                                             ?? throw new ArgumentNullException("SecretKey");
        private readonly string _issuer = configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("Issuer");
        private readonly string _audience = configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("Audience");
        private readonly int _expiresInMinutes = int.Parse(configuration["JwtSettings:ExpiresInMinutes"] ?? "60");

        // Получаем секретный ключ из переменной окружения, если нет — из конфига

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