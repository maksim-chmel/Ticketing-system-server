namespace AdminPanelBack.Services.TokenServices;

public interface ITokenService
{
    public string GenerateToken(string userId, string username);
    
}