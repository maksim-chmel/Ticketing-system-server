using AdminPanelBack.DTO;

namespace AdminPanelBack.Services.Statistic;

public interface IStatisticsService
{
    public Task<List<StatusDistributionDto>> GetStatusDistributionAsync();
    public Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync();
}