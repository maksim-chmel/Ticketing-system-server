using AdminPanelBack.Services.Token;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdminPanelBack.Tests;

public class TokenServiceTests
{
    private readonly TokenService _service;
    public TokenServiceTests()
    {
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "super-secret-key-for-testing-32-chars!!");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "test-issuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test-audience");
        Environment.SetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES", "60");

        var mockLogger = new Mock<ILogger<TokenService>>();
        _service = new TokenService(mockLogger.Object);
    }
    
    [Fact]
    public void GenerateToken_WithValidData_ReturnsNonEmptyToken()
    {
        var result = _service.GenerateToken("test", "test");
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ForDifferentUsers_ReturnsUniqueTokens()
    {
        var token1 = _service.GenerateToken("test1", "test1");
        var token2 = _service.GenerateToken("test2", "test2");
        token1.Should().NotBe(token2);
        
    }

    [Fact]
    public void GenerateToken_ReturnsTokenWithMinimumLength()
    {
        var result = _service.GenerateToken("test", "test");
        result.Length.Should().BeGreaterThan(10);
    }

    
}