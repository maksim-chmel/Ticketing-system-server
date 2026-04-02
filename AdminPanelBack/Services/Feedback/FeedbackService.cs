using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Feedback;

public class FeedbackService(IFeedbackRepository repository,IMapper mapper,ILogger<FeedbackService> logger): IFeedbackService
{
    public async Task<List<FeedbackDto>> GetAllFeedbacksAsync()
    {
        var feedbacks =await repository.GetAllFeedbacksAsync();

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
        await repository.SaveChangesAsync();
        logger.LogInformation($"Feedback updated: {feedback.Id}");
        return true;

    }

    public async Task CreateFeedbackAsync(UsersMessageDto dto)
    { 
        var feedback = mapper.Map<Models.Feedback>(dto);
        await repository.AddFeedbackAsync(feedback);
        logger.LogInformation($"Feedback created: {feedback.Id}");
        
    }

    public async Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync()
    {
        var allFeedbacks = await repository.GetAllFeedbacksAsync();
        
        var newFeedbacks = allFeedbacks.Where(f => !f.IsSentToOperator).ToList();

        if (newFeedbacks.Count == 0)
        {
            logger.LogInformation("No new feedbacks found for operator");
            return new List<FeedbackDto>();
        }
        
        foreach (var feedback in newFeedbacks)
        {
            feedback.IsSentToOperator = true;
        }
        await repository.UpdateFeedbackAsync(newFeedbacks);
        logger.LogInformation("Successfully sent {Count} feedbacks to operator",newFeedbacks.Count);
        return mapper.Map<List<FeedbackDto>>(newFeedbacks);
    }

   
}