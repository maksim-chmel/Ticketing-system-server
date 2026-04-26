using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public sealed class BroadcastMessageRepository(AppDbContext dbContext)
    : Repository<BroadcastMessage>(dbContext), IBroadcastMessageRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public void AddBroadcastMessage(BroadcastMessage message)
    {
        _dbContext.BroadcastMessages.Add(message);
    }

    public async Task<List<BroadcastMessage>> GetActiveBroadcastMessagesToList()
    {
        return await _dbContext.BroadcastMessages.Where(b => b.IsActive).ToListAsync();
    }

    public void UpdateBroadcastMessage(BroadcastMessage broadcastMessage)
    {
        _dbContext.BroadcastMessages.Update(broadcastMessage);
    }
}