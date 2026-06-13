using AdminPanelBack.DTO;

namespace AdminPanelBack.Services.Statistic;

public interface IStatisticsService
{
    public Task<List<StatusDistributionDto>> GetStatusDistributionAsync(CancellationToken cancellationToken = default);
    public Task<List<TimeDistributionDto>> GetRequestsOverTimeAsync(CancellationToken cancellationToken = default);
}
