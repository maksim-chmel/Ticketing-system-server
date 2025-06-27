using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack;

public static class SeedAdmin
{
   public static async Task SeedAdminAsync(UserManager<Admin> userManager, RoleManager<IdentityRole> roleManager)
    {
        var adminEmail = "admin@example.com";
        var adminPassword = "Admin@123";

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new Admin
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Super Admin"
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                foreach (var error in result.Errors)
                    Console.WriteLine($"❌ {error.Description}");
            }
        }
    }
}