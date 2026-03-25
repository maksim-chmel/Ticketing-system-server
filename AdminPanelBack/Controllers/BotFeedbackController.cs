using AdminPanelBack.DTO;
using AdminPanelBack.Services.Broadcast;
using AdminPanelBack.Services.Feedback;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BotFeedbackController(
    IFeedbackService feedbackService,IUserService userService, IBroadcastMessageService broadcastMessageService)
    : ControllerBase
{
    [HttpPost("new-feedback")]
    public async Task<IActionResult> NewFeedback([FromBody] UsersMessageDto messageDto)
    {
        if (string.IsNullOrWhiteSpace(messageDto.Comment))
            return BadRequest("Comment is required.");

        await feedbackService.CreateFeedbackAsync(messageDto);
        return Ok();
    }
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var userIds = await userService.GetAllUsersIds(); 
        return Ok(userIds);
    }

    [HttpPost("register-new-User")]
    public async Task<IActionResult> RegisterNewUser([FromBody] UserDto userDto)
    {
        if (userDto.UserId <= 0) 
            return BadRequest("UserID is required.");
        
        var result = await userService.RegistrationNewUser(userDto);
        if (result)
        {
            return Ok();
        }

        return BadRequest(new { success = false, message = "Registration failed" });
    }
    [HttpGet("exists/{userId}")]
    public async Task<IActionResult> CheckUserExists(long userId)
    {
        var user = await userService.IsUserExists(userId);
        return Ok(user);
    }
    [HttpGet("user-feedbacks/{userId}")]
    public async Task<IActionResult> GetUserFeedbacks(long userId)
    {
        var feedbacks = await feedbackService.GetAllUsersFeedbacksAsync(userId);
        return Ok(feedbacks); 
    }
    [HttpGet("broadcast-messages")]
    public async Task<IActionResult> GetBroadcastMessages()
    {
        var messages = await broadcastMessageService.GetActiveBroadcastMessagesAndMakeInactive();
        return Ok(messages);
    }
}
