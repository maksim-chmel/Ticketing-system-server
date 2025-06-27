using AdminPanelBack.DTO;

namespace AdminPanelBack.Repository;

public interface IStatisticsRepository
{
    Task<List<StatusDistributionDto>> GetStatusDistributionAsync();
    Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync();
}