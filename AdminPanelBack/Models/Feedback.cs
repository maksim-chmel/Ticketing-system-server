using AdminPanelBack.Services.Feedback;

namespace AdminPanelBack.Models;

public class Feedback
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; } =DateTime.Now;
    public FeedbackStatus Status { get; set; }
    public bool IsSentToOperator { get; set; } = false;
   
}