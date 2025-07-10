using AdminPanelBack.Services;
using AdminPanelBack.Services.Broadcast;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BroadcastController(IBroadcastMessageService service) : ControllerBase
{
    [HttpPost("add-broadcastMessage")]
    public async Task<IActionResult> AddBroadcastMessage([FromBody] string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest("Сообщение не может быть пустым.");
        }

        await service.CreateBroadcastMessage(message);
        return Ok("Сообщение успешно добавлено в очередь.");
    }
}