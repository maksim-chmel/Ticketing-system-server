using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IRefreshTokenRepository
{
    Task<string> CreateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken = default);
}