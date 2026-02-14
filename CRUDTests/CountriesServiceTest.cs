using AutoFixture;
using Azure.Core;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Diagnostics.Metrics;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        // Private fields
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesAdderService _countriesAdderService;

        private readonly Mock<ICountriesRepository> _countrieRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _countrieRepositoryMock = new Mock<ICountriesRepository>();

            _countriesRepository = _countrieRepositoryMock.Object;

            _countriesGetterService = new CountriesGetterService(_countriesRepository);
            _countriesAdderService = new CountriesAdderService(_countriesRepository);
        }

        #region AddCountryTests
        // When CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Act
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        // When the CountryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            // Arrange
            CountryAddRequest? countryAddRequest = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();

            // Mock
            Country country = countryAddRequest.ToCountry();
            _countrieRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            // Act
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(countryAddRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When the CountryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            // Arrange
            CountryAddRequest? request1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest? request2 = _fixture.Create<CountryAddRequest>();

            // Mock
            Country country = request1.ToCountry();
            _countrieRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
            _countrieRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country);

            // Act
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request1);
                await _countriesAdderService.AddCountry(request2);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When you supply proper country name, AddCountry should return CountryResponse with valid CountryID
        [Fact]
        public async Task AddCountry_FullCountryDetails_ToBeSuccessful()
        {
            // Arrange
            CountryAddRequest? request = _fixture.Create<CountryAddRequest>();

            // Mock
            Country country = request.ToCountry();
            CountryResponse countryResponseExpected = country.ToCountryResponse(); 
            _countrieRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            // Act
            CountryResponse responseFromAdd = await _countriesAdderService.AddCountry(request);
            countryResponseExpected.CountryID = responseFromAdd.CountryID;

            // Assert
            responseFromAdd.CountryID.Should().NotBe(Guid.Empty);
            responseFromAdd.Should().Be(countryResponseExpected);
        }

        #endregion

        #region GetAllCountriesTests

        // The list of countries should be empty by default (before adding any country)
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeSuccessful()
        {
            // Arrange
            var countries = new List<Country>();
            _countrieRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            // Acts
            List<CountryResponse> actual_country_response_list = await _countriesGetterService.GetAllCountries();

            // Assert
            actual_country_response_list.Should().BeEmpty();
        }

        // The getAllCountries should return all countries added via AddCountry
        [Fact]
        public async Task GetAllCountries_WithFewCountries_ToBeSuccessful()
        {
            // Arrange
            List<Country> countries = new List<Country>()
            {
                _fixture.Create<Country>(),
                _fixture.Create<Country>(),
                _fixture.Create<Country>()
            };

            // Act
            List<CountryResponse> CountrieListResponseExpected = countries.Select(temp => temp.ToCountryResponse()).ToList();

            // Mock
            _countrieRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            List<CountryResponse> actualCountryResponseList = await _countriesGetterService.GetAllCountries();

            // Assert
            actualCountryResponseList.Should().BeEquivalentTo(CountrieListResponseExpected);
        }

        #endregion

        #region GetCountryByCountryIDTests

        // If we supply null countryID, GetCountryByCountryID should return null
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            // Arrange
            Guid? countrID = null;

            // Act
            CountryResponse? countryResponse = await _countriesGetterService.GetCountryByCountryID(countrID);

            // Assert
            countryResponse.Should().BeNull();
        }

        // If we supply a valid countryID, it should return the corresponding CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            // Arrange
            Country country = _fixture.Create<Country>();
            CountryResponse CountryResponseExpected = country.ToCountryResponse();

            // Mock
            _countrieRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(country);

            // Act
            CountryResponse? countryResponseFromGet = await _countriesGetterService.GetCountryByCountryID(country.CountryID);

            // Assert
            countryResponseFromGet.Should().Be(CountryResponseExpected);
        }
        #endregion
    }
}
