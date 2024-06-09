using System.Collections.Generic;
using System.Collections.Immutable;
using AutoFixture;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Domain = Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Messengers.Telegram.Tests
{
    [TestFixture]
    public class TelegramNotifyServiceTests
    {
        private MockRepository _mockRepository;

        private Mock<IUsersService> _usersServiceMock;
        private Mock<ITelegramBotClient> _clientMock;
        private ILogger<TelegramNotifyService> _logMock;

        private TelegramNotifyService _target;

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _usersServiceMock = _mockRepository.Create<IUsersService>();
            _clientMock = _mockRepository.Create<ITelegramBotClient>();
            _logMock = Mock.Of<ILogger<TelegramNotifyService>>();

            _target = new TelegramNotifyService(_usersServiceMock.Object, _clientMock.Object, _logMock);

            _fixture = new Fixture { OmitAutoProperties = true };
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.VerifyAll();
            _mockRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task NotifyAbout_NoAnyNotifications_NothingHappened()
        {
            var notifications = Enumerable.Empty<Notification>().ToImmutableArray();

            var ct = CancellationToken.None;

            var result = await _target.NotifyAbout(notifications, ct);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task NotifyAbout_NoAnyUsers_NothingHappened()
        {
            var notifications = _fixture.CreateMany<Notification>().ToImmutableArray();
            var users = Enumerable.Empty<Domain.User>().ToImmutableArray();

            var ct = CancellationToken.None;

            _usersServiceMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(users);

            var result = await _target.NotifyAbout(notifications, ct);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task NotifyAbout_AnyNotificationsAndUsers_SentAllNotificationsToAllUsers()
        {
            var payload = _fixture.Build<NotificationPayload>()
                .With(n => n.Text)
                .Create();
            var notification = _fixture.Build<Notification>()
                .With(n => n.Payload, payload)
                .Create();
            var user = _fixture.Build<Domain.User>()
                .With(u => u.Id)
                .With(u => u.MessengerUserId, _fixture.Create<long>().ToString)
                .Create();

            var ct = CancellationToken.None;

            _usersServiceMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(new []{user});
            _clientMock.Setup(c => c.MakeRequestAsync(It.IsAny<SendMessageRequest>(), ct))
                .Callback((IRequest<Message> request, CancellationToken _) =>
                {
                    var sendMessageRequest = request as SendMessageRequest;
                    Assert.AreEqual(sendMessageRequest?.Text, notification.Payload.Text);
                    Assert.AreEqual(sendMessageRequest?.ChatId, (ChatId)user.MessengerUserId);
                }).ReturnsAsync(() => null);

            var pushedNotifications = (await _target.NotifyAbout(new []{notification}, ct)).ToArray();

            Assert.IsNotEmpty(pushedNotifications);

            var pushedNotification = pushedNotifications.First();

            Assert.AreEqual(notification.Id, pushedNotification.NotificationId);
            Assert.AreEqual(user.Id, pushedNotification.UserId);
        }

        [Test]
        public async Task NotifyAbout_AdminNotifications_SentOnlyToAdminUsers()
        {
            var payload = _fixture.Build<NotificationPayload>()
                .With(n => n.Text)
                .Create();
            var notification = _fixture.Build<Notification>()
                .With(n => n.Payload, payload)
                .With(n => n.Type, NotificationType.Admin)
                .Create();
            var ordinaryUser = _fixture.Build<Domain.User>()
                .With(u => u.Id)
                .With(u => u.MessengerUserId, _fixture.Create<long>().ToString)
                .Create();
            var adminUser = _fixture.Build<Domain.User>()
                .With(u => u.Id)
                .With(u => u.MessengerUserId, _fixture.Create<long>().ToString)
                .With(u => u.IsAdmin, true)
                .Create();

            var ct = CancellationToken.None;

            _usersServiceMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(new []{ adminUser, ordinaryUser });
            _clientMock.Setup(c => c.MakeRequestAsync(It.IsAny<SendMessageRequest>(), ct))
                .Callback((IRequest<Message> request, CancellationToken _) =>
                {
                    var sendMessageRequest = request as SendMessageRequest;
                    Assert.AreEqual(sendMessageRequest?.Text, notification.Payload.Text);
                    Assert.AreEqual(sendMessageRequest?.ChatId, (ChatId)adminUser.MessengerUserId);
                })
                .ReturnsAsync(() => null);

            var pushedNotifications = (await _target.NotifyAbout(new []{notification}, ct)).ToArray();

            Assert.IsNotEmpty(pushedNotifications);

            Assert.True(pushedNotifications.All(n => n.UserId == adminUser.Id));
        }
    }
}
