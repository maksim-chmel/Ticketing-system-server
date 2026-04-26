using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/users")]
public class UserController(IUserService service,
    ILogger<UserController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 50 : pageSize;
        pageSize = pageSize > 200 ? 200 : pageSize;

        logger.LogInformation("Fetching user list");
        var result = await service.GetAllUsers(page, pageSize);
        logger.LogInformation("Retrieved {Count} users", result.Count);
        return Ok(result);
    }

    [HttpGet("{userId:long}")]
    public async Task<ActionResult<UserDto>> GetById(long userId)
    {
        var user = await service.GetUserById(userId);
        if (user == null)
            throw new NotFoundException($"User with id={userId} not found");
        return Ok(user);
    }

    [HttpPatch("{userId:long}/comment")]
    public async Task<IActionResult> PatchComment(long userId, [FromBody] UpdateUserCommentRequest request)
    {
        logger.LogInformation("Updating comment for user Id={UserId}", userId);
        var result = await service.ManageComment(userId, request.Comment);
        logger.LogInformation("Comment for user Id={UserId} updated successfully", userId);
        return Ok(result);
    }
}
