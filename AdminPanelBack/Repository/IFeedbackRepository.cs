using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IFeedbackRepository: IRepository<Feedback>
{
    Task<List<Feedback>> GetAllFeedbacksAsync();
    Task AddFeedbackAsync(Feedback feedback);
    Task<List<Feedback>> GetUserFeedbacksAsync(long userId);
    Task UpdateFeedbackAsync(List<Feedback> feedback);
}