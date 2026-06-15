namespace AdminPanelBack.Services.Token;

public interface ITokenService
{
    string GenerateToken(string userId, string username, string? displayName, IList<string> roles);
}