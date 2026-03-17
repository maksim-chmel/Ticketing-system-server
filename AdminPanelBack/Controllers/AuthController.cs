using AdminPanelBack.Models;
using AdminPanelBack.Services;
using AdminPanelBack.Services.Login;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger,ILoginService loginService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (accessToken, refreshToken) = await loginService.AuthenticateAsync
            (request.Username, request.Password);

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Refresh token not found");
        var (accessToken, newRefreshToken, userName) = 
                await loginService.RefreshTokensAsync(refreshToken);
            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { accessToken });
        
    }
}