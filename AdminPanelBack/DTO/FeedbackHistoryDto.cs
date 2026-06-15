using AdminPanelBack.Models;

namespace AdminPanelBack.DTO;

public class FeedbackHistoryDto
{
    public int Id { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public FeedbackHistoryAction Action { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; }
}
