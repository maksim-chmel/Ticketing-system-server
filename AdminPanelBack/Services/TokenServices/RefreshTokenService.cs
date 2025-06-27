using System.Security.Cryptography;
using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Services.TokenServices;
public class RefreshTokenService(AppDbContext db):IRefreshTokenService
{
    public async Task<string> CreateRefreshTokenAsync(string userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        return token;
    }
    public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
    {
        var found = await db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token && r.UserId == userId && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow);

        return found != null;
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var existing = await db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
        if (existing != null)
        {
            existing.IsRevoked = true;
            await db.SaveChangesAsync();
        }
    }
    
}