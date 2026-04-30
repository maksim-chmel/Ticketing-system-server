using AdminPanelBack.DTO;
using AdminPanelBack.Services.Broadcast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/broadcast-messages")]
public class BroadcastController(IBroadcastMessageService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBroadcastMessageRequest request, CancellationToken cancellationToken)
    {
        await service.CreateBroadcastMessage(request.Message, cancellationToken);
        return NoContent();
    }
}