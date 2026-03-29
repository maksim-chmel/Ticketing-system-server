using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdminPanelBack.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AdminPanelBack.Services.Token;

public class TokenService(ILogger<TokenService> logger, IOptions<JwtSettings> jwtSettings)
    : ITokenService
{
    private readonly string _secretKey = jwtSettings.Value.SecretKey;
    private readonly string _issuer = jwtSettings.Value.Issuer;
    private readonly string _audience = jwtSettings.Value.Audience;
    private readonly int _expiresInMinutes = jwtSettings.Value.ExpiresInMinutes;

    public string GenerateToken(string userId, string username)
    {
        logger.LogInformation("Generating token for user {UserId} ({Username})", userId, username);

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

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        logger.LogInformation("Token successfully generated for user {UserId}", userId);

        return jwt;
    }
}