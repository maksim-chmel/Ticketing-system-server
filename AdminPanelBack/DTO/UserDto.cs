namespace AdminPanelBack.DTO;

public sealed class UserDto
{
    public long UserId { get; set; }
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Comments { get; set; }
}