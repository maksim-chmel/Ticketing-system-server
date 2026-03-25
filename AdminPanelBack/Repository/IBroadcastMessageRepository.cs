using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IBroadcastMessageRepository: IRepository<BroadcastMessage>
{
    public Task CreateBroadcastMessage(BroadcastMessage message);
    Task<List<BroadcastMessage>> GetActiveBroadcastMessagesToList();
    Task UpdateBroadcastMessages(List<BroadcastMessage> broadcastMessages);
}