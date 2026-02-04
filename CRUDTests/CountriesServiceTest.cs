using ServiceContracts;
using Services;
using ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;
using Entities;
using Moq;
using EntityFrameworkCoreMock;

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

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countriesService.AddCountry(request));
        }


        // When the CountryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _countriesService.AddCountry(request));
        }

        // When the CountryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
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
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, all_countries);
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
            Assert.Empty(actual_country_response_list);
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
            // Read each element from country_list_from_add_country
            foreach (CountryResponse expected_country in country_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
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
            Assert.Null(countryResponse);
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
            Assert.Equal(countryResponseFromAddCountry, countryResponseFromGet);
        }
        #endregion
    }
}
