using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        // Private fields
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        // Constructor
        public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
        }

        private PersonResponse ConvertPersonIntoPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            // Check if "personAddRequest" is not null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest), "PersonAddRequest cannot be null");
            }

            // Model validation for "personAddRequest"
            ValidationHelper.ModelValidation(personAddRequest);

            // Convert "personAddRequest" to "Person" object
            Person person = personAddRequest.ToPerson();

            // Generate a new GUID for PersonID
            person.PersonID = Guid.NewGuid();

            // Then add it into Database
            _db.Persons.Add(person);
            _db.SaveChanges();

            // Return PersonResponse object with generated PersonID and other details
            return ConvertPersonIntoPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _db.Persons.ToList().Select(person => ConvertPersonIntoPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            // Check if "personID" is null
            if (personID == null)
                return null;

            // Get matching person from Database based personID
            Person? person = _db.Persons.FirstOrDefault(person => person.PersonID == personID);
            if (person == null)
                return null;

            // Convert matching person object to PersonResponse and returns it
            return ConvertPersonIntoPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            // Check if searchBy is null or empty
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }

            // Get matching persons from Database based on searchBy and searchString
            switch (searchBy) 
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.PersonName) &&
                                         person.PersonName
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Email) &&
                                         person.Email
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons
                        .Where(person => person.DateOfBirth != null &&
                                         person.DateOfBirth.Value.ToString("dd MMMM yyyy")
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons
                        .Where(person => person.Gender != null &&
                                         person.Gender
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Country) &&
                                          person.Country
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Address) &&
                                         person.Address
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                default:
                    matchingPersons = allPersons;
                    break;
            }

            // Return all matching PersonResponse objects
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortyBy, SortOrderOptions sortOrder)
        {
            // Check if sortBy is null or empty
            if (string.IsNullOrEmpty(sortyBy))
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons = (sortyBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                    allPersons.OrderBy(person => person.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            // Check if "personUpdateRequest" is not null
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest), "PersonUpdateRequest cannot be null");

            // Validate all properties of "personUpdateRequest"
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Get the matching "Person" object from Database based on "PersonID"
            Person? matchingPerson = _db.Persons.FirstOrDefault(person => person.PersonID == personUpdateRequest.PersonID);

            // Check if matching "Person" object is not null
            if (matchingPerson == null)
            {
                throw new ArgumentException($"Person with PersonID '{personUpdateRequest.PersonID}' not found");
            }

            // Update all details from "personUpdateRequest" to matching "Person" object
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            _db.SaveChanges();

            // Convert the person object to PersonResponse object and return it
            return ConvertPersonIntoPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            // Check if "personID" is null
            if (personID == null)
                return false;

            // Get the matching "Person" object from Database based on "personID"
            Person? person = _db.Persons.FirstOrDefault(person => person.PersonID == personID);

            // Check if matching "Person" object is not null
            if (person == null)
                return false;

            // Delete the matching "Person" object from Database
            _db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personID));

            _db.SaveChanges();

            // Return boolean value indicating whether the deletion was successful or not
            return true;
        }
    }
}
