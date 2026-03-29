using AdminPanelBack.Models;
using AdminPanelBack.Services;
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
    public async Task<ActionResult<List<Feedback>>> GetAll()
    {
        logger.LogInformation("Fetching all feedbacks");
        try
        {
            var feedbacks = await feedbackService.GetAllFeedbacksAsync();
            logger.LogInformation("Retrieved {Count} feedbacks", feedbacks.Count);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving feedbacks");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] FeedbackStatus status)
    {
        try
        {
            var feedback = await feedbackService.UpdateStatus(id,status);
            if (!feedback)
            {
                return NotFound($"Feedback with id {id} not found");
            }
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating status of feedback Id={Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
