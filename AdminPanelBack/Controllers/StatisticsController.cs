using AdminPanelBack.Services;
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
        logger.LogInformation("Запрос на получение распределения статусов");
        try
        {
            var result = await service.GetStatusDistributionAsync();
            logger.LogInformation("Распределение статусов получено успешно, элементов: {Count}", result?.Count() ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении распределения статусов");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    [HttpGet("requests-over-time")]
    public async Task<IActionResult> GetRequestsOverTimeAsync()
    {
        logger.LogInformation("Запрос на получение статистики заявок во времени");
        try
        {
            var result = await service.GetRequestsOverTimeAsync();
            logger.LogInformation("Статистика заявок получена успешно, элементов: {Count}", result?.Count() ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении статистики заявок во времени");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}