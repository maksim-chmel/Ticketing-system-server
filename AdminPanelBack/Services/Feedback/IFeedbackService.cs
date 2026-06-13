using AdminPanelBack.DTO;
using AdminPanelBack.Models;

namespace AdminPanelBack.Services.Feedback;

public interface IFeedbackService
{
   public Task<PagedResult<FeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize,CancellationToken cancellationToken =  default);
   public Task UpdateStatus(int feedbackId , FeedbackStatus status, CancellationToken cancellationToken = default);
   public Task CreateFeedbackAsync(UsersMessageDto dto, CancellationToken cancellationToken = default);
   Task<List<FeedbackDto>> GetAllUsersFeedbacksAsync(long userId, CancellationToken cancellationToken = default);
   Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync(CancellationToken cancellationToken = default);
  

}
