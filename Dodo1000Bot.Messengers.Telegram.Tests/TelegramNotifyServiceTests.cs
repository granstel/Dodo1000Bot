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

        private Mock<IUsersRepository> _usersRepositoryMock;
        private Mock<ITelegramBotClient> _clientMock;
        private ILogger<TelegramNotifyService> _logMock;

        private TelegramNotifyService _target;

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _usersRepositoryMock = _mockRepository.Create<IUsersRepository>();
            _clientMock = _mockRepository.Create<ITelegramBotClient>();
            _logMock = Mock.Of<ILogger<TelegramNotifyService>>();

            _target = new TelegramNotifyService(_usersRepositoryMock.Object, _clientMock.Object, _logMock);

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

            _usersRepositoryMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(users);

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

            _usersRepositoryMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(new []{user});
            _clientMock.Setup(c => c.SendTextMessageAsync(user.MessengerUserId, notification.Payload.Text,
            It.IsAny<ParseMode>(), It.IsAny<IEnumerable<MessageEntity>>(), 
            It.IsAny<bool>(), It.IsAny<bool>(), 
            It.IsAny<int>(), It.IsAny<bool>(), 
            It.IsAny<IReplyMarkup>(), ct)).ReturnsAsync(() => null);

            var pushedNotifications = (await _target.NotifyAbout(new []{notification}, ct)).ToArray();

            Assert.IsNotEmpty(pushedNotifications);

            var pushedNotification = pushedNotifications.First();

            Assert.AreEqual(notification.Id, pushedNotification.NotificationId);
            Assert.AreEqual(user.Id, pushedNotification.UserId);
        }
    }
}
