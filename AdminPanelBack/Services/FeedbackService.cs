using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Services;

public class FeedbackService(AppDbContext dbContext): IFeedbackService
{
    public async Task<List<FeedbackDto>> GetAllFeedbacksAsync()
    {
        var feedbacks = await dbContext.Feedbacks
            .Include(f => f.User)
            .Select(f => new FeedbackDto
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
    
    public async Task UpdateStatus(int feedbackId , FeedbackStatus status)
    {
        var feedback = await dbContext.Feedbacks.FindAsync(feedbackId);
        if (feedback != null)
        {
            feedback.Status = status;
            await dbContext.SaveChangesAsync();
        }
    }
   
}