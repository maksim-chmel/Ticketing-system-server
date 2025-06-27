using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class StatisticsRepository(AppDbContext context) : IStatisticsRepository
{
    public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync()
    {
       return await context.Feedbacks
            .GroupBy(t => t.Status)
            .Select(g => new StatusDistributionDto
            {
                Name = g.Key.ToString(),
                Value = g.Count()
            })
            .ToListAsync();
    }
    public Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync()
    {
        var result=  context.Feedbacks
            .AsEnumerable() 
            .GroupBy(t => t.CreatedDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new TimeDisrtibutionDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Value = g.Count()
            })
            .ToList(); 
        return Task.FromResult(result);
    }
}