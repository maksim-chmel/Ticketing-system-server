using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class FeedbackRepository(AppDbContext dbContext, ILogger<FeedbackRepository> logger)
    : Repository<Feedback, int>(dbContext), IFeedbackRepository
{
    public Task<List<Feedback>> GetFeedbacksPageAsync(int skip, int take, CancellationToken cancellationToken = default) =>
        Context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<List<Feedback>> GetUserFeedbacksAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await Context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default) =>
        Context.Feedbacks.CountAsync(cancellationToken);

    public async Task AddFeedbackAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        await Context.Feedbacks.AddAsync(feedback, cancellationToken);
    }

    public Task<Feedback?> FindByIdWithUserAsync(int id, CancellationToken cancellationToken = default) =>
        Context.Feedbacks
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public void UpdateFeedback(Feedback feedback)
    {
        Context.Feedbacks.Update(feedback);
    }

    public async Task<List<Feedback>> PullUnsentToOperatorAsync(int take, CancellationToken cancellationToken = default)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var ids = await Context.Feedbacks
                .Where(f => !f.IsSentToOperator)
                .OrderBy(f => f.CreatedDate)
                .Take(take)
                .Select(f => f.Id)
                .ToListAsync(cancellationToken);

            if (ids.Count == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return new List<Feedback>();
            }

            await Context.Feedbacks
                .Where(f => ids.Contains(f.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(f => f.IsSentToOperator, true), cancellationToken);

            var list = await Context.Feedbacks
                .Include(f => f.User)
                .Where(f => ids.Contains(f.Id))
                .ToListAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return list;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pulling unsent feedbacks to operator");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
