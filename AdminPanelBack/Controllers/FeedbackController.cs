using AdminPanelBack.DTO;
using AdminPanelBack.Services.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/feedbacks")]
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

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateFeedbackStatusRequest request)
    {
        var updated = await feedbackService.UpdateStatus(id, request.Status);
        if (!updated)
            return NotFound();

        return NoContent();
    }
}
