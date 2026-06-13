using AdminPanelBack.Models;
using AdminPanelBack.Services.Token;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AdminPanelBack.Tests;

public class RefreshTokenCleanupServiceTests
{
    private readonly Func<RefreshToken, bool> _predicate;

    public RefreshTokenCleanupServiceTests()
    {
        var service = new RefreshTokenCleanupService(
            new Mock<IServiceScopeFactory>().Object,
            NullLogger<RefreshTokenCleanupService>.Instance);

        _predicate = service.GetPredicate().Compile();
    }
    [Fact]
    public void Predicate_WhenTokenExpired_ReturnsTrue()
    {
        var token = new RefreshToken{ExpiresAt = DateTime.UtcNow.AddHours(-1),IsRevoked = true};
        var result = _predicate(token);
        result.Should().BeTrue();
        
    }
    [Fact]
    public void Predicate_WhenTokenRevoked_ReturnsTrue()
    { 
        var token = new RefreshToken{IsRevoked = true};
       var result = _predicate(token);
       result.Should().BeTrue();
    }
    [Fact]
    public void Predicate_WhenTokenActive_ReturnsFalse()
    {
        var token = new RefreshToken{ ExpiresAt = DateTime.UtcNow.AddHours(1),IsRevoked = false};
        var result = _predicate(token);
        result.Should().BeFalse();
    }
}
