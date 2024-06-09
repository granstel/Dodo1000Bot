using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace Dodo1000Bot.Messengers.Telegram.Tests
{
    [TestFixture]
    public class TelegramServiceTests
    {
        private MockRepository _mockRepository;

        private Mock<ITelegramBotClient> _telegramBotClient;
        private Mock<IConversationService> _conversationService;
        private Mock<IMapper> _mapper;

        private TelegramService _target;

        private Fixture _fixture;

        [SetUp]
        public void InitTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);

            var loggerMock = Mock.Of<ILogger<TelegramService>>();
            _telegramBotClient = _mockRepository.Create<ITelegramBotClient>();
            _conversationService = _mockRepository.Create<IConversationService>();
            _mapper = _mockRepository.Create<IMapper>();

            _target = new TelegramService(loggerMock, _telegramBotClient.Object, _conversationService.Object, _mapper.Object);
            
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetMeAsync_Invokations_Success()
        {
            await _target.GetMeAsync();

            _telegramBotClient.Verify(c => c.MakeRequestAsync(It.IsAny<GetMeRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task TestApiAsync_Invokations_Success()
        {
            await _target.TestApiAsync();

            _telegramBotClient.Verify(c => c.TestApiAsync(It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task SetWebhookAsync_Invocation_Success()
        {
            await _target.SetWebhookAsync(It.IsAny<string>(), CancellationToken.None);

            _telegramBotClient.Verify(c => c.MakeRequestAsync(It.IsAny<SetWebhookRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ProcessIncomingAsync_Invocations_Success()
        {
            var inputModel = _fixture.Build<Update>()
                .OmitAutoProperties()
                .Create();

            var request = _fixture.Build<Request>()
                .OmitAutoProperties()
                .Create();

            var chatId = _fixture.Create<long>();

            var response = _fixture.Build<Response>()
                .With(r => r.Text)
                .With(r => r.ChatHash, chatId.ToString())
                .OmitAutoProperties()
                .Create();

            _mapper.Setup(m => m.Map<Request>(It.IsAny<Update>())).Returns(request);

            _conversationService.Setup(s => s.GetResponseAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            _mapper.Setup(m => m.Map(It.IsAny<Request>(), It.IsAny<Response>())).Returns(() => null);

 
            var result = await _target.ProcessIncomingAsync(inputModel, CancellationToken.None);


            _mockRepository.VerifyAll();

            Assert.NotNull(result);
        }
    }
}
