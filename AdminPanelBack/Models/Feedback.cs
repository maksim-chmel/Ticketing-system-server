using System.ComponentModel.DataAnnotations;
using AdminPanelBack.Services.Feedback;

namespace AdminPanelBack.Models;

public class Feedback
{
    public int Id { get; set; }
    public long UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    [Required]
    [MaxLength(4000)]
    public string Comment { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public FeedbackStatus Status { get; set; }
    public bool IsSentToOperator { get; set; } = false;
   
}
