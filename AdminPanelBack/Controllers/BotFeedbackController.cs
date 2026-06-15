using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Middleware;
using AdminPanelBack.Models;
using AdminPanelBack.Services.Broadcast;
using AdminPanelBack.Services.Feedback;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace AdminPanelBack.Controllers;

/// <summary>Telegram bot API (authenticated via X-Api-Key header).</summary>
[ServiceFilter(typeof(ApiKeyAuthFilter))]
[EnableRateLimiting("bot")]
[ApiController]
[Route("api/operator")]
public class BotFeedbackController(
    IFeedbackService feedbackService,
    IUserService userService,
    IBroadcastMessageService broadcastMessageService,
    IOutputCacheStore outputCacheStore) : ControllerBase
{
    /// <summary>Create a feedback request from a bot user.</summary>
    /// <response code="204">Feedback request created successfully.</response>
    /// <response code="400">Invalid data provided.</response>
    /// <response code="401">Invalid or missing API key.</response>
    [HttpPost("feedbacks")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateFeedback([FromBody] UsersMessageDto messageDto, CancellationToken cancellationToken)
    {
        await feedbackService.CreateFeedbackAsync(messageDto, cancellationToken);
        await outputCacheStore.EvictByTagAsync("feedbacks", cancellationToken);
        await outputCacheStore.EvictByTagAsync("statistics", cancellationToken);
        return NoContent();
    }

    /// <summary>Get a list of all user IDs.</summary>
    /// <response code="200">A list of user identifiers.</response>
    /// <response code="401">Invalid or missing API key.</response>
    [HttpGet("user-ids")]
    [ProducesResponseType(typeof(List<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<long>>> GetUserIds(CancellationToken cancellationToken)
    {
        var userIds = await userService.GetAllUsersIds(cancellationToken);
        return Ok(userIds);
    }

    /// <summary>Get user details by ID.</summary>
    /// <param name="userId">The Telegram user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">User details retrieved successfully.</response>
    /// <response code="401">Invalid or missing API key.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("users/{userId:long}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(long userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserById(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException($"User with id={userId} not found");
        return Ok(user);
    }

    /// <summary>Create or update a user (upsert).</summary>
    /// <param name="userId">The Telegram user identifier.</param>
    /// <param name="userDto">User data for creation or update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">User was successfully created or updated.</response>
    /// <response code="400">UserId in the request body does not match the userId in the route.</response>
    /// <response code="401">Invalid or missing API key.</response>
    [HttpPut("users/{userId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpsertUser(long userId, [FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        if (userDto.UserId != 0 && userDto.UserId != userId)
            throw new ValidationException("userId in body must match route userId");

        userDto.UserId = userId;
        await userService.RegistrationNewUser(userDto, cancellationToken);
        await outputCacheStore.EvictByTagAsync("users", cancellationToken);
        return NoContent();
    }

    /// <summary>Get feedback requests from a specific user.</summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">A list of user feedback requests.</response>
    /// <response code="401">Invalid or missing API key.</response>
    [HttpGet("users/{userId:long}/feedbacks")]
    [ProducesResponseType(typeof(List<FeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserFeedbacks(long userId, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetAllUsersFeedbacksAsync(userId, cancellationToken);
        return Ok(feedbacks);
    }

    /// <summary>Get active broadcast messages and mark them as sent.</summary>
    /// <remarks>Atomic operation: retrieves active messages and immediately deactivates them.</remarks>
    /// <response code="200">A list of active messages (prior to deactivation).</response>
    /// <response code="401">Invalid or missing API key.</response>
    [HttpPost("broadcast-message-pulls")]
    [ProducesResponseType(typeof(List<BroadcastMessage>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PullBroadcastMessages(CancellationToken cancellationToken)
    {
        var messages = await broadcastMessageService.GetActiveBroadcastMessagesAndMakeInactive(cancellationToken);
        return Ok(messages);
    }

    
}
