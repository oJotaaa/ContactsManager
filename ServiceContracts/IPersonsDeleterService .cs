using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines operations for managing person records, including adding new persons and retrieving all existing
    /// persons.
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Deletes the person with the specified identifier from the data store.
        /// </summary>
        /// <param name="personID">The unique identifier of the person to delete. If null, the method does not perform any operation.</param>
        /// <returns>true if the person was found and deleted; otherwise, false.</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
