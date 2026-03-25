using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class FeedbackRepository(AppDbContext dbContext) 
    : Repository<Feedback>(dbContext), IFeedbackRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<List<Feedback>> GetAllFeedbacksAsync()
    {
        return await _dbContext.Feedbacks.Include(f => f.User).ToListAsync();
    }
    public async Task<List<Feedback>> GetUserFeedbacksAsync(long userId)
    {
        return await _dbContext.Feedbacks
            .Where(f => f.UserId == userId) 
            .OrderByDescending(f => f.CreatedDate) 
            .ToListAsync();
    }

    public async Task AddFeedbackAsync(Feedback feedback)
    {
        await _dbContext.Feedbacks.AddAsync(feedback);
        await  _dbContext.SaveChangesAsync();
    }
}