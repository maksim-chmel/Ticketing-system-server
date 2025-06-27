using AdminPanelBack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController(IStatisticsService service) : ControllerBase
{
    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution()
    {
       var result = await service.GetStatusDistributionAsync();
        return Ok(result);
    }
    
    [HttpGet("requests-over-time")]
    public async Task<IActionResult> GetRequestsOverTimeAsync()
    {
       var result =  await service.GetRequestsOverTimeAsync();
        return Ok(result);
    }
}