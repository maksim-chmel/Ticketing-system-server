using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPanel.Models;

public class Feedback
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; } =DateTime.Now;
    public int Status { get; set; } = 1;
    public bool IsDone { get; set; }
}