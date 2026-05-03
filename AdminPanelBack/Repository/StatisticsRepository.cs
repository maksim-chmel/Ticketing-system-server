using AdminPanelBack.DB;
using AdminPanelBack.Models.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class StatisticsRepository(AppDbContext context) : IStatisticsRepository
{
    public async Task<List<StatusDistributionItem>> GetStatusDistributionAsync(CancellationToken cancellationToken = default)
    {
       return await context.Feedbacks
            .GroupBy(t => t.Status)
            .Select(g => new StatusDistributionItem
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);
    }
    public Task<List<RequestsOverTimeItem>> GetRequestsOverTimeAsync(CancellationToken cancellationToken = default)
    {
        return GetRequestsOverTimeAsyncImpl(cancellationToken);
    }

    private Task<List<RequestsOverTimeItem>> GetRequestsOverTimeAsyncImpl(CancellationToken cancellationToken)
    {
        
        return context.Feedbacks
            .GroupBy(t => t.CreatedDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new RequestsOverTimeItem
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);
    }
}
