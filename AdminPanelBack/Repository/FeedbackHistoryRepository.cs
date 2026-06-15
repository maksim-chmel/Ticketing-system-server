using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class FeedbackHistoryRepository(AppDbContext context) : Repository<FeedbackHistory, int>(context), IFeedbackHistoryRepository
{
    public async Task<List<FeedbackHistory>> GetByFeedbackIdAsync(int feedbackId, CancellationToken cancellationToken = default)
    {
        return await Context.FeedbackHistories.Where(h => h.FeedbackId == feedbackId)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}