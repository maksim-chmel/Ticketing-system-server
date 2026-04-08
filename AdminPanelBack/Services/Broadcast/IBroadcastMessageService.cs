using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Broadcast;

public interface IBroadcastMessageService
{
    Task CreateBroadcastMessage(string message);
    Task<List<BroadcastMessage>> GetActiveBroadcastMessagesAndMakeInactive();
}