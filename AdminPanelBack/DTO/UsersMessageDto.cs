using AdminPanelBack.Services.Feedback;

namespace AdminPanelBack.DTO;

public class UsersMessageDto
{
    public long UserId { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public FeedbackStatus Status { get; set; } = 0;

}