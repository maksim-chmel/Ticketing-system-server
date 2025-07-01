using AdminPanelBack.DB;
using AdminPanelBack.Models;
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
    AppDbContext  dbContext,
    ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        logger.LogInformation("Login attempt for user {Username}", request.Username);

        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            logger.LogWarning("Login failed: User not found - {Username}", request.Username);
            return Unauthorized(new { message = "User not found" });
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            logger.LogWarning("Login failed: Invalid credentials for user {Username}", request.Username);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var accessToken = tokenService.GenerateToken(user.Id, user.UserName);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id);

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        logger.LogInformation("Generated access token: {Token}", accessToken);
        logger.LogInformation("User {Username} logged in successfully, access token and refresh token issued", request.Username);
        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        logger.LogInformation("Token refresh attempt");

        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            logger.LogWarning("Token refresh failed: Refresh token missing");
            return Unauthorized(new { message = "Refresh token missing" });
        }

        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        if (token == null)
        {
            logger.LogWarning("Token refresh failed: Invalid or expired refresh token");
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        var user = await userManager.FindByIdAsync(token.UserId);
        if (user == null)
        {
            logger.LogWarning("Token refresh failed: User not found for token {Token}", refreshToken);
            return Unauthorized(new { message = "User not found" });
        }

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

        logger.LogInformation("Token refreshed successfully for user {Username}", user.UserName);
        return Ok(new { accessToken = newAccessToken });
    }
}