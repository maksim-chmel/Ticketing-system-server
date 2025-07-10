using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Services;

public class AuthService(ILogger<AuthService> logger,UserManager<Admin> userManager) : IAuthService
{
    public async Task<Admin> FindAdminByUsernameOrThrow(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning("Пустое имя пользователя передано в FindAdminByUsernameOrThrow");
            throw new ArgumentException("Имя пользователя не может быть пустым", nameof(username));
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            logger.LogWarning("Администратор с именем {Username} не найден", username);
            throw new InvalidOperationException("Администратор не найден");
        }

        return user;
    }

    public async Task CheckPasswordOrThrow(Admin admin, string password)
    {
        if (admin == null)
        {
            logger.LogWarning("В метод CheckPasswordOrThrow передан null вместо пользователя");
            throw new ArgumentNullException(nameof(admin));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Пустой пароль передан для пользователя {Username}", admin.UserName);
            throw new ArgumentException("Пароль не может быть пустым", nameof(password));
        }

        var result = await userManager.CheckPasswordAsync(admin, password);
        if (!result)
        {
            logger.LogWarning("Неверный пароль для пользователя {Username}", admin.UserName);
            throw new UnauthorizedAccessException("Неверный пароль");
        }
    }

}