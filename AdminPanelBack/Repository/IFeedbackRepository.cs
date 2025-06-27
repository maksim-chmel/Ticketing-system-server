using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IFeedbackRepository: IRepository<Feedback>
{
    Task<List<Feedback>> GetAllFeedbacksAsync();
}