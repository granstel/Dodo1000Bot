using Dodo1000Bot.Services.Extensions;
using AutoFixture;
using Dodo1000Bot.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Dodo1000Bot.Services.Tests.Extensions
{
    [TestFixture]
    public class ObjectExtensionsTests
    {
        private Fixture _fixture;

        [SetUp]
        public void InitTest()
        {
            _fixture = new Fixture { OmitAutoProperties = true };
        }

        [Test]
        public void Serialize_String_NotSerialize()
        {
            var expected = _fixture.Create<string>();


            var result = expected.Serialize();


            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Serialize_Object_Serialize()
        {
            var obj = _fixture.Create<object>();
            var expected = JsonConvert.SerializeObject(obj);


            var result = obj.Serialize();


            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Deserialize_Throws_Default()
        {
            var serialized = _fixture.Create<string>();


            var result = serialized.Deserialize<Request>();


            Assert.AreEqual(null, result);
        }
    }
}
