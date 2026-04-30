using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IFeedbackRepository: IRepository<Feedback>
{
    Task<List<Feedback>> GetFeedbacksPageAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task AddFeedbackAsync(Feedback feedback, CancellationToken cancellationToken = default);
    Task<List<Feedback>> GetUserFeedbacksAsync(long userId, CancellationToken cancellationToken = default);
    void UpdateFeedback(Feedback feedback);
    Task<List<Feedback>> PullUnsentToOperatorAsync(int take, CancellationToken cancellationToken = default);
}
