using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryContracts
{
    /// <summary>
    /// Defines a contract for accessing and managing person entities in a data store.
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Asynchronously adds a new person to the data store.
        /// </summary>
        /// <param name="person">The person to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added person, including any
        /// updated properties such as a generated identifier.</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Asynchronously retrieves a list of all persons.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Person"/>
        /// objects representing all persons. The list will be empty if no persons are found.</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Asynchronously retrieves a person record that matches the specified unique identifier.
        /// </summary>
        /// <param name="personID">The unique identifier of the person to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Person"/> object
        /// if found; otherwise, <see langword="null"/>.</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);

        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Deletes the person record that matches the specified unique identifier.
        /// </summary>
        /// <param name="personID">The unique identifier of the person to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the person
        /// was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);

        /// <summary>
        /// Updates the details of an existing person asynchronously.
        /// </summary>
        /// <param name="person">The person object containing the updated information. The object's identifier must correspond to an existing
        /// person. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated person object.</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
