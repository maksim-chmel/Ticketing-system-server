using AdminPanelBack.Services.Statistic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController(IStatisticsService service, ILogger<StatisticsController> logger)
    : ControllerBase
{
    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution()
    {
        logger.LogInformation("Fetching status distribution");
        try
        {
            var result = await service.GetStatusDistributionAsync();
            logger.LogInformation("Status distribution retrieved successfully, count: {Count}", result.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving status distribution");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("requests-over-time")]
    public async Task<IActionResult> GetRequestsOverTimeAsync()
    {
        logger.LogInformation("Fetching requests over time");
        try
        {
            var result = await service.GetRequestsOverTimeAsync();
            logger.LogInformation("Requests over time retrieved successfully, count: {Count}", result.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving requests over time");
            return StatusCode(500, "Internal server error");
        }
    }
}