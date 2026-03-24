using AdminPanelBack.Models;
using AdminPanelBack.Services.Auth;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdminPanelBack.Tests;

public class AuthServiceTests
{
    private  readonly AuthService _service;
    private readonly FakeUserManager mockerUserManager;

    public AuthServiceTests()
    {
        mockerUserManager = new FakeUserManager();
        var mockerLogger = new Mock<ILogger<AuthService>>();
        _service = new AuthService(mockerLogger.Object, mockerUserManager);
        
    }

    [Fact]
    public async Task FindAdminByUsernameOrThrow_WhenUsernameIsEmpty_ThrowsArgumentException()
    {
       await _service.Invoking(s => s.FindAdminByUsernameOrThrow("")).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task FindAdminByUsernameOrThrow_WhenUserNotFound_ThrowsInvalidOperationException()
    {
        await _service.Invoking(s => s.FindAdminByUsernameOrThrow("invalid_username")).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FindAdminByUsernameOrThrow_WhenUserExists_ReturnsUser()
    {
       mockerUserManager.UserToReturn = new Admin{UserName = "admin"};
       var result =  await _service.FindAdminByUsernameOrThrow("admin");
        result.Should().NotBeNull();
        result.UserName.Should().Be("admin");
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenAdminIsNull_ThrowsArgumentNullException()
    {
       
        await _service.Invoking(s =>s.CheckPasswordOrThrow(null,"password")).Should().ThrowAsync<ArgumentNullException>();
      
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        await _service.Invoking(s=>s.CheckPasswordOrThrow(new Admin(),null)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenPasswordIsInvalid_ThrowsUnauthorizedException()
    {
        mockerUserManager.PasswordResult = false;
        await _service.Invoking(s =>s.CheckPasswordOrThrow(new Admin(),"password")).Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenEverythingIsOk_CompletesSuccessfully()
    {
        mockerUserManager.PasswordResult = true;
        await _service.Invoking(s => s.CheckPasswordOrThrow(new Admin(), "pass")).Should().NotThrowAsync();
    }
    
}