using AdminPanelBack.Models;
using AdminPanelBack.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<Admin> userManager,
    SignInManager<Admin> signInManager,
    TokenService tokenService)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null)
            return Unauthorized(new { message = "User not found" });

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = tokenService.GenerateToken(user.Id, user.UserName);
        return Ok(new { token });
    }
}