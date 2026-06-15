using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Hubs;
using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Feedback;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace AdminPanelBack.Tests;

public class FeedbackServiceTests
{
    private readonly Mock<IFeedbackRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IFeedbackHistoryService> _mockHistoryService;
    private readonly Mock<IHubContext<FeedbackHub>> _mockHubContext;
    private readonly FeedbackService _service;

    public FeedbackServiceTests()
    {
        _mockRepo = new Mock<IFeedbackRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockHistoryService = new Mock<IFeedbackHistoryService>();
        _mockHubContext = new Mock<IHubContext<FeedbackHub>>();
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        _mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

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
                Comment = dto.Comment ?? string.Empty
            });

        _service = new FeedbackService(_mockRepo.Object, mockMapper.Object, NullLogger<FeedbackService>.Instance,
            _mockUnitOfWork.Object, _mockHistoryService.Object, _mockHubContext.Object);
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackExists_UpdatesStatusAndSaves()
    {
        var feedback = new Feedback { Id = 1, Status = FeedbackStatus.Open };
        _mockRepo.Setup(r => r.FindAsyncById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);

        await _service.UpdateStatus(1, FeedbackStatus.Done, "admin-id", "admin");

        feedback.Status.Should().Be(FeedbackStatus.Done);
        _mockHistoryService.Verify(h => h.AddAsync(1, "admin-id", "admin",
            FeedbackHistoryAction.StatusChanged, "Open", "Done", It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_WhenFeedbackNotFound_ThrowsNotFoundException()
    {
        _mockRepo.Setup(r => r.FindAsyncById(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feedback?)null);

        var act = () => _service.UpdateStatus(99, FeedbackStatus.Done, "admin-id", "admin");

        await act.Should().ThrowAsync<NotFoundException>();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateFeedbackAsync_WhenCalled_AddsFeedbackAndSaves()
    {
        var dto = new UsersMessageDto { UserId = 1, Comment = "New feedback" };
        _mockRepo.Setup(r => r.AddFeedbackAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()));

        await _service.CreateFeedbackAsync(dto);

        _mockRepo.Verify(r => r.AddFeedbackAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WhenFeedbacksExist_ReturnsMappedDtos()
    {
        var feedbacks = new List<Feedback>
        {
            new() { Id = 1, UserId = 100, Comment = "Great service" }
        };
        _mockRepo.Setup(r => r.GetFeedbacksPageAsync(0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedbacks);
        _mockRepo.Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.GetAllFeedbacksAsync(1, 50);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.Items[0].Id.Should().Be(1);
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
    public async Task ClaimAsync_WhenFeedbackExists_AssignsAdminAndSaves()
    {
        var feedback = new Feedback { Id = 1, Status = FeedbackStatus.Open };
        _mockRepo.Setup(r => r.FindAsyncById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);

        await _service.ClaimAsync(1, "admin-id", "ivan");

        feedback.AssignedAdminId.Should().Be("admin-id");
        feedback.AssignedAdminName.Should().Be("ivan");
        _mockHistoryService.Verify(h => h.AddAsync(1, "admin-id", "ivan",
            FeedbackHistoryAction.Claimed, null, null, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClaimAsync_WhenFeedbackNotFound_ThrowsNotFoundException()
    {
        _mockRepo.Setup(r => r.FindAsyncById(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feedback?)null);

        var act = () => _service.ClaimAsync(99, "admin-id", "ivan");

        await act.Should().ThrowAsync<NotFoundException>();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
