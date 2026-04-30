using AdminPanelBack.Models;

namespace AdminPanelBack.DTO;

public sealed class UsersMessageDto
{
    public long UserId { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public FeedbackStatus Status { get; set; } = FeedbackStatus.Open;
}
