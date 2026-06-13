using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AdminPanelBack.Controllers;

/// <summary>User management.</summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/users")]
public class UserController(
    IUserService service,
    IOutputCacheStore outputCacheStore,
    ILogger<UserController> logger) : ControllerBase
{
    /// <summary>Get a list of users (with pagination).</summary>
    /// <param name="page">The page number (minimum 1).</param>
    /// <param name="pageSize">The number of items per page (1–200).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">A list of users.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    [HttpGet]
    [OutputCache(PolicyName = "AdminUsersListPolicy")]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        logger.LogInformation("Fetching user list. Page: {Page}, Size: {Size}", page, pageSize);
        var result = await service.GetAllUsers(page, pageSize, ct);
        logger.LogInformation("Retrieved {Count} users", result.Items.Count);
        return Ok(result);
    }

    /// <summary>Get user details by ID.</summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">User details retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{userId:long}")]
    [OutputCache(PolicyName = "AdminUserByIdPolicy")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(long userId, CancellationToken cancellationToken)
    {
        var user = await service.GetUserById(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException($"User with id={userId} not found");
        return Ok(user);
    }

    /// <summary>Update the administrator's comment for a user.</summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">Request body containing the comment text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">The updated user data.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    /// <response code="404">User not found.</response>
    [HttpPatch("{userId:long}/comment")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchComment(long userId, [FromBody] UpdateUserCommentRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating comment for user Id={UserId}", userId);
        var result = await service.ManageComment(userId, request.Comment, cancellationToken);
        await outputCacheStore.EvictByTagAsync("users", cancellationToken);
        logger.LogInformation("Comment for user Id={UserId} updated successfully", userId);
        return Ok(result);
    }
}
