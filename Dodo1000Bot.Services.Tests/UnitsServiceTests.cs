using System;
using AutoFixture;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using System.Linq;
using System.Collections.Generic;
using Dodo1000Bot.Models.PublicApi;

namespace Dodo1000Bot.Services.Tests
{
    [TestFixture]
    public class UnitsServiceTests
    {
        private MockRepository _mockRepository;

        private ILogger<UnitsService> _logMock;
        private Mock<INotificationsService> _notificationsServiceMock;
        private Mock<IGlobalApiService> _globalApiService;
        private Mock<IPublicApiService> _publicApiService;
        
        private Mock<ICountriesService> _countriesServiceMock;

        private UnitsService _target;

        private Fixture _fixture;

        private readonly Random _random = new();

        private string GetRandomCountryCode()
        {
            var keys = Constants.TelegramFlags.Keys;
            var index = _random.Next(keys.Count);

            return keys.ElementAt(index);
        }

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _logMock = Mock.Of<ILogger<UnitsService>>();
            _notificationsServiceMock = _mockRepository.Create<INotificationsService>();
            _globalApiService = _mockRepository.Create<IGlobalApiService>();
            _publicApiService = _mockRepository.Create<IPublicApiService>();
            _countriesServiceMock = _mockRepository.Create<ICountriesService>();

            _target = new UnitsService(
                _logMock,
                _notificationsServiceMock.Object, 
                _globalApiService.Object, 
                _publicApiService.Object,
                _countriesServiceMock.Object);

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

            var countryCode = GetRandomCountryCode();
            var newCountry = _fixture.Build<UnitCountModel>()
                .With(c => c.PizzeriaCount, 0)
                .With(c => c.CountryName)
                .With(c => c.CountryCode, countryCode)
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

            _countriesServiceMock.Setup(s => s.GetName(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            var flag = Constants.TelegramFlags.GetValueOrDefault(countryCode);
            var expectedTexts = new[]
            {
                flag,
                $"🌏 Wow! There is new country of {brandUnitCount.Brand} - {newCountry.CountryName}! {flag}",
            };

            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.True(expectedTexts.Contains(notification.Payload.Text), $"'{notification.Payload.Text}' is not expected text, country code is {countryCode}");
                })
                .Returns(Task.CompletedTask);

            await _target.AboutNewCountries(unitsCount, unitsCountSnapshot, CancellationToken.None);
        }

        [Test]
        public async Task AboutNewCountries_NewUnexpectedCountryAtSameBrand_NotificationWithoutFlag()
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

            _countriesServiceMock.Setup(s => s.GetName(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws<Exception>();

            var expectedTexts = new[]
            {
                "🤩",
                $"🌏 Wow! There is new country of {brandUnitCount.Brand} - {newCountry.CountryName}! 🤩",
            };

            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.True(expectedTexts.Contains(notification.Payload.Text), $"'{notification.Payload.Text}' is not expected text");
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
            var brand = _fixture.Create<string>();
            var countryCode = _fixture.Create<string>();

            var unitName = _fixture.Create<string>();
            var beginDateWork = DateOnly.FromDateTime(_fixture.Create<DateTime>());

            var unitList = _fixture.Build<UnitInfo>()
                .With(u => u.Name, unitName)
                .With(u => u.BeginDateWork, beginDateWork)
                .CreateMany(1);

            var unitListSnapshot = _fixture.Build<UnitInfo>()
                .With(u => u.Name, unitName)
                .With(u => u.BeginDateWork, beginDateWork)
                .CreateMany(1);

            var restaurantsCountAtBrand = _fixture.Create<int>();
            var totalOverall = _fixture.Create<int>();

            await _target.CheckUnitsOfBrandAtCountryAndNotify(unitList, unitListSnapshot, brand, countryCode, restaurantsCountAtBrand, totalOverall, CancellationToken.None);
        }

        [Test]
        public async Task CheckUnitsOfBrandAtCountryAndNotify_LessUnitsThanAtSnapshot_NoAnyNotification()
        {
            var brand = _fixture.Create<string>();
            var countryCode = _fixture.Create<string>();

            var unitName = _fixture.Create<string>();
            var beginDateWork = DateOnly.FromDateTime(_fixture.Create<DateTime>());

            var unitList = _fixture.Build<UnitInfo>()
                .With(u => u.Name, unitName)
                .With(u => u.BeginDateWork, beginDateWork)
                .CreateMany(1).ToList();

            var deletedUnitInfo = _fixture.Build<UnitInfo>()
                .With(u => u.Name)
                .With(u => u.BeginDateWork, beginDateWork)
                .Create();

            var unitListSnapshot = new List<UnitInfo> { unitList.First(), deletedUnitInfo };

            var restaurantsCountAtBrand = _fixture.Create<int>();
            var totalOverall = _fixture.Create<int>();

            await _target.CheckUnitsOfBrandAtCountryAndNotify(unitList, unitListSnapshot, brand, countryCode, restaurantsCountAtBrand, totalOverall, CancellationToken.None);
        }

        [Test]
        public async Task CheckUnitsOfBrandAtCountryAndNotify_NewUnit_Notification()
        {
            var brand = _fixture.Create<string>();
            var countryCode = _fixture.Create<string>();
            var beginDateWork = DateOnly.FromDateTime(DateTime.Now);
        
            var newUnitAddress = _fixture.Build<AddressDetails>()
                    .With(a => a.LocalityName)
                    .Create();
            var newUnitCoordinates = _fixture.Build<Location>()
                .With(c => c.Latitude)
                .With(c => c.Longitude)
                .Create();
            var newUnitModel = _fixture.Build<UnitInfo>()
                .With(m => m.Name)
                .With(m => m.Address)
                .With(m => m.AddressDetails, newUnitAddress)
                .With(m => m.Location, newUnitCoordinates)
                .With(u => u.BeginDateWork, beginDateWork)
                .Create();
            var oldUnitModel = _fixture.Build<UnitInfo>()
                .With(m => m.Name)
                .With(u => u.BeginDateWork, beginDateWork)
                .Create();

            var unitsList = new List<UnitInfo> { oldUnitModel, newUnitModel };
            var unitListSnapshot = new List<UnitInfo> { oldUnitModel };

            var restaurantsCountAtBrand = _fixture.Create<int>();
            var totalOverall = _fixture.Create<int>();

            var expectedText =
                $"Wow! There is new {brand} in {newUnitModel.AddressDetails?.LocalityName}! You can find it on the map👆 " +
                $"\r\nIt's {restaurantsCountAtBrand} restaurant of {brand} and {totalOverall} of all Dodo brands 🔥";

            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.AreEqual(notification.Payload.Text, expectedText);
                    Assert.AreEqual(notification.Payload.Coordinates.Latitude, newUnitCoordinates.Latitude);
                    Assert.AreEqual(notification.Payload.Coordinates.Longitude, newUnitCoordinates.Longitude);
                })
                .Returns(Task.CompletedTask);

