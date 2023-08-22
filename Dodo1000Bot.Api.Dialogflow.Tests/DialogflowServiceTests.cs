using AutoFixture;
using AutoMapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;

namespace Dodo1000Bot.Api.Dialogflow.Tests
{
    [TestFixture]
    internal class DialogflowServiceTests
    {
        private MockRepository _mockRepository;

        private ILogger<DialogflowService> _log;
        private Mock<IConversationService> _conversationServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUsersRepository> _usersRepositoryMock;
        private Mock<ICustomNotificationsRepository> _customNotificationsRepositoryMock;

        private DialogflowService _target;

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _log = Mock.Of<ILogger<DialogflowService>>();
            _conversationServiceMock = _mockRepository.Create<IConversationService>();
            _mapperMock = _mockRepository.Create<IMapper>();
            _usersRepositoryMock = _mockRepository.Create<IUsersRepository>();
            _customNotificationsRepositoryMock = _mockRepository.Create<ICustomNotificationsRepository>();

            _target = new DialogflowService(
                _log,
                _conversationServiceMock.Object,
                _mapperMock.Object,
                _usersRepositoryMock.Object, _customNotificationsRepositoryMock.Object);

            _fixture = new Fixture { OmitAutoProperties = true };
        }

        [Test]
        public async Task SaveUser_InvokesSaveUser_CorrectValues()
        {
            var request = _fixture.Build<Request>()
                .With(r => r.ChatHash)
                .With(r => r.UserHash)
                .With(r => r.Source)
            .Create();

            _usersRepositoryMock.Setup(r => r.SaveUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback((User user, CancellationToken ct) =>
                {
                    Assert.AreEqual(request.ChatHash, user.MessengerUserId);
                    Assert.AreEqual(request.Source, user.MessengerType);
                })
                .Returns(Task.CompletedTask);

            await _target.SaveUser(request, CancellationToken.None);
        }
    }
}
