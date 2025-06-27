namespace AdminPanelBack.Services.TokenServices;

public interface IRefreshTokenService
{
    Task<string> CreateRefreshTokenAsync(string userId);
    Task<bool> ValidateRefreshTokenAsync(string token, string userId);
    Task RevokeRefreshTokenAsync(string token);
}