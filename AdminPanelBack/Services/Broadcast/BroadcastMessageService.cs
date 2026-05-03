using AdminPanelBack.Models;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services.Broadcast;

public class BroadcastMessageService(IBroadcastMessageRepository repository, ILogger<BroadcastMessageService> logger, IUnitOfWork unitOfWork) : IBroadcastMessageService
{
    public async Task CreateBroadcastMessage(string message, CancellationToken cancellationToken = default)
    {
        var newBroadcastMessage = new BroadcastMessage
        {
            Message = message,
            Created = DateTime.UtcNow,
            IsActive = true
        };
        repository.AddBroadcastMessage(newBroadcastMessage);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Broadcast message created: {Message}", newBroadcastMessage.Message);
    }

    public async Task<List<BroadcastMessage>> GetActiveBroadcastMessagesAndMakeInactive(CancellationToken cancellationToken = default)
    {
        var list = await repository.GetActiveBroadcastMessagesToList(cancellationToken);
        foreach (var msg in list)
        {
            msg.IsActive = false;
            repository.UpdateBroadcastMessage(msg);
        }
        if (list.Count > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        logger.LogInformation("Deactivated {Count} broadcast messages", list.Count);
        return list;
    }
}
