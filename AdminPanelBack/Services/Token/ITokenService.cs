namespace AdminPanelBack.Services.Token;

public interface ITokenService
{
    string GenerateToken(string userId, string username, IList<string> roles);
}