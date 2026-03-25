using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Feedback;

public class FeedbackService(IFeedbackRepository repository,IMapper mapper): IFeedbackService
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
    public async Task UpdateStatus(int feedbackId , FeedbackStatus status)
    {
        var feedback = await repository.FindAsyncById(feedbackId);
        if (feedback != null)
        {
            feedback.Status = status;
            await repository.SaveChangesAsync();
        }
    }

    public async Task CreateFeedbackAsync(UsersMessageDto dto)
    { 
        var feedback = mapper.Map<Models.Feedback>(dto);
        await repository.AddFeedbackAsync(feedback);
        
    }
}