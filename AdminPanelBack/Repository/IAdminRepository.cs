using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IAdminRepository
{
    Task<Admin?> FindByUsernameAsync(string username);
    Task<Admin?> FindByIdAsync(string id);
    Task<bool> CheckPasswordAsync(Admin admin, string password);
    Task<IList<string>> GetRolesAsync(Admin admin);
}
