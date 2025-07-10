using AdminPanelBack.DTO;

namespace AdminPanelBack.Services.Feedback;

public interface IFeedbackService
{
   public Task<List<FeedbackDto>> GetAllFeedbacksAsync();
   public Task UpdateStatus(int feedbackId , FeedbackStatus status);
 
}