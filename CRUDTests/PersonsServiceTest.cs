using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using ServiceContracts.Enums;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        // Private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;

        // Constructor
        public PersonsServiceTest()
        {
            _personService = new PersonsService();
            _countriesService = new CountriesService();
        }

        #region AddPersonTests

        // When we supply null value as PersonAddRequest, ArgumentNullException should be thrown
        [Fact]
        public void AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => _personService.AddPerson(personAddRequest));

        }

        // When we supply null value as PersonName, ArgumentException should be thrown
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            // Act/Assert
            Assert.Throws<ArgumentException>(() => _personService.AddPerson(personAddRequest));
        }

        // When we supply proper person details, person should be added successfully and should return a Person object with PersonID populated
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Person name",
                Email = "person@example.com",
                Address = "Some address",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            // Act
            PersonResponse personResponseFromAdd = _personService.AddPerson(personAddRequest);
            List<PersonResponse> personsList = _personService.GetAllPersons().ToList();

            // Assert
            Assert.True(personResponseFromAdd.PersonID != Guid.Empty);
            Assert.Contains(personResponseFromAdd, personsList);
        }
        #endregion

        #region GetPersonByPersonIDTests

        // If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? personResponse = _personService.GetPersonByPersonID(personID);

            // Assert
            Assert.Null(personResponse);
        }

        // If we supply a valid person id, it should return the corresponding Person object
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            // Act
            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "person@sample.com",
                Address = "Some address",
                CountryID = countryResponse.CountryID,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false
            };
            PersonResponse personResponseFromAdd = _personService.AddPerson(person_request);

            PersonResponse? personResponse = _personService.GetPersonByPersonID(personResponseFromAdd.PersonID);

            // Assert
            Assert.Equal(personResponseFromAdd, personResponse);
        }
        #endregion

        #region GetAllPersonsTests

        // The GetAllPersons should return an empty list when there are no persons added
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            // Act
            List<PersonResponse> personsList = _personService.GetAllPersons();

            // Assert
            Assert.Empty(personsList);
        }

        // First, we add multiple persons, then GetAllPersons should return all added persons
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Indiia" };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Person 1",
                Email = "person1@gmail.com",
                DateOfBirth = DateTime.Parse("1995-05-01"),
                Gender = GenderOptions.Male,
                CountryID = countryResponse1.CountryID,
                Address = "Address 1",
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Person 2",
                Email = "person2@gmail.com",
                DateOfBirth = DateTime.Parse("1992-03-15"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse2.CountryID,
                Address = "Address 2",
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Person 3",
                Email = "person3@gmail.com",
                DateOfBirth = DateTime.Parse("1992-03-15"),
                Gender = GenderOptions.Other,
                CountryID = countryResponse2.CountryID,
                Address = "Address 3",
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3
            };

            List<PersonResponse> addedPersons = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests)
            {
                PersonResponse personResponse = _personService.AddPerson(personAddRequest);
                addedPersons.Add(personResponse);
            }

            List<PersonResponse> personsListFromGet = _personService.GetAllPersons();

            foreach (PersonResponse person in addedPersons)
            {
                // Assert
                Assert.Contains(person, personsListFromGet);
            }
            #endregion
        }
    }
}