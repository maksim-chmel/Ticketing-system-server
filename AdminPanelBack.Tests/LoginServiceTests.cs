using AdminPanelBack.Models;
using AdminPanelBack.Services.Auth;
using AdminPanelBack.Services.Login;
using AdminPanelBack.Services.Token;
using AdminPanelBack.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdminPanelBack.Tests;

public class LoginServiceTests
{
    private readonly FakeUserManager _fakeUserManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRefreshTokenService> _mockRefreshTokenService;
    private readonly Mock<ILogger<LoginService>> _mockLogger;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly LoginService _service;
    
    public LoginServiceTests()
    {
        _fakeUserManager = new FakeUserManager();
        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenService = new Mock<IRefreshTokenService>();
        _mockLogger = new Mock<ILogger<LoginService>>();
        _mockAuthService = new Mock<IAuthService>();

        _service = new LoginService(
            _fakeUserManager,
            _mockTokenService.Object,
            _mockRefreshTokenService.Object,
            _mockLogger.Object,
            _mockAuthService.Object);
    }

    [Fact]
    public async Task RefreshTokensAsync_WhenTokenIsNull_ThrowsUnauthorizedException()
    {
        _mockRefreshTokenService.Setup(r =>r.GetRefreshToken("invalid_token")).
            ReturnsAsync((RefreshToken?)null);
        await _service.Invoking(s => s.RefreshTokensAsync("invalid_token")).Should()
            .ThrowAsync<UnauthorizedException>();

    }
    [Fact]
    public async Task RefreshTokensAsync_WhenEverythingIsOk_ReturnsTokens()
    {
       
        _fakeUserManager.UserToReturn = new Admin { Id = "123", UserName = "admin" };
        _mockRefreshTokenService.Setup(r => r.GetRefreshToken("valid_token"))
            .ReturnsAsync(new RefreshToken { UserId = "123" });

       
        var result = await _service.RefreshTokensAsync("valid_token");

        
        result.userName.Should().Be("admin");
    }
    [Fact]
    public async Task RefreshTokensAsync_WhenUserIsNull_ThrowsUnauthorizedException()
    {
        _mockRefreshTokenService.Setup(r => r.GetRefreshToken("valid_token"))
            .ReturnsAsync(new RefreshToken { UserId = "123" });
        await _service.Invoking(s => s.RefreshTokensAsync("valid_token")).Should().ThrowAsync<UnauthorizedException>();

    }
    [Fact]
    public async Task AuthenticateAsync_WhenCredentialsAreValid_ReturnsTokens()
    {
        var user = new Admin { Id = "123", UserName = "admin" };
        _mockAuthService.Setup(a => a.FindAdminByUsernameOrThrow("admin"))
            .ReturnsAsync(user);
        _mockAuthService.Setup(a => a.CheckPasswordOrThrow(user, "password"))
            .Returns(Task.CompletedTask);
        _mockTokenService.Setup(t => t.GenerateToken(user.Id, user.UserName))
            .Returns("access_token");
        _mockRefreshTokenService.Setup(r => r.CreateRefreshTokenAsync(user.Id))
            .ReturnsAsync("refresh_token");

        var result = await _service.AuthenticateAsync("admin", "password");

        result.accessToken.Should().Be("access_token");
        result.refreshToken.Should().Be("refresh_token");
    }
    
   
}