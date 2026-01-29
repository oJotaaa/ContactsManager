using ServiceContracts;
using Services;
using ServiceContracts.DTO;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountryTests
        // When CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _countriesService.AddCountry(request));
        }


        // When the CountryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _countriesService.AddCountry(request));
        }

        // When the CountryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        // When you supply proper country name, AddCountry should return CountryResponse with valid CountryID
        [Fact]
        public void AddCountry_ProperCountryDetailsl()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

            // Act
            CountryResponse response = _countriesService.AddCountry(request);
            List<CountryResponse> all_countries = _countriesService.GetAllCountries();

            // Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, all_countries);
        }

        #endregion

        #region GetAllCountriesTests

        // The list of countries should be empty by default (before adding any country)
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            // Acts
            List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

            // Assert
            Assert.Empty(actual_country_response_list);
        }

        // The getAllCountries should return all countries added via AddCountry
        [Fact]
        public void GetAllCountries_AddFewCountries()
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
                country_list_from_add_country.Add(_countriesService.AddCountry(country_request));
            }

            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            // Assert
            // Read each element from country_list_from_add_country
            foreach (CountryResponse expected_country in country_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }

        #endregion
    }
}
