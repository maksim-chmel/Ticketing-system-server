using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Services;
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
        try
        {
            var result = await service.GetAllUsers();
            logger.LogInformation("Retrieved {Count} users", result?.Count() ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user list");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("update-comment")]
    public async Task<IActionResult> UpdateUserComment([FromBody] UpdateCommentRequest request)
    {
        logger.LogInformation("Updating comment for user Id={UserId}", request.UserId);
        try
        {
            var result = await service.ManageComment(request.UserId, request.Comment);
            logger.LogInformation("Comment for user Id={UserId} updated successfully", request.UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating comment for user Id={UserId}", request.UserId);
            return StatusCode(500, "Internal server error");
        }
    }
}