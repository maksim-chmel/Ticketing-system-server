using AdminPanelBack.DTO;
using AdminPanelBack.Services.Feedback;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BotFeedbackController(
    IFeedbackService feedbackService)
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
    
    
    
}
