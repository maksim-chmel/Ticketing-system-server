namespace AdminPanelBack.Services.Login;

public interface ILoginService
{
    Task<(string accessToken, string refreshToken)> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<(string accessToken, string refreshToken, string userName)> RefreshTokensAsync(
        string currentRefreshToken, CancellationToken cancellationToken = default);
}