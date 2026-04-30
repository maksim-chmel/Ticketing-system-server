using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Broadcast;

public interface IBroadcastMessageService
{
    Task CreateBroadcastMessage(string message, CancellationToken cancellationToken = default);
    Task<List<BroadcastMessage>> GetActiveBroadcastMessagesAndMakeInactive(CancellationToken cancellationToken = default);
}
