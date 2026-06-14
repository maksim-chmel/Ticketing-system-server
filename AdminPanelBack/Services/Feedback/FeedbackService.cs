using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.Feedback;

public class FeedbackService(IFeedbackRepository repository, IMapper mapper, ILogger<FeedbackService> logger, IUnitOfWork unitOfWork) : IFeedbackService
{
    public async Task<PagedResult<FeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching feedbacks page {Page} with size {PageSize}", page, pageSize);
        var skip = (page - 1) * pageSize;
        var feedbacks = await repository.GetFeedbacksPageAsync(skip, pageSize, cancellationToken);
        var count = await repository.GetCountAsync(cancellationToken);
        logger.LogInformation("Retrieved {Count} feedbacks", feedbacks.Count);
        return new PagedResult<FeedbackDto> { Items = mapper.Map<List<FeedbackDto>>(feedbacks), TotalCount = count };
    }

    public async Task<List<FeedbackDto>> GetAllUsersFeedbacksAsync(long userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching feedbacks for user {UserId}", userId);
        var feedbacks = await repository.GetUserFeedbacksAsync(userId, cancellationToken);
        logger.LogInformation("Retrieved {Count} feedbacks for user {UserId}", feedbacks.Count, userId);
        return mapper.Map<List<FeedbackDto>>(feedbacks);
    }

    public async Task UpdateStatus(int feedbackId, FeedbackStatus status, CancellationToken cancellationToken = default)
    {
        var feedback = await repository.FindAsyncById(feedbackId, cancellationToken);
        if (feedback == null)
        {
            logger.LogWarning("Feedback not found: {FeedbackId}", feedbackId);
            throw new NotFoundException($"Feedback not found: {feedbackId}");
        }
        feedback.Status = status;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Feedback {FeedbackId} status updated to {Status}", feedback.Id, status);
    }

    public async Task CreateFeedbackAsync(UsersMessageDto dto, CancellationToken cancellationToken = default)
    {
        var feedback = mapper.Map<Models.Feedback>(dto);
        await repository.AddFeedbackAsync(feedback, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Feedback created with id {FeedbackId}", feedback.Id);
    }

    public async Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync(CancellationToken cancellationToken = default)
    {
        var newFeedbacks = await repository.PullUnsentToOperatorAsync(take: 100, cancellationToken: cancellationToken);
        logger.LogInformation("Successfully sent {Count} feedbacks to operator", newFeedbacks.Count);
        return mapper.Map<List<FeedbackDto>>(newFeedbacks);
    }
}
