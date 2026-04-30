using System.Security.Cryptography;
using System.Text;
using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Services.Token;

public class RefreshTokenService(AppDbContext db, ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    private static string HashToken(string rawToken)
    {
        // Refresh tokens are high-entropy random bytes, so a fast one-way hash is sufficient here.
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(hashBytes);
    }

    public async Task<string> CreateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); // raw token for client
        var tokenHash = HashToken(token);
        logger.LogInformation("Creating refresh token for user {UserId}", userId);

        var refreshToken = new RefreshToken
        {
            // Do not store raw token for new sessions. Keep legacy column populated with an empty string
            // to stay compatible with existing NOT NULL schema until it is removed in a future rollout.
            Token = string.Empty,
            TokenHash = tokenHash,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Refresh token created for user {UserId}", userId);
        return token;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Validating refresh token for user {UserId}", userId);

        var tokenHash = HashToken(token);
        var found = await db.RefreshTokens.FirstOrDefaultAsync(r =>
            r.TokenHash == tokenHash &&
            r.UserId == userId &&
            !r.IsRevoked &&
            r.ExpiresAt > DateTime.UtcNow, cancellationToken);

        if (found == null)
        {
            // Seamless rollout fallback: for old rows that still store raw Token (and might not have TokenHash filled).
            found = await db.RefreshTokens.FirstOrDefaultAsync(r =>
                r.Token == token &&
                r.UserId == userId &&
                !r.IsRevoked &&
                r.ExpiresAt > DateTime.UtcNow, cancellationToken);

            if (found != null && string.IsNullOrEmpty(found.TokenHash))
            {
                found.TokenHash = tokenHash;
                found.Token = string.Empty;
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        var isValid = found != null;
        logger.LogInformation("Token validation result for user {UserId}: {IsValid}", userId, isValid);
        return isValid;
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to revoke refresh token");

        var tokenHash = HashToken(token);
        var existing = await db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);
        if (existing == null)
        {
            existing = await db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
            if (existing != null && string.IsNullOrEmpty(existing.TokenHash))
            {
                existing.TokenHash = tokenHash;
                existing.Token = string.Empty;
            }
        }
        if (existing != null)
        {
            existing.IsRevoked = true;
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Refresh token successfully revoked");
        }
        else
        {
            logger.LogWarning("Refresh token not found for revocation");
        }
    }

    public async Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Revoking all active tokens for user {UserId}", userId);

        var tokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Revoked {Count} tokens for user {UserId}", tokens.Count, userId);
    }

    public async Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching refresh token");

        var tokenHash = HashToken(refreshToken);
        var token = await db.RefreshTokens.FirstOrDefaultAsync(t =>
            t.TokenHash == tokenHash &&
            !t.IsRevoked &&
            t.ExpiresAt > DateTime.UtcNow, cancellationToken);

        if (token == null)
        {
            token = await db.RefreshTokens.FirstOrDefaultAsync(t =>
                t.Token == refreshToken &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow, cancellationToken);

            if (token != null && string.IsNullOrEmpty(token.TokenHash))
            {
                token.TokenHash = tokenHash;
                token.Token = string.Empty;
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        if (token == null)
            logger.LogWarning("Refresh token not found or expired");
        else
            logger.LogInformation("Refresh token found and valid");

        return token;
    }
}
