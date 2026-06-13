using AdminPanelBack.Models;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Broadcast;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AdminPanelBack.Tests;

public class BroadcastMessageServiceTests
{
    private readonly Mock<IBroadcastMessageRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly BroadcastMessageService _service;

    public BroadcastMessageServiceTests()
    {
        _mockRepo = new Mock<IBroadcastMessageRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _service = new BroadcastMessageService(_mockRepo.Object, NullLogger<BroadcastMessageService>.Instance, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateBroadcastMessage_AddsActiveMessageAndSaves()
    {
        await _service.CreateBroadcastMessage("Hello!");

        _mockRepo.Verify(r => r.AddBroadcastMessage(
            It.Is<BroadcastMessage>(m => m.Message == "Hello!" && m.IsActive)), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetActiveBroadcastMessagesAndMakeInactive_WhenMessagesExist_ReturnsPulledMessages()
    {
        var messages = new List<BroadcastMessage>
        {
            new() { Message = "msg1", IsActive = false },
            new() { Message = "msg2", IsActive = false }
        };
        _mockRepo.Setup(r => r.PullActiveBroadcastMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(messages);

        var result = await _service.GetActiveBroadcastMessagesAndMakeInactive();

        result.Should().HaveCount(2);
        _mockRepo.Verify(r => r.PullActiveBroadcastMessagesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetActiveBroadcastMessagesAndMakeInactive_WhenNoMessages_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.PullActiveBroadcastMessagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BroadcastMessage>());

        var result = await _service.GetActiveBroadcastMessagesAndMakeInactive();

        result.Should().BeEmpty();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
