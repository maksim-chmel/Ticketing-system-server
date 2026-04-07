using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.Models;

public class LoginRequest
{
    [Required]
    [MaxLength(100)]
    public required string Username { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Password { get; set; }
}
