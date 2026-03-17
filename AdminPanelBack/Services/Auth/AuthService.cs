using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Services.Auth;

public class AuthService(ILogger<AuthService> logger, UserManager<Admin> userManager) : IAuthService
{
    public async Task<Admin> FindAdminByUsernameOrThrow(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning("Empty username passed to FindAdminByUsernameOrThrow");
            throw new ArgumentException("Username cannot be empty", nameof(username));
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            logger.LogWarning("Admin with username {Username} not found", username);
            throw new InvalidOperationException("Admin not found");
        }

        return user;
    }

    public async Task CheckPasswordOrThrow(Admin admin, string password)
    {
        if (admin == null)
        {
            logger.LogWarning("Null admin passed to CheckPasswordOrThrow");
            throw new ArgumentNullException(nameof(admin));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Empty password passed for user {Username}", admin.UserName);
            throw new ArgumentException("Password cannot be empty", nameof(password));
        }

        var result = await userManager.CheckPasswordAsync(admin, password);
        if (!result)
        {
            logger.LogWarning("Invalid password for user {Username}", admin.UserName);
            throw new UnauthorizedAccessException("Invalid password");
        }
    }
}