using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IFeedbackRepository: IRepository<Feedback>
{
    Task<List<Feedback>> GetFeedbacksPageAsync(int skip, int take);
    Task AddFeedbackAsync(Feedback feedback);
    Task<List<Feedback>> GetUserFeedbacksAsync(long userId);
    void UpdateFeedback(Feedback feedback);
    Task<List<Feedback>> PullUnsentToOperatorAsync(int take);
}
