using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country Entity.
    /// </summary>
    public interface ICountriesGetterService
    {
        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries from the list as List of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Retrieves country information for the specified country identifier.
        /// </summary>
        /// <param name="countryID">The unique identifier of the country to retrieve. If null, the method returns null.</param>
        /// <returns>A <see cref="CountryResponse"/> containing the country information if found; otherwise, null.</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}
