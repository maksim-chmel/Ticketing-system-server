using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IFeedbackRepository: IRepository<Feedback, int>
{
    Task<List<Feedback>> GetFeedbacksPageAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task AddFeedbackAsync(Feedback feedback, CancellationToken cancellationToken = default);
    Task<List<Feedback>> GetUserFeedbacksAsync(long userId, CancellationToken cancellationToken = default);
    void UpdateFeedback(Feedback feedback);
    Task<Feedback?> FindByIdWithUserAsync(int id, CancellationToken cancellationToken = default);
}
