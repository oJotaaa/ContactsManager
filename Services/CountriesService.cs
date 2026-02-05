using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        // Constructor
        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            // Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            // Generate CountryID
            country.CountryID = Guid.NewGuid();

            // Add country to the _countries list
            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();

        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> countries = await _countriesRepository.GetAllCountries();
            return countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? countryResponseFromList = await _countriesRepository.GetCountryByCountryID(countryID.Value);

            if (countryResponseFromList == null)
                return null;

            return countryResponseFromList.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if (await _countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country() { CountryName = countryName };
                            await _countriesRepository.AddCountry(country);

                            countriesInserted++;
                        }
                    }
                }

            }

            return countriesInserted;
        }
    }
}
