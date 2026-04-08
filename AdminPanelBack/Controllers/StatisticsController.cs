using AdminPanelBack.Services.Statistic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/statistics")]
public class StatisticsController(IStatisticsService service, ILogger<StatisticsController> logger)
    : ControllerBase
{
    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution()
    {
        logger.LogInformation("Fetching status distribution");
        var result = await service.GetStatusDistributionAsync();
        logger.LogInformation("Status distribution retrieved successfully, count: {Count}", result.Count);
        return Ok(result);
    }

    [HttpGet("requests-over-time")]
    public async Task<IActionResult> GetRequestsOverTime()
    {
        logger.LogInformation("Fetching requests over time");
        var result = await service.GetRequestsOverTimeAsync();
        logger.LogInformation("Requests over time retrieved successfully, count: {Count}", result.Count);
        return Ok(result);
    }
}