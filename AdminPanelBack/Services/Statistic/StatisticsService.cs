using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Statistic;

public class StatisticsService(IStatisticsRepository repository, IMapper mapper, ILogger<StatisticsService> logger) : IStatisticsService
{
    public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching status distribution statistics");
        var data = await repository.GetStatusDistributionAsync(cancellationToken);
        logger.LogInformation("Retrieved {Count} status distribution entries", data.Count);
        return mapper.Map<List<StatusDistributionDto>>(data);
    }

    public async Task<List<TimeDistributionDto>> GetRequestsOverTimeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching requests over time statistics");
        var list = await repository.GetRequestsOverTimeAsync(cancellationToken);
        logger.LogInformation("Retrieved {Count} time distribution entries", list.Count);
        return mapper.Map<List<TimeDistributionDto>>(list);
    }
}
