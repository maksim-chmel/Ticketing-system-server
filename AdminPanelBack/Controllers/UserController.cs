using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService service) : ControllerBase
{
    [HttpGet("users-to-list")]
    public async Task<IActionResult> GetUsersToList()
    {
        var result = await service.GetAllUsers();
        return Ok(result);
    }
    [HttpPost("update-comment")]
    public async Task<IActionResult> UpdateUserComment([FromBody]UpdateCommentRequest  request)
    {
      var result = await service.ManageComment(request.UserId, request.Comment);
        return Ok(result);
    }
}