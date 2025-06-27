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
}