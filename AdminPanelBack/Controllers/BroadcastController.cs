using AdminPanelBack.Models;
using AdminPanelBack.Services.Broadcast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BroadcastController(IBroadcastMessageService service) : ControllerBase
{
    [HttpPost("add-broadcastMessage")]
    public async Task<IActionResult> AddBroadcastMessage([FromBody] BroadcastMessage request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty");
        }

        await service.CreateBroadcastMessage(request.Message);
        return Ok("Message has been created");
    }
}