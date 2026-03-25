using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class BroadcastMessageRepository(AppDbContext dbContext)
    : Repository<BroadcastMessage>(dbContext), IBroadcastMessageRepository
{
    public async Task CreateBroadcastMessage(BroadcastMessage message)
    {
        await dbContext.BroadcastMessages.AddAsync(message);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<BroadcastMessage>> GetActiveBroadcastMessagesToList()
    {
        return await dbContext.BroadcastMessages.Where(b => b.IsActive == true).ToListAsync();
    }

    public async Task UpdateBroadcastMessages(List<BroadcastMessage>  broadcastMessages)
    {
        if (broadcastMessages.Count == 0) return;
        dbContext.BroadcastMessages.UpdateRange(broadcastMessages);
        await dbContext.SaveChangesAsync();
       
    }
    
    
}