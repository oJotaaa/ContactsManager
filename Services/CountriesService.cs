using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;

        // Constructor
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            // Validation: Check if countryAddRequest is null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            // Validation: Check if CountryName is null or empty
            if (string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            // Validation: Check for duplicate CountryName (case-insensitive)
            if (_db.Countries.Count(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            // Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            // Generate CountryID
            country.CountryID = Guid.NewGuid();

            // Add country to the _countries list
            _db.Countries.Add(country);
            _db.SaveChanges();

            return country.ToCountryResponse();

        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? countryResponseFromList = _db.Countries.FirstOrDefault(country => country.CountryID == countryID);

            if (countryResponseFromList == null)
                return null;

            return countryResponseFromList.ToCountryResponse();
        }
    }
}
