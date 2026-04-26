using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class FeedbackRepository(AppDbContext dbContext) 
    : Repository<Feedback>(dbContext), IFeedbackRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<List<Feedback>> GetFeedbacksPageAsync(int skip, int take) =>
        _dbContext.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    public async Task<List<Feedback>> GetUserFeedbacksAsync(long userId)
    {
        return await _dbContext.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Where(f => f.UserId == userId) 
            .OrderByDescending(f => f.CreatedDate) 
            .ToListAsync();
    }

    public async Task AddFeedbackAsync(Feedback feedback)
    {
        await _dbContext.Feedbacks.AddAsync(feedback);
    }

    public void UpdateFeedback(Feedback feedback)
    {
        _dbContext.Feedbacks.Update(feedback);
    }

    public async Task<List<Feedback>> PullUnsentToOperatorAsync(int take)
    {
        // Используем атомарное обновление для предотвращения состояния гонки.
        // Обновляем IsSentToOperator для 'take' записей, которые еще не были отправлены.
        var updatedCount = await _dbContext.Feedbacks
            .Where(f => !f.IsSentToOperator)
            .OrderBy(f => f.CreatedDate)
            .Take(take)
            .ExecuteUpdateAsync(s => s
                .SetProperty(f => f.IsSentToOperator, true));

        if (updatedCount == 0)
            return new List<Feedback>();

        // Теперь получаем обновленные записи. Можно было бы возвращать только ID,
        // но для полной совместимости с предыдущим API, мы получаем полные объекты.
        var list = await _dbContext.Feedbacks
            .Include(f => f.User)
            .Where(f => f.IsSentToOperator && f.CreatedDate >= EF.Functions.DateAdd(EF.Functions.Now, "day", -1))
            .OrderByDescending(f => f.CreatedDate)
            .Take(take)
            .ToListAsync();

        return list;
    }
}
