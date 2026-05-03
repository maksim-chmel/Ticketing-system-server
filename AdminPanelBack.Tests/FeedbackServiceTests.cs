using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Feedback;
using AutoMapper;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace AdminPanelBack.Tests;

public class FeedbackServiceTests
{
    private readonly Mock<IFeedbackRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly FeedbackService _service;

    public FeedbackServiceTests()
    {
        _mockRepo = new Mock<IFeedbackRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<List<FeedbackDto>>(It.IsAny<List<Feedback>>()))
            .Returns((List<Feedback> src) => src.Select(f => new FeedbackDto
            {
                Id = f.Id,
                UserId = f.UserId,
                Comment = f.Comment,
                Status = f.Status
            }).ToList());

        mockMapper.Setup(m => m.Map<Feedback>(It.IsAny<UsersMessageDto>()))
            .Returns((UsersMessageDto dto) => new Feedback
            {
                UserId = dto.UserId,
                Comment = dto.Comment
            });
    
        // Сервис использует репозиторий напрямую и unitOfWork только для сохранения
        _service = new FeedbackService(_mockRepo.Object, mockMapper.Object, NullLogger<FeedbackService>.Instance, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackExists_UpdatesStatusAndSaves()
    {
        // Arrange
        var feedback = new Feedback { Id = 1, Status = FeedbackStatus.Open };
        _mockRepo.Setup(r => r.FindAsyncById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);

        // Act
        await _service.UpdateStatus(1, FeedbackStatus.Done);

        // Assert
        feedback.Status.Should().Be(FeedbackStatus.Done);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepo.Setup(r => r.FindAsyncById(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feedback?)null);

        // Act
        var act = () => _service.UpdateStatus(99, FeedbackStatus.Done);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateFeedbackAsync_WhenCalled_AddsFeedbackAndSaves()
    {
        // Arrange
        var dto = new UsersMessageDto { UserId = 1, Comment = "New feedback" };
        _mockRepo.Setup(r => r.AddFeedbackAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()));

        // Act
        await _service.CreateFeedbackAsync(dto);

        // Assert
        _mockRepo.Verify(r => r.AddFeedbackAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenFeedbacksExist_ReturnsMappedDtos()
    {
        // Arrange
        var feedbacks = new List<Feedback>
        {
            new() { Id = 1, UserId = 100, Comment = "Great service" }
        };
        _mockRepo.Setup(r => r.GetFeedbacksPageAsync(0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedbacks);

        // Act
        var result = await _service.GetAllFeedbacksAsync(1, 50);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task GetAllUsersFeedbacksAsync_ReturnsMappedFeedbacksForUser()
    {
        var feedbacks = new List<Feedback>
        {
            new() { Id = 1, UserId = 42, Comment = "hello" },
            new() { Id = 2, UserId = 42, Comment = "world" }
        };
        _mockRepo.Setup(r => r.GetUserFeedbacksAsync(42, It.IsAny<CancellationToken>())).ReturnsAsync(feedbacks);

        var result = await _service.GetAllUsersFeedbacksAsync(42);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(f => f.UserId.Should().Be(42));
    }

    [Fact]
    public async Task GetNewFeedbacksForOperatorAsync_ReturnsPulledFeedbacks()
    {
        var feedbacks = new List<Feedback>
        {
            new() { Id = 1, UserId = 1, Comment = "test" }
        };
        _mockRepo.Setup(r => r.PullUnsentToOperatorAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(feedbacks);

        var result = await _service.GetNewFeedbacksForOperatorAsync();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(1);
    }
}