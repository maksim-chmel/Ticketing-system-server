using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.Models;

public class User
{
    public long UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public required string Phone { get; set; }

    [Required]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(100)]
    public string? Username { get; set; }

    public List<Feedback> Feedbacks { get; set; } = new();

    [MaxLength(2000)]
    public string? Comments { get; set; }
}