namespace AdminPanelBack.Services.Token;

public interface ITokenService
{
    public string GenerateToken(string userId, string username);
    
}