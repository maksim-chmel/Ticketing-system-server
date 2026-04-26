using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Services.Broadcast;
using AdminPanelBack.Services.Feedback;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/operator")]
public class BotFeedbackController(
    IFeedbackService feedbackService,
    IUserService userService,
    IBroadcastMessageService broadcastMessageService)
    : ControllerBase
{
    [HttpPost("feedbacks")]
    public async Task<IActionResult> CreateFeedback([FromBody] UsersMessageDto messageDto)
    {
        await feedbackService.CreateFeedbackAsync(messageDto);
        return NoContent();
    }

    [HttpGet("user-ids")]
    public async Task<ActionResult<List<long>>> GetUserIds()
    {
        var userIds = await userService.GetAllUsersIds();
        return Ok(userIds);
    }

    [HttpGet("users/{userId:long}")]
    public async Task<ActionResult<UserDto>> GetUser(long userId)
    {
        var user = await userService.GetUserById(userId);
        if (user == null)
            throw new NotFoundException($"User with id={userId} not found");
        return Ok(user);
    }

    [HttpPut("users/{userId:long}")]
    public async Task<IActionResult> UpsertUser(long userId, [FromBody] UserDto userDto)
    {
        if (userDto.UserId != 0 && userDto.UserId != userId)
            throw new ValidationException("userId in body must match route userId");

        userDto.UserId = userId;
        var result = await userService.RegistrationNewUser(userDto);
        if (!result)
            throw new ValidationException("Registration failed");

        return NoContent();
    }

    [HttpGet("users/{userId:long}/feedbacks")]
    public async Task<IActionResult> GetUserFeedbacks(long userId)
    {
        var feedbacks = await feedbackService.GetAllUsersFeedbacksAsync(userId);
        return Ok(feedbacks);
    }

    [HttpPost("broadcast-message-pulls")]
    public async Task<IActionResult> PullBroadcastMessages()
    {
        var messages = await broadcastMessageService.GetActiveBroadcastMessagesAndMakeInactive();
        return Ok(messages);
    }

    [HttpPost("unnotified-feedback-pulls")]
    public async Task<IActionResult> PullUnnotifiedFeedbacks()
    {
        var list = await feedbackService.GetNewFeedbacksForOperatorAsync();
        return Ok(list);
    }
}
