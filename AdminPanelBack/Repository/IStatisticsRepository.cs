using AdminPanelBack.Models.Statistics;

namespace AdminPanelBack.Repository;

public interface IStatisticsRepository
{
    Task<List<StatusDistributionItem>> GetStatusDistributionAsync(CancellationToken cancellationToken = default);
    Task<List<RequestsOverTimeItem>> GetRequestsOverTimeAsync(CancellationToken cancellationToken = default);
}
