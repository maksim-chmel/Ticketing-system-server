using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Services.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/feedbacks")]
public class FeedbackController(IFeedbackService feedbackService,
    ILogger<FeedbackController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FeedbackDto>>> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default) 
    {
        
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        logger.LogInformation("Fetching feedbacks. Page: {Page}, Size: {Size}", page, pageSize);

      
        var feedbacks = await feedbackService.GetAllFeedbacksAsync(page, pageSize, cancellationToken);

        logger.LogInformation("Retrieved {Count} feedbacks", feedbacks.Count);
    
        return Ok(feedbacks);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateFeedbackStatusRequest request,CancellationToken cancellationToken)
    {
        await feedbackService.UpdateStatus(id, request.Status,cancellationToken);
        return NoContent();
    }
}
