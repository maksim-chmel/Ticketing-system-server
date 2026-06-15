using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Hubs;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace AdminPanelBack.Services.Feedback;

public class FeedbackService(IFeedbackRepository repository, IMapper mapper, ILogger<FeedbackService> logger,
    IUnitOfWork unitOfWork, IFeedbackHistoryService historyService,
    IHubContext<FeedbackHub> hubContext) : IFeedbackService
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

    public async Task UpdateStatus(int feedbackId, FeedbackStatus status,string adminId,string adminName,
        CancellationToken cancellationToken = default)
    {
        var feedback = await repository.FindAsyncById(feedbackId, cancellationToken);
        if (feedback == null)
        {
            logger.LogWarning("Feedback not found: {FeedbackId}", feedbackId);
            throw new NotFoundException($"Feedback not found: {feedbackId}");
        }
        var oldStatus = feedback.Status.ToString();
        feedback.Status = status;
        await historyService.AddAsync(feedbackId, adminId, adminName, FeedbackHistoryAction.StatusChanged,
            oldStatus, status.ToString(), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Feedback {FeedbackId} status updated to {Status}", feedback.Id, status);
    }

    public async Task CreateFeedbackAsync(UsersMessageDto dto, CancellationToken cancellationToken = default)
    {
        var feedback = mapper.Map<Models.Feedback>(dto);
        await repository.AddFeedbackAsync(feedback, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Feedback created with id {FeedbackId}", feedback.Id);
        var feedbackWithUser = await repository.FindByIdWithUserAsync(feedback.Id, cancellationToken);
        await hubContext.Clients.All.SendAsync("newFeedback", mapper.Map<FeedbackDto>(feedbackWithUser), cancellationToken);
    }

    public async Task<List<FeedbackDto>> GetNewFeedbacksForOperatorAsync(CancellationToken cancellationToken = default)
    {
        var newFeedbacks = await repository.PullUnsentToOperatorAsync(take: 100, cancellationToken: cancellationToken);
        logger.LogInformation("Successfully sent {Count} feedbacks to operator", newFeedbacks.Count);
        return mapper.Map<List<FeedbackDto>>(newFeedbacks);
    }

    public async Task ClaimAsync(int feedbackId,string adminId,string adminName, CancellationToken cancellationToken = default)
    {
        var feedback = await repository.FindAsyncById(feedbackId, cancellationToken);
        if (feedback == null)
        {
            logger.LogWarning("Feedback not found: {FeedbackId}", feedbackId);
            throw new NotFoundException($"Feedback not found: {feedbackId}");
        }
        feedback.AssignedAdminId = adminId;
        feedback.AssignedAdminName = adminName;
        await historyService.AddAsync(feedbackId, adminId, adminName, FeedbackHistoryAction.Claimed,
            null, null, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Feedback {FeedbackId} assigned to admin {AdminId}", feedback.Id, adminId);
        
    }
}
