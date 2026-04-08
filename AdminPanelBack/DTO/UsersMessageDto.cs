using AdminPanelBack.Services.Feedback;
using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.DTO;

public sealed class UsersMessageDto
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Required]
    [MaxLength(4000)]
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public FeedbackStatus Status { get; set; } = 0;

}