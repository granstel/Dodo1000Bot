using AutoFixture;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
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
        private Mock<ICountriesService> _countriesService;

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
            _countriesService = _mockRepository.Create<ICountriesService>();

            _target = new UnitsService(
                _logMock, 
                _globalApiClientMock.Object, 
                _notificationsServiceMock.Object, 
                _snapshotsRepositoryMock.Object,
                _countriesService.Object);

            _fixture = new Fixture { OmitAutoProperties = true };
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.VerifyAll();
            _mockRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task AboutTotalAtCountries_ZeroUnit_NoAnyNotification()
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
        public async Task AboutNewCountries_NewCountryAtAnotherBrand_Notification()
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
                .With(b => b.Countries, new []{ newCountry })
                .With(b => b.Brand, Brands.Drinkit)
                .Create();
            var unitsCount = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCount })
                .Create();

            var brandUnitCountAtSnapshot = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new []{ country })
                .With(b => b.Brand, Brands.Dodopizza)
                .Create();
            var unitsCountSnapshot = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCountAtSnapshot })
                .Create();

            var expectedText = $"There is new country of {brandUnitCount.Brand} - {newCountry.CountryName}!";

            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.AreEqual(notification.Payload.Text, expectedText);
                })
                .Returns(Task.CompletedTask);

            await _target.AboutNewCountries(unitsCount, unitsCountSnapshot, CancellationToken.None);
        }

        [Test]
        public async Task AboutNewCountries_NewCountryAtSameBrand_Notification()
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
                .With(b => b.Countries, new []{ country, newCountry  })
                .With(b => b.Brand, Brands.Doner42)
                .Create();
            var unitsCount = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCount })
                .Create();

            var brandUnitCountAtSnapshot = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new []{ country})
                .With(b => b.Brand, Brands.Doner42)
                .Create();
            var unitsCountSnapshot = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCountAtSnapshot })
                .Create();

            var expectedText = $"There is new country of {brandUnitCount.Brand} - {newCountry.CountryName}!";

            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.AreEqual(notification.Payload.Text, expectedText);
                })
                .Returns(Task.CompletedTask);

            await _target.AboutNewCountries(unitsCount, unitsCountSnapshot, CancellationToken.None);
        }

        [Test]
        public async Task AboutNewCountries_LessCountriesThanAtSnapshot_NoAnyNotification()
        {
            var country = _fixture.Build<UnitCountModel>()
                .With(c => c.PizzeriaCount, 0)
                .With(c => c.CountryName)
                .Create();
            var oldCountry = _fixture.Build<UnitCountModel>()
                .With(c => c.PizzeriaCount, 0)
                .With(c => c.CountryName)
                .Create();

            var brandUnitCount = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new []{ country })
                .With(b => b.Brand, Brands.Doner42)
                .Create();
            var unitsCount = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCount })
                .Create();

            var brandUnitCountAtSnapshot = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new []{ country, oldCountry })
                .With(b => b.Brand, Brands.Doner42)
                .Create();
            var unitsCountSnapshot = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCountAtSnapshot })
                .Create();

            await _target.AboutNewCountries(unitsCount, unitsCountSnapshot, CancellationToken.None);

            _notificationsServiceMock.Verify(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task AboutNewCountries_NullSnapshotData_NothingHappened()
        {
            var country = _fixture.Build<UnitCountModel>()
                .With(c => c.PizzeriaCount, 0)
                .With(c => c.CountryName)
                .Create();
            var brandUnitCount = _fixture.Build<BrandTotalUnitCountListModel>()
                .With(b => b.Countries, new []{ country })
                .With(b => b.Brand, Brands.Doner42)
                .Create();
            var unitsCount = _fixture.Build<BrandListTotalUnitCountListModel>()
                .With(c => c.Brands, new[] { brandUnitCount })
                .Create();

            BrandListTotalUnitCountListModel unitsCountSnapshot = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            await _target.AboutNewCountries(unitsCount, unitsCountSnapshot, CancellationToken.None);

            _notificationsServiceMock.Verify(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task CheckUnitsOfBrandAtCountryAndNotify_SameUnitNames_NoAnyNotifications()
        {
            var brand = _fixture.Create<Brands>();
            var countryId = _fixture.Create<int>();

            var unitName = _fixture.Create<string>();
            var unitModel = _fixture.Build<UnitModel>()
                .With(m => m.Name, unitName)
                .Create();
            var unitListModel = _fixture.Build<UnitListModel>()
                    .With(m => m.Pizzerias, new []{unitModel})
                    .Create();
            var unitsAtCountry = _fixture.Build<BrandData<UnitListModel>>()
                    .With(m => m.Countries, new []{unitListModel})
                    .Create();

            var unitModelSnapshot = _fixture.Build<UnitModel>()
                .With(m => m.Name, unitName)
                .Create();
            var unitListModelSnapshot = _fixture.Build<UnitListModel>()
                    .With(m => m.Pizzerias, new []{unitModelSnapshot})
                    .Create();
            var unitsAtCountrySnapshot = _fixture.Build<BrandData<UnitListModel>>()
                    .With(m => m.Countries, new []{unitListModelSnapshot})
                    .Create();

            _globalApiClientMock.Setup(c => c.UnitsOfBrandAtCountry(brand, countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(unitsAtCountry);

            var snapshotName = $"UnitsOfBrandAtCountry{brand}{countryId}";
            var snapshot = Snapshot<BrandData<UnitListModel>>.Create(snapshotName, unitsAtCountrySnapshot);

            _snapshotsRepositoryMock.Setup(r => r.Get<BrandData<UnitListModel>>(snapshotName, CancellationToken.None))
                .ReturnsAsync(snapshot);

            _snapshotsRepositoryMock.Setup(r => 
                r.Save(It.IsAny<Snapshot<BrandData<UnitListModel>>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _target.CheckUnitsOfBrandAtCountryAndNotify(brand, countryId, CancellationToken.None);
        }

        [Test]
        public async Task CheckUnitsOfBrandAtCountryAndNotify_LessUnitsThanAtSnapshot_NoAnyNotification()
        {
            var brand = _fixture.Create<Brands>();
            var countryId = _fixture.Create<int>();

            var unitName = _fixture.Create<string>();
            var unitModel = _fixture.Build<UnitModel>()
                .With(m => m.Name, unitName)
                .Create();
            var unitListModel = _fixture.Build<UnitListModel>()
                    .With(m => m.Pizzerias, new []{unitModel})
                    .Create();
            var unitsAtCountry = _fixture.Build<BrandData<UnitListModel>>()
                    .With(m => m.Countries, new []{unitListModel})
                    .Create();

            var oldUnitName = _fixture.Create<string>();
            var oldUnitModel = _fixture.Build<UnitModel>()
                .With(m => m.Name, oldUnitName)
                .Create();
            var unitModelSnapshot = _fixture.Build<UnitModel>()
                .With(m => m.Name, unitName)
                .Create();
            var unitListModelSnapshot = _fixture.Build<UnitListModel>()
                    .With(m => m.Pizzerias, new []{unitModelSnapshot, oldUnitModel})
                    .Create();
            var unitsAtCountrySnapshot = _fixture.Build<BrandData<UnitListModel>>()
                    .With(m => m.Countries, new []{unitListModelSnapshot})
                    .Create();

            _globalApiClientMock.Setup(c => c.UnitsOfBrandAtCountry(brand, countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(unitsAtCountry);

            var snapshotName = $"UnitsOfBrandAtCountry{brand}{countryId}";
            var snapshot = Snapshot<BrandData<UnitListModel>>.Create(snapshotName, unitsAtCountrySnapshot);

            _snapshotsRepositoryMock.Setup(r => r.Get<BrandData<UnitListModel>>(snapshotName, CancellationToken.None))
                .ReturnsAsync(snapshot);

            _snapshotsRepositoryMock.Setup(r => 
                r.Save(It.IsAny<Snapshot<BrandData<UnitListModel>>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _target.CheckUnitsOfBrandAtCountryAndNotify(brand, countryId, CancellationToken.None);
        }

        [Test]
        public async Task CheckUnitsOfBrandAtCountryAndNotify_NewUnit_Notification()
        {
            var brand = _fixture.Create<Brands>();
            var countryId = _fixture.Create<int>();

            var unitName = _fixture.Create<string>();
            var unitModel = _fixture.Build<UnitModel>()
                .With(m => m.Name, unitName)
                .Create();
            var newUnitName = _fixture.Create<string>();
            var expectedCoordinates = _fixture.Build<CoordinatesModel>()
                .With(c => c.Lat)
                .With(c => c.Long)
                .Create();
            var newUnitModel = _fixture.Build<UnitModel>()
                .With(m => m.Name, newUnitName)
                .With(m => m.Coords, expectedCoordinates)
                .Create();

            var unitListModel = _fixture.Build<UnitListModel>()
                    .With(m => m.Pizzerias, new []{unitModel, newUnitModel})
                    .Create();
            var unitsAtCountry = _fixture.Build<BrandData<UnitListModel>>()
                    .With(m => m.Countries, new []{unitListModel})
                    .Create();

            var unitModelSnapshot = _fixture.Build<UnitModel>()
                .With(m => m.Name, unitName)
                .Create();
            var unitListModelSnapshot = _fixture.Build<UnitListModel>()
                    .With(m => m.Pizzerias, new []{unitModelSnapshot})
                    .Create();
            var unitsAtCountrySnapshot = _fixture.Build<BrandData<UnitListModel>>()
                    .With(m => m.Countries, new []{unitListModelSnapshot})
                    .Create();

            _globalApiClientMock.Setup(c => c.UnitsOfBrandAtCountry(brand, countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(unitsAtCountry);

            var snapshotName = $"UnitsOfBrandAtCountry{brand}{countryId}";
            var snapshot = Snapshot<BrandData<UnitListModel>>.Create(snapshotName, unitsAtCountrySnapshot);

            _snapshotsRepositoryMock.Setup(r => r.Get<BrandData<UnitListModel>>(snapshotName, CancellationToken.None))
                .ReturnsAsync(snapshot);

            _snapshotsRepositoryMock.Setup(r => 
                r.Save(It.IsAny<Snapshot<BrandData<UnitListModel>>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedText = $"There is new unit of {brand} - {newUnitName}! You can find it here👇";
            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.AreEqual(notification.Payload.Text, expectedText);
                    Assert.AreEqual(notification.Payload.Coordinates, expectedCoordinates);
                })
                .Returns(Task.CompletedTask);

            await _target.CheckUnitsOfBrandAtCountryAndNotify(brand, countryId, CancellationToken.None);
        }
    }
}
