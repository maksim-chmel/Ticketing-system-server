using AdminPanelBack.DB;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services.Broadcast;

public class BroadcastMessageService(IBroadcastMessageRepository repository,ILogger<BroadcastMessageService> logger, AppDbContext context) : IBroadcastMessageService
{
    public async Task CreateBroadcastMessage(string message)
    {
        var newBroadcastMessage = new BroadcastMessage
        {
            Message = message,
            Created = DateTime.UtcNow,
            IsActive = true
        };
       repository.AddBroadcastMessage(newBroadcastMessage);
       await context.SaveChangesAsync();
       logger.LogInformation($"Broadcast message created: {newBroadcastMessage.Message}");
    }

    public async Task<List<BroadcastMessage>> GetActiveBroadcastMessagesAndMakeInactive()
    {
        var list = await repository.GetActiveBroadcastMessagesToList();
        foreach (var msg  in list)
        {
            msg.IsActive = false;
            repository.UpdateBroadcastMessage(msg);
        }
        if (list.Count > 0)
        {
            await context.SaveChangesAsync();
        }
        logger.LogInformation("Deactivated {Count} broadcast messages", list.Count);
        return list;
    }
}