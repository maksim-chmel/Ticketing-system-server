using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AdminPanelBack;

public static class SeedAdmin
{
   public static async Task<bool> SeedAdminAsync(
       UserManager<Admin> userManager,
       RoleManager<IdentityRole> roleManager,
       IConfiguration configuration,
       IHostEnvironment env)
    {
        var adminEmail =
            configuration["ADMIN_EMAIL"] ??
            configuration["SeedAdmin:Email"];

        var adminPassword =
            configuration["ADMIN_PASSWORD"] ??
            configuration["SeedAdmin:Password"];

        var adminName =
            configuration["ADMIN_NAME"] ??
            configuration["SeedAdmin:Name"] ??
            "Super Admin";

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            if (env.IsDevelopment())
            {
                adminEmail ??= "admin@example.com";
                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    Log.Warning("ADMIN_PASSWORD not set for development environment, skipping admin seeding.");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new Admin
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = adminName
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                return true;
            }
            return false;
        }

        return true;
    }
}