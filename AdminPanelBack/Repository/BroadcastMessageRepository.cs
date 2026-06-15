using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public sealed class BroadcastMessageRepository(AppDbContext dbContext)
    : Repository<BroadcastMessage, int>(dbContext), IBroadcastMessageRepository
{

    public void AddBroadcastMessage(BroadcastMessage message)
    {
        Context.BroadcastMessages.Add(message);
    }

    public async Task<List<BroadcastMessage>> PullActiveBroadcastMessagesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.BroadcastMessages
            .FromSqlRaw(@"UPDATE ""BroadcastMessages"" SET ""IsActive"" = false WHERE ""IsActive"" = true RETURNING *")
            .ToListAsync(cancellationToken);
    }
}
