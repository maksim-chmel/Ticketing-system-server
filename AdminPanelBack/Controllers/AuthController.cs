using AdminPanelBack.Models;
using AdminPanelBack.Services.Login;
using AdminPanelBack.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(ILogger<AuthController> logger,ILoginService loginService,IWebHostEnvironment env) : ControllerBase
{
    private const string RefreshTokenCookie = "refreshToken";

    private  CookieOptions RefreshCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
    };

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken) = await loginService.AuthenticateAsync(
            request.Username, request.Password, cancellationToken);

        Response.Cookies.Append(RefreshTokenCookie, refreshToken, RefreshCookieOptions());

        logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException("Refresh token not found");

        var (accessToken, newRefreshToken, _) =
            await loginService.RefreshTokensAsync(refreshToken, cancellationToken);
        Response.Cookies.Append(RefreshTokenCookie, newRefreshToken, RefreshCookieOptions());

        return Ok(new { accessToken });
    }
}
