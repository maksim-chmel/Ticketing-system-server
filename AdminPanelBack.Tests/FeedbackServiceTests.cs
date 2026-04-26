using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Feedback;
using AutoMapper;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Tests;

public class FeedbackServiceTests
{
    private readonly Mock<IFeedbackRepository> _mockRepo;
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly FeedbackService _service;

    public FeedbackServiceTests()
    {
        _mockRepo = new Mock<IFeedbackRepository>();
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

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
        
        _service = new FeedbackService(_mockRepo.Object, mockMapper.Object,NullLogger<FeedbackService>.Instance, _mockDbContext.Object);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenFeedbacksExist_ReturnsMappedDtos()
    {
        
        var feedbacks = new List<Feedback>
        {
            new() { Id = 1, UserId = 100, Comment = "Great service", Status = FeedbackStatus.Open },
            new() { Id = 2, UserId = 200, Comment = "Issue with login", Status = FeedbackStatus.InProgress }
        };
        _mockRepo.Setup(r => r.GetFeedbacksPageAsync(0, 50)).ReturnsAsync(feedbacks);

       
        var result = await _service.GetAllFeedbacksAsync(1, 50);

        
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[1].Comment.Should().Be("Issue with login");
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenNoFeedbacks_ReturnsEmptyList()
    {
        
        _mockRepo.Setup(r => r.GetFeedbacksPageAsync(0, 50)).ReturnsAsync(new List<Feedback>());

        
        var result = await _service.GetAllFeedbacksAsync(1, 50);

       
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackExists_UpdatesStatusAndSaves()
    {
        
        var feedback = new Feedback { Id = 1, Status = FeedbackStatus.Open };
        _mockRepo.Setup(r => r.FindAsyncById(1)).ReturnsAsync(feedback);

       
        await _service.UpdateStatus(1, FeedbackStatus.Done);

        
        feedback.Status.Should().Be(FeedbackStatus.Done);
        _mockDbContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackNotFound_DoesNotSave()
    {
        
        _mockRepo.Setup(r => r.FindAsyncById(99)).ReturnsAsync((Feedback?)null);

        
        await _service.UpdateStatus(99, FeedbackStatus.Done);

        
        _mockDbContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenOneFeedbackExists_ReturnsIt()
    {
        _mockRepo.Setup(r => r.GetFeedbacksPageAsync(0, 50)).ReturnsAsync(new List<Feedback>{new (){Id = 5}});
        var result = await _service.GetAllFeedbacksAsync(1, 50);
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(5);
    }
    [Fact]
    public async Task GetAllFeedbacksAsync_WhenFeedbacksHaveDifferentStatus_ReturnsIt()
    {
        _mockRepo.Setup(r => r.GetFeedbacksPageAsync(0, 50)).ReturnsAsync(new List<Feedback>{new (){Status =  FeedbackStatus.Open},
            new(){Status =  FeedbackStatus.InProgress } } );
        var result = await _service.GetAllFeedbacksAsync(1, 50);
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
        
        var feedback = new Feedback { Id = 1, Status = FeedbackStatus.Open };
        _mockRepo.Setup(r => r.FindAsyncById(1)).ReturnsAsync(feedback);

        
        await _service.UpdateStatus(1, newStatus);

       
        feedback.Status.Should().Be(newStatus);
        _mockDbContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFeedbackAsync_WhenCalled_AddsFeedbackAndSaves()
    {
        var dto = new UsersMessageDto { UserId = 1, Comment = "New feedback" };
        _mockRepo.Setup(r => r.AddFeedbackAsync(It.IsAny<Feedback>()));
        
        await _service.CreateFeedbackAsync(dto);

        _mockRepo.Verify(r => r.AddFeedbackAsync(It.IsAny<Feedback>()), Times.Once);
        _mockDbContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
