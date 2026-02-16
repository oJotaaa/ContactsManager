using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines operations for managing person records, including adding new persons and retrieving all existing
    /// persons.
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds a new person into the list of persons. 
        /// </summary>
        /// <param name="personAddRequest">Person to Add</param>
        /// <returns>Returns the same person details, along with newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}
