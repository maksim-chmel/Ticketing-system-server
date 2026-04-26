using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
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
    public async Task<ActionResult<List<FeedbackDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 50 : pageSize;
        pageSize = pageSize > 200 ? 200 : pageSize;

        logger.LogInformation("Fetching all feedbacks");
        var feedbacks = await feedbackService.GetAllFeedbacksAsync(page, pageSize);
        logger.LogInformation("Retrieved {Count} feedbacks", feedbacks.Count);
        return Ok(feedbacks);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateFeedbackStatusRequest request)
    {
        var updated = await feedbackService.UpdateStatus(id, request.Status);
        if (!updated)
            throw new NotFoundException($"Feedback with id={id} not found");

        return NoContent();
    }
}
