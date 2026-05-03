using System.Security.Cryptography;
using System.Text;
using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    private static string HashToken(string rawToken)
    {
        if (string.IsNullOrEmpty(rawToken))
        {
            throw new ArgumentException("Raw token cannot be null or empty", nameof(rawToken));
        }

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(hashBytes);
    }

    public Task<string> CreateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenHash = HashToken(token);

        var refreshToken = new RefreshToken
        {
            Token = string.Empty,
            TokenHash = tokenHash,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        dbContext.RefreshTokens.Add(refreshToken);

        return Task.FromResult(token);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default)
    {
       

        var tokenHash = HashToken(token);
        var found = await dbContext.RefreshTokens.FirstOrDefaultAsync(r =>
            r.TokenHash == tokenHash &&
            r.UserId == userId &&
            !r.IsRevoked &&
            r.ExpiresAt > DateTime.UtcNow, cancellationToken);

        if (found == null)
        {
           
            found = await dbContext.RefreshTokens.FirstOrDefaultAsync(r =>
                r.Token == token &&
                r.UserId == userId &&
                !r.IsRevoked &&
                r.ExpiresAt > DateTime.UtcNow, cancellationToken);

            if (found != null && string.IsNullOrEmpty(found.TokenHash))
            {
                found.TokenHash = tokenHash;
                found.Token = string.Empty;
            }
        }

        var isValid = found != null;
        
        return isValid;
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
       

        var tokenHash = HashToken(token);
        var existing = await dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);
        if (existing == null)
        {
            existing = await dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
            if (existing != null && string.IsNullOrEmpty(existing.TokenHash))
            {
                existing.TokenHash = tokenHash;
                existing.Token = string.Empty;
            }
        }
        if (existing != null)
        {
            existing.IsRevoked = true;
        }
       
    }

    public async Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default)
    {
        

        var tokens = await dbContext.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
       
    }

    public async Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(t =>
            t.TokenHash == tokenHash &&
            !t.IsRevoked &&
            t.ExpiresAt > DateTime.UtcNow, cancellationToken);

        if (token != null) return token;
        {
            token = await dbContext.RefreshTokens.FirstOrDefaultAsync(t =>
                t.Token == refreshToken &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow, cancellationToken);

            if (token == null || !string.IsNullOrEmpty(token.TokenHash)) return token;
            token.TokenHash = tokenHash;
            token.Token = string.Empty;
        }

        return token;
    }
}