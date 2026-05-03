using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/users")]
public class UserController(IUserService service,
    ILogger<UserController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default) 
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        logger.LogInformation("Fetching user list. Page: {Page}, Size: {Size}", page, pageSize);
    
       
        var result = await service.GetAllUsers(page, pageSize, ct);
    
        logger.LogInformation("Retrieved {Count} users", result.Count);
        return Ok(result);
    }

    [HttpGet("{userId:long}")]
    public async Task<ActionResult<UserDto>> GetById(long userId, CancellationToken cancellationToken)
    {
        var user = await service.GetUserById(userId, cancellationToken);
        if (user == null)
        {
            return NotFound($"User with id={userId} not found");
        }
        return Ok(user);
    }

    [HttpPatch("{userId:long}/comment")]
    public async Task<IActionResult> PatchComment(long userId, [FromBody] UpdateUserCommentRequest request,CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating comment for user Id={UserId}", userId);
        var result = await service.ManageComment(userId, request.Comment, cancellationToken);
        logger.LogInformation("Comment for user Id={UserId} updated successfully", userId);
        return Ok(result);
    }
}
