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

            _fixture = new Fixture();
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
            var notifications = Enumerable.Empty<Notification>();

            var ct = CancellationToken.None;

            _usersRepositoryMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(() => null);

            var result = await _target.NotifyAbout(notifications, ct);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task NotifyAbout_NoAnyUsers_NothingHappened()
        {
            var notifications = _fixture.CreateMany<Notification>();
            var users = Enumerable.Empty<User>();

            var ct = CancellationToken.None;

            _usersRepositoryMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(users);

            var result = await _target.NotifyAbout(notifications, ct);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task NotifyAbout_AnyNotificationsAndUsers_SentAllNotificationsToAllUsers()
        {
            var notifications = _fixture.CreateMany<Notification>();
            var users = _fixture.CreateMany<User>();

            var ct = CancellationToken.None;

            _usersRepositoryMock.Setup(r => r.GetUsers(Source.Telegram, ct)).ReturnsAsync(users);
            _clientMock.Setup(c => c.SendTextMessageAsync(It.IsAny<string>(), It.IsAny<string>()));

            var result = await _target.NotifyAbout(notifications, ct);

            Assert.IsEmpty(result);
        }
    }
}
