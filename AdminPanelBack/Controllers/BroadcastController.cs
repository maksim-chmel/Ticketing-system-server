using AdminPanelBack.DTO;
using AdminPanelBack.Services.Broadcast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;

/// <summary>Broadcast message management for the bot.</summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/broadcast-messages")]
public class BroadcastController(IBroadcastMessageService service) : ControllerBase
{
    /// <summary>Create a new broadcast message.</summary>
    /// <remarks>The message is marked as active and will be delivered by the bot during the next polling cycle.</remarks>
    /// <param name="request">Request body containing the message text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Broadcast message created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Access denied.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateBroadcastMessageRequest request, CancellationToken cancellationToken)
    {
        await service.CreateBroadcastMessage(request.Message, cancellationToken);
        return NoContent();
    }
}
