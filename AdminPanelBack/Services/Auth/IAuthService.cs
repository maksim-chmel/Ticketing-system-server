using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Auth;

public interface IAuthService
{
    Task<Admin> FindAdminByUsernameOrThrow(string username);
    Task CheckPasswordOrThrow(Admin admin, string password);
}