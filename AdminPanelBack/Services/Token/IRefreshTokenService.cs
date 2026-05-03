using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Token;

public interface IRefreshTokenService
{
    Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default);
    Task<string> CreateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}