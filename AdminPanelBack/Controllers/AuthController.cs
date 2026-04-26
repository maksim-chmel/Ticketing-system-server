using AdminPanelBack.Models;
using AdminPanelBack.Services.Login;
using AdminPanelBack.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(ILogger<AuthController> logger,ILoginService loginService) : ControllerBase
{
    private const string RefreshTokenCookie = "refreshToken";

    private static CookieOptions RefreshCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
    };

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (accessToken, refreshToken) = await loginService.AuthenticateAsync
            (request.Username, request.Password);

        Response.Cookies.Append(RefreshTokenCookie, refreshToken, RefreshCookieOptions());

        logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException("Refresh token not found");
        var (accessToken, newRefreshToken, _) = 
                await loginService.RefreshTokensAsync(refreshToken);
            Response.Cookies.Append(RefreshTokenCookie, newRefreshToken, RefreshCookieOptions());

            return Ok(new { accessToken });
        
    }
}
