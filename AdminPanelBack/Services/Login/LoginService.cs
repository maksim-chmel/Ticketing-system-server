using AdminPanelBack.Models;
using AdminPanelBack.Services.Auth;
using AdminPanelBack.Services.Token;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Services.Login;

public class LoginService(UserManager<Admin> userManager, ITokenService tokenService,
    IRefreshTokenService refreshTokenService, ILogger<LoginService> logger, IAuthService authService) : ILoginService
{
    public async Task<(string accessToken, string refreshToken)> AuthenticateAsync(string username, string password)
    {
        try
        {
            logger.LogInformation("Authentication attempt for user {Username}", username);
            var user = await authService.FindAdminByUsernameOrThrow(username);
            await authService.CheckPasswordOrThrow(user, password);
            await refreshTokenService.RevokeAllUserTokensAsync(user.Id);
            var (accessToken, refreshToken) = await GenerateTokensAsync(user);
            logger.LogInformation("User {Username} authenticated successfully", username);
            return (accessToken, refreshToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Authentication failed for user {Username}", username);
            throw;
        }
    }

    public async Task<(string accessToken, string refreshToken, string userName)> RefreshTokensAsync(string currentRefreshToken)
    {
        try
        {
            logger.LogInformation("Attempting to refresh tokens");

            var token = await refreshTokenService.GetRefreshToken(currentRefreshToken);
            if (token == null)
            {
                logger.LogWarning("Invalid refresh token provided");
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var user = await userManager.FindByIdAsync(token.UserId);
            if (user == null)
            {
                logger.LogWarning("User not found during token refresh");
                throw new UnauthorizedAccessException("User not found");
            }

            await refreshTokenService.RevokeRefreshTokenAsync(currentRefreshToken);
            var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user);
            logger.LogInformation("Tokens successfully refreshed for user {Username}", user.UserName);

            return (newAccessToken, newRefreshToken, user.UserName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error refreshing tokens");
            throw;
        }
    }

    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(Admin user)
    {
        var accessToken = tokenService.GenerateToken(user.Id, user.UserName);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id);
        logger.LogInformation("Tokens successfully issued for admin {Username} (Id: {UserId})",user.UserName, user.Id);
        return (accessToken, refreshToken);
    }
}