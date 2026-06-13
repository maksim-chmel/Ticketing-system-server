using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Token;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AdminPanelBack.Tests;

public class RefreshTokenServiceTests
{
    private readonly Mock<IRefreshTokenRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly RefreshTokenService _service;

    public RefreshTokenServiceTests()
    {
        _mockRepo = new Mock<IRefreshTokenRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _service = new RefreshTokenService(NullLogger<RefreshTokenService>.Instance, _mockRepo.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_ReturnsTokenAndSaves()
    {
        _mockRepo.Setup(r => r.CreateRefreshTokenAsync("user1", It.IsAny<CancellationToken>())).ReturnsAsync("token123");

        var result = await _service.CreateRefreshTokenAsync("user1");

        result.Should().Be("token123");
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WhenValid_ReturnsTrue()
    {
        _mockRepo.Setup(r => r.ValidateRefreshTokenAsync("token", "user1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.ValidateRefreshTokenAsync("token", "user1");

        result.Should().BeTrue();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WhenInvalid_ReturnsFalseAndDoesNotSave()
    {
        _mockRepo.Setup(r => r.ValidateRefreshTokenAsync("token", "user1", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.ValidateRefreshTokenAsync("token", "user1");

        result.Should().BeFalse();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_CallsRepoAndSaves()
    {
        await _service.RevokeRefreshTokenAsync("token");

        _mockRepo.Verify(r => r.RevokeRefreshTokenAsync("token", It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevokeAllUserTokensAsync_CallsRepoAndSaves()
    {
        await _service.RevokeAllUserTokensAsync("user1");

        _mockRepo.Verify(r => r.RevokeAllUserTokensAsync("user1", It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_WhenFound_ReturnsToken()
    {
        var token = new RefreshToken { UserId = "user1" };
        _mockRepo.Setup(r => r.GetRefreshTokenAsync("token", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var result = await _service.GetRefreshTokenAsync("token");

        result.Should().Be(token);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_WhenNotFound_ReturnsNullAndDoesNotSave()
    {
        _mockRepo.Setup(r => r.GetRefreshTokenAsync("invalid", It.IsAny<CancellationToken>())).ReturnsAsync((RefreshToken?)null);

        var result = await _service.GetRefreshTokenAsync("invalid");

        result.Should().BeNull();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
