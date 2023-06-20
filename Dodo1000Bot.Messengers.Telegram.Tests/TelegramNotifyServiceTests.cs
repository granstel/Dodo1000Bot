using AutoFixture;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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
    }
}
