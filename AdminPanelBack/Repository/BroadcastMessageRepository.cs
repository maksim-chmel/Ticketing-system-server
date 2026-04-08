using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public sealed class BroadcastMessageRepository(AppDbContext dbContext)
    : Repository<BroadcastMessage>(dbContext), IBroadcastMessageRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task CreateBroadcastMessage(BroadcastMessage message)
    {
        await _dbContext.BroadcastMessages.AddAsync(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<BroadcastMessage>> GetActiveBroadcastMessagesToList()
    {
        return await _dbContext.BroadcastMessages.Where(b => b.IsActive == true).ToListAsync();
    }

    public async Task UpdateBroadcastMessages(List<BroadcastMessage>  broadcastMessages)
    {
        if (broadcastMessages.Count == 0) return;
        _dbContext.BroadcastMessages.UpdateRange(broadcastMessages);
        await _dbContext.SaveChangesAsync();
       
    }
    
    
}