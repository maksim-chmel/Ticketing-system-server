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
    [HttpPost("make-done/{feedbackId}")]
    public async Task<ActionResult> MakeDone(int feedbackId)
    {
       await feedbackService.MakeDone(feedbackId);
       return Ok();
    }
}