using AdminPanelBack.DTO;
using AdminPanelBack.Repository;

namespace AdminPanelBack.Services.Statistic;

public class StatisticsService(IStatisticsRepository repository):IStatisticsService
{
    public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync()
    {
        return await repository.GetStatusDistributionAsync();
    }
    
    public async Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync()
    {
       return await repository.GetRequestsOverTimeAsync();
    }
}