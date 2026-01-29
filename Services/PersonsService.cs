using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        // Private fields
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        // Constructor
        public PersonsService()
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
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

            // Then add it into List<Persons>
            _persons.Add(person);

            // Return PersonResponse object with generated PersonID and other details
            return ConvertPersonIntoPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            // Check if "personID" is null
            if (personID == null)
                return null;

            // Get matching person from List<Person> based personID
            Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);
            if (person == null)
                return null;

            // Convert matching person object to PersonResponse and returns it
            return person.ToPersonResponse();
        }
    }
}
