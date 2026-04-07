using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Services.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FeedbackController(IFeedbackService feedbackService,
    ILogger<FeedbackController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FeedbackDto>>> GetAll()
    {
        logger.LogInformation("Fetching all feedbacks");
        var feedbacks = await feedbackService.GetAllFeedbacksAsync();
        logger.LogInformation("Retrieved {Count} feedbacks", feedbacks.Count);
        return Ok(feedbacks);
    }

    [HttpPost("update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] FeedbackStatus status)
    {
        var updated = await feedbackService.UpdateStatus(id, status);
        if (!updated)
            return NotFound($"Feedback with id {id} not found");

        return Ok();
    }
}
