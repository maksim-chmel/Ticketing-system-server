using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.User;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AdminPanelBack.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();

        _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserDto>()))
            .Returns((UserDto dto) => new User { UserId = dto.UserId, Phone = "123", FirstName = "Test" });

        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns((User u) => new UserDto { UserId = u.UserId });

        _mockMapper.Setup(m => m.Map<List<UserDto>>(It.IsAny<List<User>>()))
            .Returns((List<User> src) => src.Select(u => new UserDto { UserId = u.UserId }).ToList());

        _service = new UserService(_mockRepo.Object, _mockMapper.Object, NullLogger<UserService>.Instance, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task ManageComment_WhenUserNotFound_ThrowsNotFoundException()
    {
        _mockRepo.Setup(r => r.FindAsyncById(99L, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await _service.Invoking(s => s.ManageComment(99, "comment")).Should().ThrowAsync<NotFoundException>();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ManageComment_WhenUserExists_UpdatesCommentAndSaves()
    {
        var user = new User { UserId = 1, Phone = "123", FirstName = "Test" };
        _mockRepo.Setup(r => r.FindAsyncById(1L, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        await _service.ManageComment(1, "new comment");

        user.Comments.Should().Be("new comment");
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrationNewUser_WhenUserExists_UpdatesWithoutCreating()
    {
        var existing = new User { UserId = 42, Phone = "old", FirstName = "Old" };
        var dto = new UserDto { UserId = 42 };
        _mockRepo.Setup(r => r.FindAsyncById(42L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        await _service.RegistrationNewUser(dto);

        _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrationNewUser_WhenUserNotFound_CreatesAndSaves()
    {
        var dto = new UserDto { UserId = 99 };
        _mockRepo.Setup(r => r.FindAsyncById(99L, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await _service.RegistrationNewUser(dto);

        _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ReturnsMappedDto()
    {
        var user = new User { UserId = 5, Phone = "555", FirstName = "John" };
        _mockRepo.Setup(r => r.FindAsyncById(5L, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetUserById(5);

        result.Should().NotBeNull();
        result!.UserId.Should().Be(5);
    }

    [Fact]
    public async Task GetUserById_WhenUserNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.FindAsyncById(99L, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _service.GetUserById(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task IsUserExists_WhenUserFound_ReturnsTrue()
    {
        var user = new User { UserId = 1, Phone = "123", FirstName = "Test" };
        _mockRepo.Setup(r => r.FindAsyncById(1L, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.IsUserExists(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsUserExists_WhenUserNotFound_ReturnsFalse()
    {
        _mockRepo.Setup(r => r.FindAsyncById(99L, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _service.IsUserExists(99);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllUsers_ReturnsMappedPage()
    {
        var users = new List<User>
        {
            new() { UserId = 1, Phone = "111", FirstName = "A" },
            new() { UserId = 2, Phone = "222", FirstName = "B" }
        };
        _mockRepo.Setup(r => r.GetUsersPageAsync(0, 10, It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mockRepo.Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);

        var result = await _service.GetAllUsers(1, 10);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items[0].UserId.Should().Be(1);
        result.Items[1].UserId.Should().Be(2);
    }
}
