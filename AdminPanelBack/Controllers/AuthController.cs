using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Services.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AdminPanelBack.Controllers;

/// <summary>Admin authentication.</summary>
[ApiController]
[Route("api/auth")]
[EnableRateLimiting("fixed")]
public class AuthController(ILogger<AuthController> logger, ILoginService loginService, IWebHostEnvironment env) : ControllerBase
{
    private const string RefreshTokenCookie = "refreshToken";

    private CookieOptions RefreshCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
    };

    /// <summary>Log in.</summary>
    /// <remarks>Returns a JWT access token. The refresh token is set as an HttpOnly cookie (7 days).</remarks>
    /// <response code="200">Authenticated successfully. Returns the access token.</response>
    /// <response code="400">Empty username or password.</response>
    /// <response code="401">Invalid username or password.</response>
    /// <response code="429">Rate limit exceeded.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken) = await loginService.AuthenticateAsync(
            request.Username, request.Password, cancellationToken);

        Response.Cookies.Append(RefreshTokenCookie, refreshToken, RefreshCookieOptions());

        logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(new { accessToken });
    }

    /// <summary>Refresh tokens.</summary>
    /// <remarks>Uses the refresh token from the HttpOnly cookie. Returns a new access token and updates the cookie.</remarks>
    /// <response code="200">Tokens refreshed successfully. Returns the new access token.</response>
    /// <response code="401">Refresh token is missing, expired or revoked.</response>
    /// <response code="429">Rate limit exceeded.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
