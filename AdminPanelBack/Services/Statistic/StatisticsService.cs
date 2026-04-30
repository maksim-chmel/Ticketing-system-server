using AdminPanelBack.DTO;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services.Statistic;

public class StatisticsService(IStatisticsRepository repository):IStatisticsService
{
    public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync(CancellationToken cancellationToken = default)
    {
        var list = await repository.GetStatusDistributionAsync(cancellationToken);
        return list.Select(x => new StatusDistributionDto
        {
            Name = x.Status.ToString(),
            Value = x.Count
        }).ToList();
    }
    
    public async Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync(CancellationToken cancellationToken = default)
    {
        var list = await repository.GetRequestsOverTimeAsync(cancellationToken);
        return list.Select(x => new TimeDisrtibutionDto
        {
            Date = x.Date.ToString("yyyy-MM-dd"),
            Value = x.Count
        }).ToList();
    }
}
