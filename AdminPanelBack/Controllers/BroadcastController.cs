using AdminPanelBack.DTO;
using AdminPanelBack.Services.Broadcast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/broadcast-messages")]
public class BroadcastController(IBroadcastMessageService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBroadcastMessageRequest request)
    {
        await service.CreateBroadcastMessage(request.Message);
        return NoContent();
    }
}