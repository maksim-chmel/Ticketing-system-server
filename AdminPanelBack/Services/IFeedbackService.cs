using AdminPanel.Models;

namespace AdminPanel.Services;

public interface IFeedbackService
{
   public Task<List<FeedbackDto>> GetAllFeedbacksAsync();
}