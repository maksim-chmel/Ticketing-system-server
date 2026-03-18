using System.Security.Cryptography;
using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Services.Token;

public class RefreshTokenService(AppDbContext db, ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    public async Task<string> CreateRefreshTokenAsync(string userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        logger.LogInformation("Creating refresh token for user {UserId}", userId);

        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        logger.LogInformation("Refresh token created for user {UserId}", userId);
        return token;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
    {
        logger.LogInformation("Validating refresh token for user {UserId}", userId);

        var found = await db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token && r.UserId == userId && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow);

        var isValid = found != null;
        logger.LogInformation("Token validation result for user {UserId}: {IsValid}", userId, isValid);
        return isValid;
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        logger.LogInformation("Attempting to revoke refresh token");

        var existing = await db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
        if (existing != null)
        {
            existing.IsRevoked = true;
            await db.SaveChangesAsync();
            logger.LogInformation("Refresh token successfully revoked");
        }
        else
        {
            logger.LogWarning("Refresh token not found for revocation");
        }
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        logger.LogInformation("Revoking all active tokens for user {UserId}", userId);

        var tokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await db.SaveChangesAsync();
        logger.LogInformation("Revoked {Count} tokens for user {UserId}", tokens.Count, userId);
    }

    public async Task<RefreshToken> GetRefreshToken(string refreshToken)
    {
        logger.LogInformation("Fetching refresh token");

        var token = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        if (token == null)
            logger.LogWarning("Refresh token not found or expired");
        else
            logger.LogInformation("Refresh token found and valid");

        return token;
    }
}