using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IFeedbackHistoryRepository : IRepository<FeedbackHistory, int>
{
    Task<List<FeedbackHistory>> GetByFeedbackIdAsync(int feedbackId, CancellationToken cancellationToken = default);
}