using Azure.Core;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            var countriesInitialData = new List<Country>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            var dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesService = new CountriesService(dbContext);
        }

        #region AddCountryTests
        // When CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Act
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        // When the CountryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            // Act
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When the CountryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

            // Act
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When you supply proper country name, AddCountry should return CountryResponse with valid CountryID
        [Fact]
        public async Task AddCountry_ProperCountryDetailsl()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

            // Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> all_countries = await _countriesService.GetAllCountries();

            // Assert
            response.CountryID.Should().NotBe(Guid.Empty);
            all_countries.Should().Contain(response);
        }

        #endregion

        #region GetAllCountriesTests

        // The list of countries should be empty by default (before adding any country)
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // Acts
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            // Assert
            actual_country_response_list.Should().BeEmpty();
        }

        // The getAllCountries should return all countries added via AddCountry
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName = "USA" },
                new CountryAddRequest() { CountryName = "UK" },
            };

            // Act
            List<CountryResponse> country_list_from_add_country = new List<CountryResponse>();

            foreach (CountryAddRequest country_request in country_request_list)
            {
                country_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
            }

            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            // Assert
            actualCountryResponseList.Should().BeEquivalentTo(country_list_from_add_country);
        }

        #endregion

        #region GetCountryByCountryIDTests

        // If we supply null countryID, GetCountryByCountryID should return null
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            // Arrange
            Guid? countrID = null;

            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryID(countrID);

            // Assert
            countryResponse.Should().BeNull();
        }

        // If we supply a valid countryID, it should return the corresponding CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            // Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "China" };
            CountryResponse countryResponseFromAddCountry = await _countriesService.AddCountry(countryAddRequest);

            // Act
            CountryResponse? countryResponseFromGet = await _countriesService.GetCountryByCountryID(countryResponseFromAddCountry.CountryID);

            // Assert
            countryResponseFromGet.Should().Be(countryResponseFromAddCountry);
        }
        #endregion
    }
}
