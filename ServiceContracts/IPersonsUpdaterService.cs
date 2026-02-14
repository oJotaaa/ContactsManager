using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines operations for managing person records, including adding new persons and retrieving all existing
    /// persons.
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Updates the details of an existing person using the specified update request.
        /// </summary>
        /// <param name="personUpdateRequest">An object containing the update information for the person</param>
        /// <returns>A response object containing the updated person details and the result of the update operation.</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
