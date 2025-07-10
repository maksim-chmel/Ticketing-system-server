using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Services;
using AdminPanelBack.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService service,
    ILogger<UserController> logger) : ControllerBase
{
    [HttpGet("users-to-list")]
    public async Task<IActionResult> GetUsersToList()
    {
        logger.LogInformation("Запрос на получение списка пользователей");
        try
        {
            var result = await service.GetAllUsers();
            logger.LogInformation("Получено {Count} пользователей", result?.Count() ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении списка пользователей");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost("update-comment")]
    public async Task<IActionResult> UpdateUserComment([FromBody] UpdateCommentRequest request)
    {
        logger.LogInformation("Запрос на обновление комментария пользователя с Id {UserId}", request.UserId);
        try
        {
            var result = await service.ManageComment(request.UserId, request.Comment);
            logger.LogInformation("Комментарий пользователя с Id {UserId} обновлен успешно", request.UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении комментария пользователя с Id {UserId}", request.UserId);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}