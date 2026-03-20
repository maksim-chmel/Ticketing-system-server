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
    
}