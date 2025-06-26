using AdminPanelBack.DB;
using AdminPanelBack.Models;
using AdminPanelBack.Services;
using AdminPanelBack.Services.TokenServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<Admin> userManager,
    SignInManager<Admin> signInManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    AppDbContext  dbContext)
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

        var accessToken = tokenService.GenerateToken(user.Id, user.UserName);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id);

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new { accessToken });
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "Refresh token missing" });

        // Найдём токен в БД
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        if (token == null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        var user = await userManager.FindByIdAsync(token.UserId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });

        // Отзываем старый токен и выдаём новый
        await refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        var newRefreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id);
        var newAccessToken = tokenService.GenerateToken(user.Id, user.UserName);

        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new { accessToken = newAccessToken });
    }
    
}