using AdminPanelBack.DTO;

namespace AdminPanelBack.Services.Feedback;

public interface IFeedbackService
{
   public Task<List<FeedbackDto>> GetAllFeedbacksAsync();
   public Task<bool> UpdateStatus(int feedbackId , FeedbackStatus status);
   public Task CreateFeedbackAsync(UsersMessageDto dto);
   Task<List<FeedbackDto>> GetAllUsersFeedbacksAsync(long userId);
   Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync();
  

}