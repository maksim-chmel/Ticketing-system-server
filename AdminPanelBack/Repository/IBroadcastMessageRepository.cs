using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IBroadcastMessageRepository: IRepository<BroadcastMessage>
{
    void AddBroadcastMessage(BroadcastMessage message);
    Task<List<BroadcastMessage>> PullActiveBroadcastMessagesAsync(CancellationToken cancellationToken = default);
}
