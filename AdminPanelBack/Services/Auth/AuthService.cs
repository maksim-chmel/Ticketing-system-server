using AdminPanelBack.Models;
using AdminPanelBack.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Services.Auth;

public class AuthService(ILogger<AuthService> logger, UserManager<Admin> userManager) : IAuthService
{
    public async Task<Admin> FindAdminByUsernameOrThrow(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning("Empty username passed to FindAdminByUsernameOrThrow");
            throw new ValidationException("Username cannot be empty");
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            logger.LogWarning("Admin with username {Username} not found", username);
            throw new NotFoundException("Admin not found");
        }

        return user;
    }

    public async Task CheckPasswordOrThrow(Admin admin, string password)
    {
        ArgumentNullException.ThrowIfNull(admin);

        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Empty password passed for user {Username}", admin.UserName);
            throw new ValidationException("Password cannot be empty");
        }

        var result = await userManager.CheckPasswordAsync(admin, password);
        if (!result)
        {
            logger.LogWarning("Invalid password for user {Username}", admin.UserName);
            throw new UnauthorizedException("Invalid password");
        }
    }
}