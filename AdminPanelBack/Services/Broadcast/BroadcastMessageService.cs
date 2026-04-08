using AdminPanelBack.Models;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services.Broadcast;

public class BroadcastMessageService(IBroadcastMessageRepository repository,ILogger<BroadcastMessageService> logger) : IBroadcastMessageService
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
       logger.LogInformation($"Broadcast message created: {newBroadcastMessage.Message}");
    }

    public async Task<List<BroadcastMessage>> GetActiveBroadcastMessagesAndMakeInactive()
    {
        var list = await repository.GetActiveBroadcastMessagesToList();
        foreach (var msg  in list)
        {
            msg.IsActive = false;
        }
        await repository.UpdateBroadcastMessages(list);
        logger.LogInformation("Deactivated {Count} broadcast messages", list.Count);
        return list;
       
    }
    
}