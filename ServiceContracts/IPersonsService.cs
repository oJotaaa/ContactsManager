using ServiceContracts.DTO;

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

    }
}