            await _target.CheckUnitsOfBrandAtCountryAndNotify(unitsList, unitListSnapshot, brand, countryCode, restaurantsCountAtBrand, totalOverall, CancellationToken.None);
        }

        [Test]
        public async Task CheckUnitsOfBrandAtCountryAndNotify_OldUnitWithNewDate_Notification()
        {
            var brand = _fixture.Create<string>();
            var countryCode = _fixture.Create<string>();

            var unitName = _fixture.Create<string>();
            var beginDateWork = DateOnly.FromDateTime(DateTime.Now);

            var newUnitAddress = _fixture.Build<AddressDetails>()
                .With(a => a.LocalityName)
                .Create();
            var newUnitCoordinates = _fixture.Build<Location>()
                .With(c => c.Latitude)
                .With(c => c.Longitude)
                .Create();
            var newUnitModel = _fixture.Build<UnitInfo>()
                .With(m => m.Name, unitName)
                .With(m => m.Address)
                .With(m => m.AddressDetails, newUnitAddress)
                .With(m => m.Location, newUnitCoordinates)
                .With(u => u.BeginDateWork, beginDateWork)
                .Create();
            var oldUnitModel = _fixture.Build<UnitInfo>()
                .With(m => m.Name, unitName)
                .Without(u => u.BeginDateWork)
                .Create();

            var unitsList = new List<UnitInfo> { newUnitModel };
            var unitListSnapshot = new List<UnitInfo> { oldUnitModel };

            var restaurantsCountAtBrand = _fixture.Create<int>();
            var totalOverall = _fixture.Create<int>();

            var expectedText =
                $"Wow! There is new {brand} in {newUnitModel.AddressDetails?.LocalityName}! You can find it on the map👆 " +
                $"\r\nIt's {restaurantsCountAtBrand} restaurant of {brand} and {totalOverall} of all Dodo brands 🔥";

            _notificationsServiceMock.Setup(n => n.Save(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback((Notification notification, CancellationToken _) =>
                {
                    Assert.AreEqual(notification.Payload.Text, expectedText);
                    Assert.AreEqual(notification.Payload.Coordinates.Latitude, newUnitCoordinates.Latitude);
                    Assert.AreEqual(notification.Payload.Coordinates.Longitude, newUnitCoordinates.Longitude);
                })
                .Returns(Task.CompletedTask);

            await _target.CheckUnitsOfBrandAtCountryAndNotify(unitsList, unitListSnapshot, brand, countryCode, restaurantsCountAtBrand, totalOverall, CancellationToken.None);
        }
    }
}
