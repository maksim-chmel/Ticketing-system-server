using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Auth;
using AdminPanelBack.Services.Token;

namespace AdminPanelBack.Services.Login;

public class LoginService(IAdminRepository adminRepository, ITokenService tokenService,
    IRefreshTokenService refreshTokenService, ILogger<LoginService> logger, IAuthService authService) : ILoginService
{
    public async Task<(string accessToken, string refreshToken)> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Authentication attempt for user {Username}", username);
        var user = await authService.FindAdminByUsernameOrThrow(username);
        await authService.CheckPasswordOrThrow(user, password);
        await refreshTokenService.RevokeAllUserTokensAsync(user.Id, cancellationToken);
        var (accessToken, refreshToken) = await GenerateTokensAsync(user, cancellationToken);
        logger.LogInformation("User {Username} authenticated successfully", username);
        return (accessToken, refreshToken);
    }

    public async Task<(string accessToken, string refreshToken, string userName)> RefreshTokensAsync(string currentRefreshToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to refresh tokens");

        var token = await refreshTokenService.GetRefreshTokenAsync(currentRefreshToken, cancellationToken);
        if (token == null)
        {
            logger.LogWarning("Invalid refresh token provided");
            throw new UnauthorizedException("Invalid refresh token");
        }

        var user = await adminRepository.FindByIdAsync(token.UserId);
        if (user == null)
        {
            logger.LogWarning("User not found during token refresh");
            throw new UnauthorizedException("User not found");
        }

        await refreshTokenService.RevokeRefreshTokenAsync(currentRefreshToken, cancellationToken);
        var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user, cancellationToken);
        logger.LogInformation("Tokens successfully refreshed for user {Username}", user.UserName);

        return (newAccessToken, newRefreshToken, user.UserName!);
    }

    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(Admin user, CancellationToken cancellationToken = default)
    {
        var roles = await adminRepository.GetRolesAsync(user);
        var accessToken = tokenService.GenerateToken(user.Id, user.UserName!, user.Name, roles);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id, cancellationToken);
        logger.LogInformation("Tokens successfully issued for admin {Username} (Id: {UserId})", user.UserName, user.Id);
        return (accessToken, refreshToken);
    }
}
