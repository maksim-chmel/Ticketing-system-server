using AdminPanelBack.Models.Statistics;

namespace AdminPanelBack.Repository;

public interface IStatisticsRepository
{
    Task<List<StatusDistributionItem>> GetStatusDistributionAsync();
    Task<List<RequestsOverTimeItem>> GetRequestsOverTimeAsync();
}
