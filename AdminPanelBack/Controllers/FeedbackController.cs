using AdminPanelBack.Models;
using AdminPanelBack.Services;
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
        logger.LogInformation("Запрос на получение всех отзывов");
        try
        {
            var feedbacks = await feedbackService.GetAllFeedbacksAsync();
            logger.LogInformation("Получено {Count} отзывов", feedbacks.Count);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении отзывов");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost("update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] FeedbackStatus status)
    {
        logger.LogInformation("Запрос на обновление статуса отзыва с Id={Id} на {Status}", id, status);
        try
        {
            await feedbackService.UpdateStatus(id, status);
            logger.LogInformation("Статус отзыва с Id={Id} успешно обновлен на {Status}", id, status);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении статуса отзыва с Id={Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}