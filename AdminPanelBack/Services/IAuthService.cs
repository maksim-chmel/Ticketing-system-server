using AdminPanelBack.Models;

namespace AdminPanelBack.Services;

public interface IAuthService
{
    Task<Admin> FindAdminByUsernameOrThrow(string username);
    Task CheckPasswordOrThrow(Admin admin, string password);
}