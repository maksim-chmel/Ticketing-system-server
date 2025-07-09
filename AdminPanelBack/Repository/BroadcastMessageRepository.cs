using AdminPanelBack.DB;
using AdminPanelBack.Models;
namespace AdminPanelBack.Repository;

public class BroadcastMessageRepository(AppDbContext dbContext)
    : Repository<BroadcastMessage>(dbContext), IBroadcastMessageRepository
{
    public async Task CreateBroadcastMessage(BroadcastMessage message)
    {
        await dbContext.BroadcastMessages.AddAsync(message);
        await dbContext.SaveChangesAsync();
    }
    
}