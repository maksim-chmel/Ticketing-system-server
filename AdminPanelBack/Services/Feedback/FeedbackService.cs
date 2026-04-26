using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Feedback;

public class FeedbackService(IFeedbackRepository repository,IMapper mapper,ILogger<FeedbackService> logger, AppDbContext context): IFeedbackService
{
    public async Task<List<FeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var feedbacks = await repository.GetFeedbacksPageAsync(skip, pageSize);

        return mapper.Map<List<FeedbackDto>>(feedbacks);
    }
    public async Task<List<FeedbackDto>> GetAllUsersFeedbacksAsync(long userId)
    {
        var feedbacks = await repository.GetUserFeedbacksAsync(userId);

        return mapper.Map<List<FeedbackDto>>(feedbacks);
    }
    public async Task<bool> UpdateStatus(int feedbackId , FeedbackStatus status)
    {
        var feedback = await repository.FindAsyncById(feedbackId);
        if (feedback == null)
        {
            logger.LogError($"Feedback not found: {feedbackId}");
            return false;
        }
        feedback.Status = status;
        await context.SaveChangesAsync();
        logger.LogInformation($"Feedback updated: {feedback.Id}");
        return true;

    }

    public async Task CreateFeedbackAsync(UsersMessageDto dto)
    { 
        var feedback = mapper.Map<Models.Feedback>(dto);
        await repository.AddFeedbackAsync(feedback);
        await context.SaveChangesAsync();
        logger.LogInformation($"Feedback created: {feedback.Id}");
        
    }

    public async Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync()
    {
        var newFeedbacks = await repository.PullUnsentToOperatorAsync(take: 100);
        logger.LogInformation("Successfully sent {Count} feedbacks to operator", newFeedbacks.Count);
        return mapper.Map<List<FeedbackDto>>(newFeedbacks);
    }

   
}
