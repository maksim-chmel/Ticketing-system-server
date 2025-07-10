using AdminPanelBack.Models;
using AdminPanelBack.Services.Auth;
using AdminPanelBack.Services.Token;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Services.Login;

public class LoginService(UserManager<Admin> userManager,ITokenService tokenService,
    IRefreshTokenService refreshTokenService,ILogger<LoginService> logger,IAuthService authService) : ILoginService
{
    public async Task<(string accessToken, string refreshToken)> AuthenticateAsync(string username, string password)
    {
        try
        {
            logger.LogInformation("Попытка аутентификации пользователя {Username}", username);
            var user = await authService.FindAdminByUsernameOrThrow(username);
            await authService.CheckPasswordOrThrow(user, password);
            await refreshTokenService.RevokeAllUserTokensAsync(user.Id);
            var (accessToken, refreshToken) = await GenerateTokensAsync(user);
            logger.LogInformation("Пользователь {Username} успешно аутентифицирован", username);
            return (accessToken, refreshToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка аутентификации пользователя {Username}", username);
            throw;
        }
    }
    
    public async Task<(string accessToken, string refreshToken, string userName)> RefreshTokensAsync(string currentRefreshToken)
    {
        try
        {
            logger.LogInformation("Попытка обновления токенов");

            var token = await refreshTokenService.GetRefreshToken(currentRefreshToken);
            if (token == null)
            {
                logger.LogWarning("Неверный refresh токен");
                throw new UnauthorizedAccessException("Недействительный refresh токен");
            }

            var user = await userManager.FindByIdAsync(token.UserId);
            if (user == null)
            {
                logger.LogWarning("Пользователь не найден при обновлении токена");
                throw new UnauthorizedAccessException("Пользователь не найден");
            }
            await refreshTokenService.RevokeRefreshTokenAsync(currentRefreshToken);
            var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user);
            logger.LogInformation("Токены успешно обновлены для пользователя {Username}", user.UserName);

            return (newAccessToken, newRefreshToken, user.UserName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка при обновлении токенов");
            throw;
        }
    }
    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(Admin user)
    
    {
        await refreshTokenService.RevokeRefreshTokenAsync(user.Id);
        var accessToken = tokenService.GenerateToken(user.Id, user.UserName); 
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id);
        return (accessToken, refreshToken);
    }
    

}