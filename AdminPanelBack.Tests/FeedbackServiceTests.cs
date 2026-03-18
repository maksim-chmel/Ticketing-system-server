using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Feedback;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace AdminPanelBack.Tests;

public class FeedbackServiceTests
{
    private readonly Mock<IFeedbackRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly FeedbackService _service;

    public FeedbackServiceTests()
    {
        _mockRepo = new Mock<IFeedbackRepository>();
        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<List<FeedbackDto>>(It.IsAny<List<Feedback>>()))
            .Returns((List<Feedback> src) => src.Select(f => new FeedbackDto
            {
                Id = f.Id,
                UserId = f.UserId,
                Comment = f.Comment,
                Status = f.Status
            }).ToList());

        _service = new FeedbackService(_mockRepo.Object, mockMapper.Object);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenFeedbacksExist_ReturnsMappedDtos()
    {
        // Arrange
        var feedbacks = new List<Feedback>
        {
            new() { Id = 1, UserId = 100, Comment = "Great service", Status = FeedbackStatus.Open },
            new() { Id = 2, UserId = 200, Comment = "Issue with login", Status = FeedbackStatus.InProgress }
        };
        _mockRepo.Setup(r => r.GetAllFeedbacksAsync()).ReturnsAsync(feedbacks);

        // Act
        var result = await _service.GetAllFeedbacksAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[1].Comment.Should().Be("Issue with login");
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenNoFeedbacks_ReturnsEmptyList()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllFeedbacksAsync()).ReturnsAsync(new List<Feedback>());

        // Act
        var result = await _service.GetAllFeedbacksAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackExists_UpdatesStatusAndSaves()
    {
        // Arrange
        var feedback = new Feedback { Id = 1, Status = FeedbackStatus.Open };
        _mockRepo.Setup(r => r.FindAsyncById(1)).ReturnsAsync(feedback);

        // Act
        await _service.UpdateStatus(1, FeedbackStatus.Done);

        // Assert
        feedback.Status.Should().Be(FeedbackStatus.Done);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackNotFound_DoesNotSave()
    {
        // Arrange
        _mockRepo.Setup(r => r.FindAsyncById(99)).ReturnsAsync((Feedback?)null);

        // Act
        await _service.UpdateStatus(99, FeedbackStatus.Done);

        // Assert
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenOneFeedbackExists_ReturnsIt()
    {
        _mockRepo.Setup(r => r.GetAllFeedbacksAsync()).ReturnsAsync(new List<Feedback>{new (){Id = 5}});
        var result = await _service.GetAllFeedbacksAsync();
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(5);
    }
    [Fact]
    public async Task GetAllFeedbacksAsync_WhenFeedbacksHaveDifferentStatus_ReturnsIt()
    {
        _mockRepo.Setup(r => r.GetAllFeedbacksAsync()).ReturnsAsync(new List<Feedback>{new (){Status =  FeedbackStatus.Open},
            new(){Status =  FeedbackStatus.InProgress } } );
        var result = await _service.GetAllFeedbacksAsync();
        result.Should().HaveCount(2);
        
        result[0].Status.Should().Be(FeedbackStatus.Open);
        result[1].Status.Should().Be(FeedbackStatus.InProgress);
    }

    [Theory]
    [InlineData(FeedbackStatus.InProgress)]
    [InlineData(FeedbackStatus.Waiting)]
    [InlineData(FeedbackStatus.Rejected)]
    public async Task UpdateStatus_ToAnyStatus_SavesCorrectly(FeedbackStatus newStatus)
    {
        // Arrange
        var feedback = new Feedback { Id = 1, Status = FeedbackStatus.Open };
        _mockRepo.Setup(r => r.FindAsyncById(1)).ReturnsAsync(feedback);

        // Act
        await _service.UpdateStatus(1, newStatus);

        // Assert
        feedback.Status.Should().Be(newStatus);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}