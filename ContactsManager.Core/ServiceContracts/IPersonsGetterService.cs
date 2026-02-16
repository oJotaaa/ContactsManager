using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines operations for managing person records, including adding new persons and retrieving all existing
    /// persons.
    /// </summary>
    public interface IPersonsGetterService
    {
        /// <summary>
        /// Returns all persons
        /// </summary>
        /// <returns>Return a list of objects of PersonResponse</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Retrieves the person object associated with the specified person identifier.
        /// </summary>
        /// <param name="personID">The unique identifier of the person to retrieve.</param>
        /// <returns>A matching Person Object containing the details of the person if found.</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Retrieves a list of persons that match the specified search criteria.
        /// </summary>
        /// <param name="searchBy">The name of the property to search by, such as "Name" or "Email". If null or empty, all
        /// persons are returned.</param>
        /// <param name="searchString">The value to search for within the specified property. The search is typically case-insensitive and may
        /// match partial values.</param>
        /// <returns>A list of <see cref="PersonResponse"/> objects that match the search criteria. Returns an empty list if no
        /// persons are found.</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Return persons as CSV
        /// </summary>
        /// <returns>Returns the memory stream with CSV</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Generates an Excel file containing a list of persons and returns it as a memory stream.
        /// </summary>
        /// <returns>A <see cref="MemoryStream"/> containing the Excel file data for the persons list. The stream is positioned
        /// at the beginning.</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
