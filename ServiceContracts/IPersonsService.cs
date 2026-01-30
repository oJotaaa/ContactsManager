using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines operations for managing person records, including adding new persons and retrieving all existing
    /// persons.
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new person into the list of persons. 
        /// </summary>
        /// <param name="personAddRequest">Person to Add</param>
        /// <returns>Returns the same person details, along with newly generated PersonID</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Returns all persons
        /// </summary>
        /// <returns>Return a list of objects of PersonResponse</returns>
        List<PersonResponse> GetAllPersons();

        /// <summary>
        /// Retrieves the person object associated with the specified person identifier.
        /// </summary>
        /// <param name="personID">The unique identifier of the person to retrieve.</param>
        /// <returns>A matching Person Object containing the details of the person if found.</returns>
        PersonResponse? GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Retrieves a list of persons that match the specified search criteria.
        /// </summary>
        /// <param name="searchBy">The name of the property to search by, such as "Name" or "Email". If null or empty, all
        /// persons are returned.</param>
        /// <param name="searchString">The value to search for within the specified property. The search is typically case-insensitive and may
        /// match partial values.</param>
        /// <returns>A list of <see cref="PersonResponse"/> objects that match the search criteria. Returns an empty list if no
        /// persons are found.</returns>
        List<PersonResponse> GetFilteredPersons(string? searchBy, string searchString);

        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortyBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortyBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Updates the details of an existing person using the specified update request.
        /// </summary>
        /// <param name="personUpdateRequest">An object containing the update information for the person</param>
        /// <returns>A response object containing the updated person details and the result of the update operation.</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
