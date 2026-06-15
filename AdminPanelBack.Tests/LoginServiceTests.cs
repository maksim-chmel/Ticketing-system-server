using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Auth;
using AdminPanelBack.Services.Login;
using AdminPanelBack.Services.Token;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdminPanelBack.Tests;

public class LoginServiceTests
{
    private readonly Mock<IAdminRepository> _mockAdminRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRefreshTokenService> _mockRefreshTokenService;
    private readonly Mock<ILogger<LoginService>> _mockLogger;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly LoginService _service;

    public LoginServiceTests()
    {
        _mockAdminRepository = new Mock<IAdminRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenService = new Mock<IRefreshTokenService>();
        _mockLogger = new Mock<ILogger<LoginService>>();
        _mockAuthService = new Mock<IAuthService>();

        _service = new LoginService(
            _mockAdminRepository.Object,
            _mockTokenService.Object,
            _mockRefreshTokenService.Object,
            _mockLogger.Object,
            _mockAuthService.Object);
    }

    [Fact]
    public async Task RefreshTokensAsync_WhenTokenIsNull_ThrowsUnauthorizedException()
    {
        _mockRefreshTokenService
            .Setup(r => r.GetRefreshTokenAsync("invalid_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        await _service.Invoking(s => s.RefreshTokensAsync("invalid_token")).Should()
            .ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task RefreshTokensAsync_WhenEverythingIsOk_ReturnsTokens()
    {
        var user = new Admin { Id = "123", UserName = "admin" };
        _mockAdminRepository.Setup(r => r.FindByIdAsync("123")).ReturnsAsync(user);
        _mockAdminRepository.Setup(r => r.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        _mockRefreshTokenService
            .Setup(r => r.GetRefreshTokenAsync("valid_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshToken { UserId = "123" });

        var result = await _service.RefreshTokensAsync("valid_token");

        result.userName.Should().Be("admin");
    }

    [Fact]
    public async Task RefreshTokensAsync_WhenUserIsNull_ThrowsUnauthorizedException()
    {
        _mockAdminRepository.Setup(r => r.FindByIdAsync("123")).ReturnsAsync((Admin?)null);
        _mockRefreshTokenService
            .Setup(r => r.GetRefreshTokenAsync("valid_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshToken { UserId = "123" });

        await _service.Invoking(s => s.RefreshTokensAsync("valid_token")).Should()
            .ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task AuthenticateAsync_WhenCredentialsAreValid_ReturnsTokens()
    {
        var user = new Admin { Id = "123", UserName = "admin" };
        _mockAuthService.Setup(a => a.FindAdminByUsernameOrThrow("admin")).ReturnsAsync(user);
        _mockAuthService.Setup(a => a.CheckPasswordOrThrow(user, "password")).Returns(Task.CompletedTask);
        _mockAdminRepository.Setup(r => r.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        _mockTokenService
            .Setup(t => t.GenerateToken(user.Id, user.UserName!, It.IsAny<string>(), It.IsAny<IList<string>>()))
            .Returns("access_token");
        _mockRefreshTokenService
            .Setup(r => r.CreateRefreshTokenAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync("refresh_token");

        var result = await _service.AuthenticateAsync("admin", "password");

        result.accessToken.Should().Be("access_token");
        result.refreshToken.Should().Be("refresh_token");
    }
}
