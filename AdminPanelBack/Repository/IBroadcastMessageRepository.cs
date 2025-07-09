using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IBroadcastMessageRepository: IRepository<BroadcastMessage>
{
    public Task CreateBroadcastMessage(BroadcastMessage message);
}