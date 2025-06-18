using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Services;

public class StatisticsService(AppDbContext context):IStatisticsService
{
    public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync()
    {
        var result = await context.Feedbacks
            .GroupBy(t => t.Status)
            .Select(g => new StatusDistributionDto
            {
                Name = g.Key.ToString(),
                Value = g.Count()
            })
            .ToListAsync();

        return result;
    }
    
    public Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync()
    {
        var result=  context.Feedbacks
            .AsEnumerable() // переключаемся на выполнение на стороне клиента
            .GroupBy(t => t.CreatedDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new TimeDisrtibutionDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Value = g.Count()
            })
            .ToList(); // или ToList() — AsEnumerable() уже делает это клиентским LINQ
        return Task.FromResult(result);
    }
}