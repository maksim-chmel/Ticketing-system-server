using AdminPanelBack.Models;
using AdminPanelBack.Models.Statistics;
using AdminPanelBack.Profiles;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Statistic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdminPanelBack.Tests;

public class StatisticsServiceTests
{
    private readonly Mock<IStatisticsRepository> _repositoryMock = new();

    private readonly IMapper _mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<StatisticProfile>();
    }).CreateMapper();

    private StatisticsService CreateService() => new(_repositoryMock.Object, _mapper, new Mock<ILogger<StatisticsService>>().Object);

    [Fact]
    public async Task GetStatusDistributionAsync_ShouldReturnMappedDtos()
    {
        var fakeData = new List<StatusDistributionItem>
        {
            new() { Status = FeedbackStatus.Open, Count = 10 },
            new() { Status = FeedbackStatus.Done, Count = 3 }
        };

        _repositoryMock
            .Setup(r => r.GetStatusDistributionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeData);

        var result = await CreateService().GetStatusDistributionAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(nameof(FeedbackStatus.Open), result[0].Name);
        Assert.Equal(10, result[0].Value);
        Assert.Equal(nameof(FeedbackStatus.Done), result[1].Name);
        Assert.Equal(3, result[1].Value);
    }

    [Fact]
    public async Task GetStatusDistributionAsync_EmptyRepository_ShouldReturnEmptyList()
    {
        _repositoryMock
            .Setup(r => r.GetStatusDistributionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await CreateService().GetStatusDistributionAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStatusDistributionAsync_ShouldPassCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repositoryMock
            .Setup(r => r.GetStatusDistributionAsync(token))
            .ReturnsAsync([]);

        await CreateService().GetStatusDistributionAsync(token);

        _repositoryMock.Verify(r => r.GetStatusDistributionAsync(token), Times.Once);
    }

    [Fact]
    public async Task GetRequestsOverTimeAsync_ShouldReturnMappedDtos()
    {
        var date = new DateTime(2026, 5, 1);
        var fakeData = new List<RequestsOverTimeItem>
        {
            new() { Date = date, Count = 7 }
        };

        _repositoryMock
            .Setup(r => r.GetRequestsOverTimeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeData);

        var result = await CreateService().GetRequestsOverTimeAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("2026-05-01", result[0].Date);
        Assert.Equal(7, result[0].Value);
    }

    [Fact]
    public async Task GetRequestsOverTimeAsync_EmptyRepository_ShouldReturnEmptyList()
    {
        _repositoryMock
            .Setup(r => r.GetRequestsOverTimeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await CreateService().GetRequestsOverTimeAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRequestsOverTimeAsync_ShouldPassCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repositoryMock
            .Setup(r => r.GetRequestsOverTimeAsync(token))
            .ReturnsAsync([]);

        await CreateService().GetRequestsOverTimeAsync(token);

        _repositoryMock.Verify(r => r.GetRequestsOverTimeAsync(token), Times.Once);
    }

    [Fact]
    public async Task GetRequestsOverTimeAsync_MultipleDates_ShouldMapAllCorrectly()
    {
        var fakeData = new List<RequestsOverTimeItem>
        {
            new() { Date = new DateTime(2026, 1, 1), Count = 2 },
            new() { Date = new DateTime(2026, 1, 2), Count = 5 },
            new() { Date = new DateTime(2026, 1, 3), Count = 1 }
        };

        _repositoryMock
            .Setup(r => r.GetRequestsOverTimeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeData);

        var result = await CreateService().GetRequestsOverTimeAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal("2026-01-01", result[0].Date);
        Assert.Equal(2, result[0].Value);
        Assert.Equal("2026-01-02", result[1].Date);
        Assert.Equal(5, result[1].Value);
        Assert.Equal("2026-01-03", result[2].Date);
        Assert.Equal(1, result[2].Value);
    }
}
