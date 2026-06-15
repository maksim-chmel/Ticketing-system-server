using AdminPanelBack.DTO;
using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Feedback;

public interface IFeedbackHistoryService
{
    Task AddAsync(int feedbackId, string adminId, string adminName, FeedbackHistoryAction action, string? oldValue,
        string? newValue, CancellationToken cancellationToken = default);

    Task<List<FeedbackHistoryDto>> GetByFeedbackIdAsync(int feedbackId, CancellationToken cancellationToken = default);
}