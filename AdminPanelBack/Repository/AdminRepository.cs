using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Repository;

public class AdminRepository(UserManager<Admin> userManager) : IAdminRepository
{
    public Task<Admin?> FindByUsernameAsync(string username) =>
        userManager.FindByNameAsync(username);

    public Task<Admin?> FindByIdAsync(string id) =>
        userManager.FindByIdAsync(id);

    public Task<bool> CheckPasswordAsync(Admin admin, string password) =>
        userManager.CheckPasswordAsync(admin, password);

    public Task<IList<string>> GetRolesAsync(Admin admin) =>
        userManager.GetRolesAsync(admin);
}
