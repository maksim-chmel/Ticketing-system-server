using AdminPanelBack.Models;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services;

public class BroadcastMessageService(IBroadcastMessageRepository repository) : IBroadcastMessageService
{
    public async Task CreateBroadcastMessage(string message)
    {
        var newBroadcastMessage = new BroadcastMessage
        {
            Message = message,
            Created = DateTime.UtcNow,
            IsActive = true

        };
       await repository.CreateBroadcastMessage(newBroadcastMessage);
    }
}