using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Feedback;

public class FeedbackService(IFeedbackRepository repository,IMapper mapper,ILogger<FeedbackService> logger, IUnitOfWork unitOfWork): IFeedbackService
{
    public async Task<List<FeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize,CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var feedbacks = await repository.GetFeedbacksPageAsync(skip, pageSize, cancellationToken);

        return mapper.Map<List<FeedbackDto>>(feedbacks);
    }
    public async Task<List<FeedbackDto>> GetAllUsersFeedbacksAsync(long userId, CancellationToken cancellationToken = default)
    {
        var feedbacks = await repository.GetUserFeedbacksAsync(userId, cancellationToken);

        return mapper.Map<List<FeedbackDto>>(feedbacks);
    }
    public async Task UpdateStatus(int feedbackId , FeedbackStatus status,CancellationToken cancellationToken =  default)
    {
        var feedback = await repository.FindAsyncById(feedbackId, cancellationToken);
        if (feedback == null)
        {
            logger.LogWarning($"Feedback not found: {feedbackId}");
            throw new NotFoundException($"Feedback not found: {feedbackId}");
        }
        feedback.Status = status;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Feedback updated: {feedback.Id}");

    }

    public async Task CreateFeedbackAsync(UsersMessageDto dto, CancellationToken cancellationToken = default)
    { 
        var feedback = mapper.Map<Models.Feedback>(dto);
        await repository.AddFeedbackAsync(feedback, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Feedback created: {feedback.Id}");
        
    }

    public async Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync(CancellationToken cancellationToken = default)
    {
        var newFeedbacks = await repository.PullUnsentToOperatorAsync(take: 100, cancellationToken: cancellationToken);
        logger.LogInformation("Successfully sent {Count} feedbacks to operator", newFeedbacks.Count);
        return mapper.Map<List<FeedbackDto>>(newFeedbacks);
    }

   
}
