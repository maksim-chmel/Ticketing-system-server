using AdminPanelBack.DTO;
using AdminPanelBack.Services.Statistic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;

/// <summary>Feedback statistics.</summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/statistics")]
public class StatisticsController(IStatisticsService service, ILogger<StatisticsController> logger) : ControllerBase
{
    /// <summary>Get the distribution of feedback requests by status.</summary>
    /// <response code="200">A list of "status / count" pairs.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    [HttpGet("status-distribution")]
    [ProducesResponseType(typeof(List<StatusDistributionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStatusDistribution(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching status distribution");
        var result = await service.GetStatusDistributionAsync(cancellationToken);
        logger.LogInformation("Status distribution retrieved successfully, count: {Count}", result.Count);
        return Ok(result);
    }

    /// <summary>Get feedback request dynamics over time.</summary>
    /// <response code="200">A list of "date / count" pairs.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    [HttpGet("requests-over-time")]
    [ProducesResponseType(typeof(List<TimeDisrtibutionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRequestsOverTime(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching requests over time");
        var result = await service.GetRequestsOverTimeAsync(cancellationToken);
        logger.LogInformation("Requests over time retrieved successfully, count: {Count}", result.Count);
        return Ok(result);
    }
}
