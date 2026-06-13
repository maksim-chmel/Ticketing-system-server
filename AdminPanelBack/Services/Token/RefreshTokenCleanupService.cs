using System.Linq.Expressions;
using AdminPanelBack.Models;
using AdminPanelBack.Services.Background;

namespace AdminPanelBack.Services.Token;

public class RefreshTokenCleanupService(IServiceScopeFactory serviceScopeFactory,
    ILogger<RefreshTokenCleanupService> logger)
    :CleanupService<RefreshToken>(serviceScopeFactory, logger)
{
    public override Expression<Func<RefreshToken, bool>> GetPredicate() 
        => token => token.ExpiresAt < DateTime.UtcNow
        || token.IsRevoked; 
}