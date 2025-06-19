using AdminPanelBack.Models;
using AdminPanelBack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]

[ApiController]
[Route("api/[controller]")]
public class FeedbackController(IFeedbackService feedbackService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Feedback>>> GetAll()
    {
        var feedbacks = await feedbackService.GetAllFeedbacksAsync();
        return Ok(feedbacks);
    }
    [HttpPost("update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] FeedbackStatus status)
    {
       await feedbackService.UpdateStatus(id , status);
       return Ok();
    }
}