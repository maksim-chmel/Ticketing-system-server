using AdminPanelBack.Models;
using AdminPanelBack.Services.Broadcast;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BroadcastController(IBroadcastMessageService service) : ControllerBase
{
    [HttpPost("add-broadcastMessage")]
    public async Task<IActionResult> AddBroadcastMessage([FromBody] BroadcastMessage request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Сообщение не может быть пустым.");
        }

        await service.CreateBroadcastMessage(request.Message);
        return Ok("Сообщение успешно добавлено в очередь.");
    }
}