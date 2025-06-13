using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Models;

public class User
{
    public long UserId { get; set; }
    public string Phone { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public List<Feedback> Feedbacks { get; set; }
}