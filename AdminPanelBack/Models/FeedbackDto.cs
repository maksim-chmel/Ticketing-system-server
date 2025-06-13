namespace AdminPanel.Models;

public class FeedbackDto
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string? Comment { get; set; }
    public string? Username { get; set; }
    public string? Phone { get; set; }
    public DateTime Date { get; set; }
    public int Status { get; set; }
}