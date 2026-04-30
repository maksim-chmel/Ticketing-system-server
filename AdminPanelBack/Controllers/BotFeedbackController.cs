using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Middleware;
using AdminPanelBack.Services.Broadcast;
using AdminPanelBack.Services.Feedback;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ServiceFilter(typeof(ApiKeyAuthFilter))]
[ApiController]
[Route("api/operator")]
public class BotFeedbackController(
    IFeedbackService feedbackService,
    IUserService userService,
    IBroadcastMessageService broadcastMessageService)
    : ControllerBase
{
    [HttpPost("feedbacks")]
    public async Task<IActionResult> CreateFeedback([FromBody] UsersMessageDto messageDto, CancellationToken cancellationToken)
    {
        await feedbackService.CreateFeedbackAsync(messageDto, cancellationToken);
        return NoContent();
    }

    [HttpGet("user-ids")]
    public async Task<ActionResult<List<long>>> GetUserIds(CancellationToken cancellationToken)
    {
        var userIds = await userService.GetAllUsersIds(cancellationToken);
        return Ok(userIds);
    }

    [HttpGet("users/{userId:long}")]
    public async Task<ActionResult<UserDto>> GetUser(long userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserById(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException($"User with id={userId} not found");
        return Ok(user);
    }

    [HttpPut("users/{userId:long}")]
    public async Task<IActionResult> UpsertUser(long userId, [FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        if (userDto.UserId != 0 && userDto.UserId != userId)
            throw new ValidationException("userId in body must match route userId");

        userDto.UserId = userId;
        var result = await userService.RegistrationNewUser(userDto, cancellationToken);
        if (!result)
            throw new ValidationException("Registration failed");

        return NoContent();
    }

    [HttpGet("users/{userId:long}/feedbacks")]
    public async Task<IActionResult> GetUserFeedbacks(long userId, CancellationToken cancellationToken)
    {
        var feedbacks = await feedbackService.GetAllUsersFeedbacksAsync(userId, cancellationToken);
        return Ok(feedbacks);
    }

    [HttpPost("broadcast-message-pulls")]
    public async Task<IActionResult> PullBroadcastMessages(CancellationToken cancellationToken)
    {
        var messages = await broadcastMessageService.GetActiveBroadcastMessagesAndMakeInactive(cancellationToken);
        return Ok(messages);
    }

    [HttpPost("unnotified-feedback-pulls")]
    public async Task<IActionResult> PullUnnotifiedFeedbacks(CancellationToken cancellationToken)
    {
        var list = await feedbackService.GetNewFeedbacksForOperatorAsync(cancellationToken);
        return Ok(list);
    }
}
