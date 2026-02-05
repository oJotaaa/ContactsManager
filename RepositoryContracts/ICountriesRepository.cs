using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Asynchronously adds a new country to the data store.
        /// </summary>
        /// <param name="country">The country to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Asynchronously retrieves a list of all available countries.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Country"/>
        /// objects representing all countries. The list will be empty if no countries are available.</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Asynchronously retrieves the country that matches the specified country identifier.
        /// </summary>
        /// <param name="countryID">The unique identifier of the country to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Country"/> if
        /// found; otherwise, <see langword="null"/>.</returns>
        Task<Country?> GetCountryByCountryID(Guid countryID);

        /// <summary>
        /// Asynchronously retrieves a country that matches the specified country name.
        /// </summary>
        /// <param name="countryName">The name of the country to search for. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the matching <see
        /// cref="Country"/> if found; otherwise, <see langword="null"/>.</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
