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

        // Constructor
        public PersonsServiceTest()
        {
            _personService = new PersonsService();
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
    }
}
