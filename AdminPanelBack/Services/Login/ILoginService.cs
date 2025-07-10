namespace AdminPanelBack.Services;

public interface ILoginService
{ 
    public Task<(string accessToken, string refreshToken)> AuthenticateAsync(string username, string password);

    public Task<(string accessToken, string refreshToken, string userName)> RefreshTokensAsync(
        string currentRefreshToken);
}