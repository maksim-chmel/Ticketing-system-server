namespace AdminPanelBack.Models;

public class BroadcastMessage
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
    public bool IsActive { get; set; }
}