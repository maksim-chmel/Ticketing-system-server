using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Services;

public class FeedbackService(AppDbContext dbContext): IFeedbackService
{
    public async Task<List<FeedbackDto>> GetAllFeedbacksAsync()
    {
        var feedbacks = await dbContext.Feedbacks.Include(f => f.User).Select(f => new FeedbackDto
            {
                Id = f.Id,
                UserId = f.UserId,
                Comment = f.Comment,
                Username = f.User.Username,
                Phone = f.User.Phone,
                Date = f.CreatedDate,
                Status = f.Status,
            })
            .ToListAsync();;
       return feedbacks;
    }
    
    public async Task MakeDone(int feedbackId)
    {
        var feedback = await dbContext.Feedbacks.FindAsync(feedbackId);
        if (feedback != null)
        {
            feedback.IsDone = true;
            await dbContext.SaveChangesAsync();
        }
    }
}