using AdminPanelBack.Models;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services.Token;

public class RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork) : IRefreshTokenService
{
    public async Task<string> CreateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var token = await refreshTokenRepository.CreateRefreshTokenAsync(userId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Refresh token created for user {UserId}", userId);
        return token;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Validating refresh token for user {UserId}", userId);
        var isValid = await refreshTokenRepository.ValidateRefreshTokenAsync(token, userId, cancellationToken);
        if (isValid)
            await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Token validation result for user {UserId}: {IsValid}", userId, isValid);
        return isValid;
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to revoke refresh token");
        await refreshTokenRepository.RevokeRefreshTokenAsync(token, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Refresh token successfully revoked");
    }

    public async Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Revoking all active tokens for user {UserId}", userId);
        await refreshTokenRepository.RevokeAllUserTokensAsync(userId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching refresh token");
        var token = await refreshTokenRepository.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (token != null)
            await unitOfWork.SaveChangesAsync(cancellationToken);
        return token;
    }
}
