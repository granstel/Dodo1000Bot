using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoFixture;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dodo1000Bot.Services.Tests;

[TestFixture]
public class UsersServiceTests
{
    private MockRepository _mockRepository;

    private ILogger<UsersService> _logMock;
    private Mock<IUsersRepository> _usersRepositoryMock;
    private Mock<ChannelWriter<Notification>> _notificationsChannelMock;
    
    private UsersService _target;

    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _logMock = Mock.Of<ILogger<UsersService>>();
        _usersRepositoryMock = _mockRepository.Create<IUsersRepository>();
        _notificationsChannelMock = _mockRepository.Create<ChannelWriter<Notification>>();

        _target = new UsersService(_logMock, _usersRepositoryMock.Object, _notificationsChannelMock.Object);

        _fixture = new Fixture { OmitAutoProperties = true };
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
        _mockRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Save_User_SavedUser()
    {
        var user = _fixture.Create<User>();

        _usersRepositoryMock.Setup(r => r.SaveUser(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _usersRepositoryMock.Setup(r => r.Count(It.IsAny<CancellationToken>())).Throws<Exception>();

        await _target.SaveAndNotify(user, CancellationToken.None);
    }

    [Test]
    public async Task CheckAndNotifyAboutSubscribers_RemainderNotEqualsZero_NoAnyNotifications()
    {
        _usersRepositoryMock.Setup(r => r.Count(It.IsAny<CancellationToken>())).ReturnsAsync(9);

        await _target.CheckAndNotifyAboutSubscribers(CancellationToken.None);

        _notificationsChannelMock.Verify(s => 
            s.WriteAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CheckAndNotifyAboutSubscribers_RemainderEqualsZero_NoAnyNotifications()
    {
        const int subscribersCount = 200;
        _usersRepositoryMock.Setup(r => r.Count(It.IsAny<CancellationToken>())).ReturnsAsync(subscribersCount);

        var expectedText = $"👥 Hey! I already have {subscribersCount} subscribers! Thank you for staying with me 🤗";

        _notificationsChannelMock.Setup(s => s.WriteAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback((Notification notification, CancellationToken _) => 
                Assert.AreEqual(expectedText, notification.Payload.Text))
            .Returns(ValueTask.CompletedTask);

        await _target.CheckAndNotifyAboutSubscribers(CancellationToken.None);
    }
}
