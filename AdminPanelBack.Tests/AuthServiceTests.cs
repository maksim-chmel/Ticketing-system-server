using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Auth;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdminPanelBack.Tests;

public class AuthServiceTests
{
    private readonly AuthService _service;
    private readonly Mock<IAdminRepository> _mockAdminRepository;

    public AuthServiceTests()
    {
        _mockAdminRepository = new Mock<IAdminRepository>();
        var mockLogger = new Mock<ILogger<AuthService>>();
        _service = new AuthService(mockLogger.Object, _mockAdminRepository.Object);
    }

    [Fact]
    public async Task FindAdminByUsernameOrThrow_WhenUsernameIsEmpty_ThrowsValidationException()
    {
        await _service.Invoking(s => s.FindAdminByUsernameOrThrow("")).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task FindAdminByUsernameOrThrow_WhenUserNotFound_ThrowsUnauthorizedException()
    {
        _mockAdminRepository.Setup(r => r.FindByUsernameAsync("invalid_username")).ReturnsAsync((Admin?)null);

        await _service.Invoking(s => s.FindAdminByUsernameOrThrow("invalid_username")).Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task FindAdminByUsernameOrThrow_WhenUserExists_ReturnsUser()
    {
        _mockAdminRepository.Setup(r => r.FindByUsernameAsync("admin")).ReturnsAsync(new Admin { UserName = "admin" });

        var result = await _service.FindAdminByUsernameOrThrow("admin");

        result.Should().NotBeNull();
        result.UserName.Should().Be("admin");
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenAdminIsNull_ThrowsArgumentNullException()
    {
        await _service.Invoking(s => s.CheckPasswordOrThrow(null!, "password")).Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenPasswordIsEmpty_ThrowsValidationException()
    {
        await _service.Invoking(s => s.CheckPasswordOrThrow(new Admin(), null!)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenPasswordIsInvalid_ThrowsUnauthorizedException()
    {
        _mockAdminRepository.Setup(r => r.CheckPasswordAsync(It.IsAny<Admin>(), "password")).ReturnsAsync(false);

        await _service.Invoking(s => s.CheckPasswordOrThrow(new Admin(), "password")).Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task CheckPasswordOrThrow_WhenEverythingIsOk_CompletesSuccessfully()
    {
        _mockAdminRepository.Setup(r => r.CheckPasswordAsync(It.IsAny<Admin>(), "pass")).ReturnsAsync(true);

        await _service.Invoking(s => s.CheckPasswordOrThrow(new Admin(), "pass")).Should().NotThrowAsync();
    }
}
