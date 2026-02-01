using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        // Constructor
        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize) 
            {
                _countries.AddRange(new List<Country>() {
                    new Country { CountryID = Guid.Parse("EC878DA4-9CAD-4B0E-A28C-6D16F7950BF9"), CountryName = "USA" },
                    new Country { CountryID = Guid.Parse("E11DF336-E385-48BE-BDC5-DD2CF905282F"), CountryName = "Canada" },
                    new Country { CountryID = Guid.Parse("B006CEBF-C0ED-4EB9-B175-727B861621D9"), CountryName = "UK" },
                    new Country { CountryID = Guid.Parse("C41BDA78-434E-4782-B74B-34408B0E2A66"), CountryName = "India" },
                    new Country { CountryID = Guid.Parse("1A904F09-F6AA-44FD-A8AA-DE79E46A7FFD"), CountryName = "Australia" }
                });
            }
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
            if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            // Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            // Generate CountryID
            country.CountryID = Guid.NewGuid();

            // Add country to the _countries list
            _countries.Add(country);

            return country.ToCountryResponse();

        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? countryResponseFromList = _countries.FirstOrDefault(country => country.CountryID == countryID);

            if (countryResponseFromList == null)
                return null;

            return countryResponseFromList.ToCountryResponse();
        }
    }
}
