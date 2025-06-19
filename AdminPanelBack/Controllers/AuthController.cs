using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (UsersStore.Users.TryGetValue(request.Username, out var password) && password == request.Password)
        {
            var token = tokenService.GenerateToken(request.Username, request.Username);
            return Ok(new { token });
        }
        return Unauthorized();
    }
}