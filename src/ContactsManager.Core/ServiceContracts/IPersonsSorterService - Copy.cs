using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines operations for managing person records, including adding new persons and retrieving all existing
    /// persons.
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortyBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortyBy, SortOrderOptions sortOrder);
    }
}
