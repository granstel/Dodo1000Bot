﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.RealtimeBoard;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dodo1000Bot.Services.Tests;

public class StatisticsServiceTests
{
    private MockRepository _mockRepository;
    
    private ILogger<StatisticsService> _logMock;
    private Mock<IRealtimeBoardApiClient> _realtimeBoardApiClientMock;
    private Mock<INotificationsService> _notificationsServiceMock;

    private StatisticsService _target;
    
    private Fixture _fixture;
    private Random _random;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _logMock = Mock.Of<ILogger<StatisticsService>>();
        _realtimeBoardApiClientMock = _mockRepository.Create<IRealtimeBoardApiClient>();
        _notificationsServiceMock = _mockRepository.Create<INotificationsService>();

        _target = new StatisticsService(_logMock, _realtimeBoardApiClientMock.Object, _notificationsServiceMock.Object);

        _fixture = new Fixture { OmitAutoProperties = true };
        _random = new Random();
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
        _mockRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AboutOrdersPerMinute_GreaterOrEqual1000_Notification()
    {
        var ordersPerMinute = _random.Next(1000, 100000);
        var statistics = _fixture.Build<Statistics>()
            .With(s => s.OrdersPerMinute, ordersPerMinute)
            .Create();

        var expectedText = $"There is {ordersPerMinute} orders per minute! See that on https://realtime.dodobrands.io";

        _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback((Notification notification, CancellationToken ct) =>
            {
                Assert.AreEqual(notification.Payload.Text, expectedText);
            })
            .Returns(Task.CompletedTask);

        await _target.AboutOrdersPerMinute(statistics, CancellationToken.None);
    }
}