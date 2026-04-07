using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.Models;

public class BroadcastMessage
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(4000)]
    public string Message { get; set; } = string.Empty;
    
    public DateTime Created { get; set; }
    public bool IsActive { get; set; }
}
