using AdminPanelBack.Models;

namespace AdminPanelBack.Services;

public interface IFeedbackService
{
   public Task<List<FeedbackDto>> GetAllFeedbacksAsync();
   public Task MakeDone(int feedbackId);
}