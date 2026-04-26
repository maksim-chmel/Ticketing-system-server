namespace AdminPanelBack.Models;

public class RefreshToken
{
    public int Id { get; set; }

    // Legacy: kept to avoid breaking already issued refresh tokens during rollout.
    // New tokens should not store the raw value here (we store TokenHash instead).
    public string Token { get; set; } = string.Empty;

    // Store only a hash of the refresh token. The raw token is only ever sent to the client.
    public string TokenHash { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
}
