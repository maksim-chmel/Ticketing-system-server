using AdminPanel.Models;
using AdminPanel.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

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
}