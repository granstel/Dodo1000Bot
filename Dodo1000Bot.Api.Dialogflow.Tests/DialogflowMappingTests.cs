using AutoFixture;
using AutoMapper;
using NUnit.Framework;

namespace Dodo1000Bot.Api.Dialogflow.Tests
{
    [TestFixture]
    public class DialogflowMappingTests
    {
        private IMapper _target;

        [SetUp]
        public void InitTest()
        {
            _target = new Mapper(new MapperConfiguration(c => c.AddProfile<DialogflowMapping>()));
        }

        [Test]
        public void ValidateConfiguration_Success()
        {
            _target.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
