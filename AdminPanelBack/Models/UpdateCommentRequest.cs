namespace AdminPanelBack.Models;

public class UpdateCommentRequest
{
    public long UserId { get; set; }
    public string Comment { get; set; }
}