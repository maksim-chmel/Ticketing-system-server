using AdminPanelBack.DTO;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services.Statistic;

public class StatisticsService(IStatisticsRepository repository):IStatisticsService
{
    public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync()
    {
        var list = await repository.GetStatusDistributionAsync();
        return list.Select(x => new StatusDistributionDto
        {
            Name = x.Status.ToString(),
            Value = x.Count
        }).ToList();
    }
    
    public async Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync()
    {
        var list = await repository.GetRequestsOverTimeAsync();
        return list.Select(x => new TimeDisrtibutionDto
        {
            Date = x.Date.ToString("yyyy-MM-dd"),
            Value = x.Count
        }).ToList();
    }
}
