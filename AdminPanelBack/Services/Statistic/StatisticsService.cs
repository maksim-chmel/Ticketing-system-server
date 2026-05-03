using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Statistic;

public class StatisticsService(IStatisticsRepository repository,IMapper mapper):IStatisticsService
{
    public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync(CancellationToken cancellationToken = default)
    {
        var date = await repository.GetStatusDistributionAsync(cancellationToken);
        return mapper.Map<List<StatusDistributionDto>>(date);
    }
    
    public async Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync(CancellationToken cancellationToken = default)
    {
        var list = await repository.GetRequestsOverTimeAsync(cancellationToken);
        return mapper.Map<List<TimeDisrtibutionDto>>(list);
    }
}
