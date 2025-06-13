using AdminPanelBack.Models;
using AdminPanelBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;

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