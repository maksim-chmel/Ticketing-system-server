using AdminPanelBack.DTO;

namespace AdminPanelBack.Services;

public interface IFeedbackService
{
   public Task<List<FeedbackDto>> GetAllFeedbacksAsync();
   public Task UpdateStatus(int feedbackId , FeedbackStatus status);
 
}