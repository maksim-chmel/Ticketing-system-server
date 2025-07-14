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
        logger.LogInformation("Создание refresh токена для пользователя {UserId}", userId);

        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        logger.LogInformation("Refresh токен создан: {Token}", token);
        return token;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
    {
        logger.LogInformation("Проверка refresh токена для пользователя {UserId}", userId);

        var found = await db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token && r.UserId == userId && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow);

        var isValid = found != null;
        logger.LogInformation("Результат валидации токена {Token}: {IsValid}", token, isValid);
        return isValid;
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        logger.LogInformation("Попытка отозвать refresh токен: {Token}", token);

        var existing = await db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
        if (existing != null)
        {
            existing.IsRevoked = true;
            await db.SaveChangesAsync();

            logger.LogInformation("Refresh токен {Token} отозван", token);
        }
        else
        {
            logger.LogWarning("Не удалось найти refresh токен для отзыва: {Token}", token);
        }
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        logger.LogInformation("Отзыв всех активных токенов для пользователя {UserId}", userId);

        var tokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await db.SaveChangesAsync();
        logger.LogInformation("Отозвано токенов: {Count} для пользователя {UserId}", tokens.Count, userId);
    }

    public async Task<RefreshToken> GetRefreshToken(string refreshToken)
    {
        logger.LogInformation("Получение refresh токена: {Token}", refreshToken);

        var token = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        if (token == null)
            logger.LogWarning("Токен {Token} не найден или недействителен", refreshToken);
        else
            logger.LogInformation("Токен {Token} найден и действителен", refreshToken);

        return token;
    }
}