using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.Models;

public class FeedbackHistory
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public Feedback Feedback { get; set; } = null!;

    [MaxLength(450)]
    public string AdminId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string AdminName { get; set; } = string.Empty;

    public FeedbackHistoryAction Action { get; set; }

    [MaxLength(100)]
    public string? OldValue { get; set; }

    [MaxLength(100)]
    public string? NewValue { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
