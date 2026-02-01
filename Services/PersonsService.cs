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
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        // Constructor
        public PersonsService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();

            if (initialize)
            {
                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("B6BDB29E-0853-405C-9DBF-519795093816"),
                    PersonName = "Chadd",
                    Email = "csabey0@wordpress.com",
                    DateOfBirth = DateTime.Parse("1998-04-29"),
                    Gender = "Male",
                    Address = "14916 Village Center",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("EC878DA4-9CAD-4B0E-A28C-6D16F7950BF9")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("466CAD53-3537-4B83-8003-3D6F55048A91"),
                    PersonName = "Moll",
                    Email = "mfarnon1@mtv.com",
                    DateOfBirth = DateTime.Parse("1992-08-18"),
                    Gender = "Female",
                    Address = "53902 Derek Lane",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("E11DF336-E385-48BE-BDC5-DD2CF905282F")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("F4010EFD-28F4-4D45-BC7B-F91FD0BC3AB9"),
                    PersonName = "Heloise",
                    Email = "hdobbing2@cafepress.com",
                    DateOfBirth = DateTime.Parse("1993-02-15"),
                    Gender = "Female",
                    Address = "3867 Charing Cross Junction",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("B006CEBF-C0ED-4EB9-B175-727B861621D9")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("F0A988F1-A9A0-4855-8A25-1D422C9393CF"),
                    PersonName = "Faber",
                    Email = "frubenov3@taobao.com",
                    DateOfBirth = DateTime.Parse("1990-01-30"),
                    Gender = "Male",
                    Address = "7585 Coleman Avenue",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("C41BDA78-434E-4782-B74B-34408B0E2A66")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("28C51834-D502-46D3-8990-BB4B639ECD07"),
                    PersonName = "Fernando",
                    Email = "fmichelotti4@google.it",
                    DateOfBirth = DateTime.Parse("1991-11-29"),
                    Gender = "Male",
                    Address = "48 Mendota Center",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("1A904F09-F6AA-44FD-A8AA-DE79E46A7FFD")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("D0071BD4-AB7D-47DA-AB4C-9C8714A67312"),
                    PersonName = "Nonie",
                    Email = "nspilsburie5@printfriendly.com",
                    DateOfBirth = DateTime.Parse("2000-08-05"),
                    Gender = "Female",
                    Address = "90 Oak Valley Lane",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("EC878DA4-9CAD-4B0E-A28C-6D16F7950BF9")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("2903BE48-7D62-47E5-84D6-AE78E94A66DE"),
                    PersonName = "Celine",
                    Email = "cleeman6@vinaora.com",
                    DateOfBirth = DateTime.Parse("1994-01-18"),
                    Gender = "Female",
                    Address = "8 Northview Pass",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("E11DF336-E385-48BE-BDC5-DD2CF905282F")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("CF84866A-D35E-4272-98A5-0C0202C1DD93"),
                    PersonName = "Obed",
                    Email = "osrawley7@thetimes.co.uk",
                    DateOfBirth = DateTime.Parse("2000-08-14"),
                    Gender = "Male",
                    Address = "03 Clemons Hill",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("B006CEBF-C0ED-4EB9-B175-727B861621D9")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("84035C10-A2D6-4B14-BDDE-0DEC4DDFE06E"),
                    PersonName = "Letty",
                    Email = "ldansie8@tripadvisor.com",
                    DateOfBirth = DateTime.Parse("1990-03-20"),
                    Gender = "Female",
                    Address = "53 Upham Point",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("C41BDA78-434E-4782-B74B-34408B0E2A66")
                });

                _persons.Add(new Person
                {
                    PersonID = Guid.Parse("EE9D55F0-2112-49C6-9F21-D91860C74E66"),
                    PersonName = "Wade",
                    Email = "wlandman9@yolasite.com",
                    DateOfBirth = DateTime.Parse("1996-05-20"),
                    Gender = "Male",
                    Address = "886 Eagle Crest Crossing",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("1A904F09-F6AA-44FD-A8AA-DE79E46A7FFD")
                });



            }
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
            return ConvertPersonIntoPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPersons(string? searchBy, string searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            // Check if searchBy is null or empty
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }

            // Get matching persons from List<Persons> based on searchBy and searchString
            switch (searchBy) 
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.PersonName) &&
                                         person.PersonName
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Email) &&
                                         person.Email
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons
                        .Where(person => person.DateOfBirth != null &&
                                         person.DateOfBirth.Value.ToString("dd MMMM yyyy")
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPersons
                        .Where(person => person.Gender != null &&
                                         person.Gender
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.CountryID):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Country) &&
                                          person.Country
                                         .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Address):
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

            // Get the matching "Person" object from List<Persons> based on "PersonID"
            Person? matchingPerson = _persons.FirstOrDefault(person => person.PersonID == personUpdateRequest.PersonID);

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

            // Convert the person object to PersonResponse object and return it
            return ConvertPersonIntoPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            // Check if "personID" is null
            if (personID == null)
                return false;

            // Get the matching "Person" object from List<Persons> based on "personID"
            Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);

            // Check if matching "Person" object is not null
            if (person == null)
                return false;

            // Delete the matching "Person" object from List<Persons>
            _persons.RemoveAll(temp => temp.PersonID == personID);

            // Return boolean value indicating whether the deletion was successful or not
            return true;
        }
    }
}
