using AdminPanelBack.DTO;
using AdminPanelBack.Services.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AdminPanelBack.Controllers;

/// <summary>Feedback management.</summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/feedbacks")]
public class FeedbackController(
    IFeedbackService feedbackService,
    IOutputCacheStore outputCacheStore,
    ILogger<FeedbackController> logger) : ControllerBase
{
    /// <summary>Get a list of feedback requests (with pagination).</summary>
    /// <param name="page">The page number (minimum 1).</param>
    /// <param name="pageSize">The number of items per page (1–200).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">A list of feedback requests.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    [HttpGet]
    [OutputCache(PolicyName = "AdminFeedbacksPolicy")]
    [ProducesResponseType(typeof(PagedResult<FeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<FeedbackDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        logger.LogInformation("Fetching feedbacks. Page: {Page}, Size: {Size}", page, pageSize);
        var result = await feedbackService.GetAllFeedbacksAsync(page, pageSize, cancellationToken);
        logger.LogInformation("Retrieved {Count} feedbacks", result.Items.Count);
        return Ok(result);
    }

    /// <summary>Update the status of a feedback request.</summary>
    /// <param name="id">The feedback request identifier.</param>
    /// <param name="request">Request body containing the new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Status updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    /// <response code="404">Feedback request not found.</response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateFeedbackStatusRequest request, CancellationToken cancellationToken)
    {
        await feedbackService.UpdateStatus(id, request.Status, cancellationToken);
        await outputCacheStore.EvictByTagAsync("feedbacks", cancellationToken);
        await outputCacheStore.EvictByTagAsync("statistics", cancellationToken);
        return NoContent();
    }
}
