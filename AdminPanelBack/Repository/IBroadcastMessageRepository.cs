using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IBroadcastMessageRepository: IRepository<BroadcastMessage>
{
    void AddBroadcastMessage(BroadcastMessage message);
    Task<List<BroadcastMessage>> GetActiveBroadcastMessagesToList(CancellationToken cancellationToken = default);
    void UpdateBroadcastMessage(BroadcastMessage broadcastMessage);
}
