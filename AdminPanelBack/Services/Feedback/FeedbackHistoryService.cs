using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Feedback;

public class FeedbackHistoryService(IFeedbackHistoryRepository historyRepository, IMapper mapper) : IFeedbackHistoryService
{
    public async Task AddAsync(int feedbackId, string adminId, string adminName, FeedbackHistoryAction action, string? oldValue,
        string? newValue, CancellationToken cancellationToken = default)
    {
        await historyRepository.AddAsync(new FeedbackHistory
        {
            FeedbackId = feedbackId,
            AdminId = adminId,
            AdminName = adminName,
            Action = action,
            OldValue = oldValue,
            NewValue = newValue,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task<List<FeedbackHistoryDto>> GetByFeedbackIdAsync(int feedbackId, CancellationToken cancellationToken = default)
    {
        var history = await historyRepository.GetByFeedbackIdAsync(feedbackId, cancellationToken);
        return mapper.Map<List<FeedbackHistoryDto>>(history);
    }
}
