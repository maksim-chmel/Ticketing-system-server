using AdminPanelBack.Models;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService service,
    ILogger<UserController> logger) : ControllerBase
{
    [HttpGet("users-to-list")]
    public async Task<IActionResult> GetUsersToList()
    {
        logger.LogInformation("Fetching user list");
        var result = await service.GetAllUsers();
        logger.LogInformation("Retrieved {Count} users", result.Count);
        return Ok(result);
    }

    [HttpPost("update-comment")]
    public async Task<IActionResult> UpdateUserComment([FromBody] UpdateCommentRequest request)
    {
        logger.LogInformation("Updating comment for user Id={UserId}", request.UserId);
        var result = await service.ManageComment(request.UserId, request.Comment);
        logger.LogInformation("Comment for user Id={UserId} updated successfully", request.UserId);
        return Ok(result);
    }
}