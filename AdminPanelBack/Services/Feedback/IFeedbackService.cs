using AdminPanelBack.DTO;
using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Feedback;

public interface IFeedbackService
{
   public Task<PagedResult<FeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize,CancellationToken cancellationToken =  default);

   Task UpdateStatus(int feedbackId, FeedbackStatus status, string adminId, string adminName,
      CancellationToken cancellationToken = default);
   public Task CreateFeedbackAsync(UsersMessageDto dto, CancellationToken cancellationToken = default);
   Task<List<FeedbackDto>> GetAllUsersFeedbacksAsync(long userId, CancellationToken cancellationToken = default);
   Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync(CancellationToken cancellationToken = default);
   Task ClaimAsync(int feedbackId, string adminId, string adminName, CancellationToken cancellationToken = default);
}
