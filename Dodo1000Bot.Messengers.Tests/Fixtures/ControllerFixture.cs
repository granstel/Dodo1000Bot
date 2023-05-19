using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Messengers.Tests.Fixtures
{
    public class ControllerFixture : MessengerController<InputFixture, OutputFixture>
    {
        public ControllerFixture(ILogger<ControllerFixture> log, IMessengerService<InputFixture, OutputFixture> messengerService, MessengerConfiguration configuration)
            : base(log, messengerService, configuration)
        {
        }
    }
}
