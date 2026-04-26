using AdminPanelBack.DB;
using AdminPanelBack.Models.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class StatisticsRepository(AppDbContext context) : IStatisticsRepository
{
    public async Task<List<StatusDistributionItem>> GetStatusDistributionAsync()
    {
       return await context.Feedbacks
            .GroupBy(t => t.Status)
            .Select(g => new StatusDistributionItem
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync();
    }
    public Task<List<RequestsOverTimeItem>> GetRequestsOverTimeAsync()
    {
        return GetRequestsOverTimeAsyncImpl();
    }

    private Task<List<RequestsOverTimeItem>> GetRequestsOverTimeAsyncImpl()
    {
        // Keep aggregation in SQL. Date formatting is done in-memory after the DB aggregation.
        return context.Feedbacks
            .GroupBy(t => t.CreatedDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new RequestsOverTimeItem
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync();
    }
}
