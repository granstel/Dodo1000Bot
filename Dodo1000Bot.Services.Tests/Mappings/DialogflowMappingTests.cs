using AutoMapper;
using Dodo1000Bot.Services.Mappings;
using NUnit.Framework;

namespace Dodo1000Bot.Services.Tests.Mappings
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
