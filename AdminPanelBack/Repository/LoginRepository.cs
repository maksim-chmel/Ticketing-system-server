using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Repository;

public interface ILoginRepository
{
}

public class LoginRepository(UserManager<Admin> userManager,SignInManager<Admin> signInManager) : ILoginRepository
{
    
}