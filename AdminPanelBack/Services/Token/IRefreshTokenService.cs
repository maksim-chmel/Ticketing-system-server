using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Token;

public interface IRefreshTokenService
{
    public Task RevokeAllUserTokensAsync(string userId);
    Task<string> CreateRefreshTokenAsync(string userId);
    Task<bool> ValidateRefreshTokenAsync(string token, string userId);
    Task RevokeRefreshTokenAsync(string token);
    public Task<RefreshToken?> GetRefreshToken(string refreshToken);
}