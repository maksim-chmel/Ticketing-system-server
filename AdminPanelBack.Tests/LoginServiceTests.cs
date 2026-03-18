using AdminPanelBack.Models;
using AdminPanelBack.Services.Auth;
using AdminPanelBack.Services.Login;
using AdminPanelBack.Services.Token;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            .ThrowAsync<UnauthorizedAccessException>();

    }
    [Fact]
    public async Task RefreshTokensAsync_WhenEverythingIsOk_ReturnsTokens()
    {
        // Arrange
        _fakeUserManager.UserToReturn = new Admin { Id = "123", UserName = "admin" };
        _mockRefreshTokenService.Setup(r => r.GetRefreshToken("valid_token"))
            .ReturnsAsync(new RefreshToken { UserId = "123" });

        // Act
        var result = await _service.RefreshTokensAsync("valid_token");

        // Assert
        result.userName.Should().Be("admin");
    }
    [Fact]
    public async Task RefreshTokensAsync_WhenUserIsNull_ThrowsUnauthorizedException()
    {
        _mockRefreshTokenService.Setup(r => r.GetRefreshToken("valid_token"))
            .ReturnsAsync(new RefreshToken { UserId = "123" });
        await _service.Invoking(s => s.RefreshTokensAsync("valid_token")).Should().ThrowAsync<UnauthorizedAccessException>();

    }
    
    
    public class FakeUserManager : UserManager<Admin>
    {
        public Admin? UserToReturn { get; set; }  

        public FakeUserManager() : base(
            new Mock<IUserStore<Admin>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<Admin>>().Object,
            new IUserValidator<Admin>[0],
            new IPasswordValidator<Admin>[0],
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<Admin>>>().Object)
        { }

        public override Task<Admin?> FindByIdAsync(string userId)
            => Task.FromResult(UserToReturn); 
    }
}