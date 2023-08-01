using AutoFixture;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services.Interfaces;

namespace Dodo1000Bot.Services.Tests
{
    [TestFixture]
    public class UnitsServiceTests
    {
        private MockRepository _mockRepository;

        private ILogger<UnitsService> _logMock;
        private Mock<IGlobalApiClient> _globalApiClientMock;
        private Mock<INotificationsService> _notificationsServiceMock;
        private Mock<ISnapshotsRepository> _snapshotsRepositoryMock;

        private UnitsService _target;

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _logMock = Mock.Of<ILogger<UnitsService>>();
            _globalApiClientMock = _mockRepository.Create<IGlobalApiClient>();
            _notificationsServiceMock = _mockRepository.Create<INotificationsService>();
            _snapshotsRepositoryMock = _mockRepository.Create<ISnapshotsRepository>();

            _target = new UnitsService(
                _logMock, 
                _globalApiClientMock.Object, 
                _notificationsServiceMock.Object, 
                _snapshotsRepositoryMock.Object);

            _fixture = new Fixture { OmitAutoProperties = true };
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.VerifyAll();
            _mockRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task AboutTotalAtCountries_ZeroUnit_NewCountryNotification()
        {
            var country = _fixture.Build<UnitCountModel>()
                .With(c => c.PizzeriaCount, 0)
                .With(c => c.CountryName)
                .Create();

            var brandUnitCount = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new[] { country })
                .With(b => b.Brand)
                .Create();

            var unitsCount = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCount })
            .Create();

            await _target.AboutTotalAtCountries(unitsCount, CancellationToken.None);

            _notificationsServiceMock.Verify(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task AboutNewCountries_NewCountry_Notification()
        {
            var country = _fixture.Build<UnitCountModel>()
                .With(c => c.PizzeriaCount, 0)
                .With(c => c.CountryName)
                .Create();
            var newCountry = _fixture.Build<UnitCountModel>()
                .With(c => c.PizzeriaCount, 0)
                .With(c => c.CountryName)
                .Create();

            var brandUnitCount = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new []{ country })
                .With(b => b.Brand)
                .Create();
            var unitsCount = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCount })
                .Create();

            var brandUnitCountAtSnapshot = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new []{ country, newCountry })
                .With(b => b.Brand)
                .Create();
            var unitsCountAtSnapshot = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCountAtSnapshot })
                .Create();

            var unitsCountSnapshot = new Snapshot<BrandListTotalUnitCountListModel>
            {
                Data = unitsCountAtSnapshot
            };

            var expectedText = $"There is new country of {brandUnitCount.Brand} - {newCountry.CountryName}!";

            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.AreEqual(notification.Payload.Text, expectedText);
                })
                .Returns(Task.CompletedTask);

            await _target.AboutNewCountries(unitsCount, unitsCountSnapshot, CancellationToken.None);
        }
    }
}
