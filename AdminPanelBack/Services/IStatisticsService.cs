using AdminPanelBack.DTO;
using AdminPanelBack.Models;

namespace AdminPanelBack.Services;

public interface IStatisticsService
{
    public Task<List<StatusDistributionDto>> GetStatusDistributionAsync();
    public Task<List<TimeDisrtibutionDto>> GetRequestsOverTimeAsync();
}