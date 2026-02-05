using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        // Private fields
        private readonly IPersonsService _personService;

        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        // Constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            _personService = new PersonsService(_personsRepository);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPersonTests

        // When we supply null value as PersonAddRequest, ArgumentNullException should be thrown
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Act/Assert
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);

            };

            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        // When we supply null value as PersonName, ArgumentException should be thrown
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            // When PersonRepository.AddPerson is called, it has to return the same "person" object
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            // Act/Assert
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);

            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When we supply proper person details, person should be added successfully and should return a Person object with PersonID populated
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample@example.com")
                .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            // If we supply any argument value to the AddPerson method, it should return the same return value
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            // Act
            PersonResponse personResponseFromAdd = await _personService.AddPerson(personAddRequest);
            personResponseExpected.PersonID = personResponseFromAdd.PersonID;

            // Assert
            personResponseFromAdd.PersonID.Should().NotBe(Guid.Empty);
            personResponseFromAdd.Should().Be(personResponseExpected);

        }
        #endregion

        #region GetPersonByPersonIDTests

        // If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? personResponse = await _personService.GetPersonByPersonID(personID);

            // Assert
            personResponse.Should().BeNull();
        }

        // If we supply a valid person id, it should return the corresponding Person object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "sample@example.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse personResponseExpected = person.ToPersonResponse(); 

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act
            PersonResponse? personResponseFromGet = await _personService.GetPersonByPersonID(person.PersonID);

            // Assert
            personResponseFromGet.Should().Be(personResponseExpected);
        }
        #endregion

        #region GetAllPersonsTests

        // The GetAllPersons should return an empty list when there are no persons added
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            // Arrange
            var persons = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            // Act
            List<PersonResponse> personsList = await _personService.GetAllPersons();

            // Assert
            personsList.Should().BeEmpty();
        }

        // First, we add multiple persons, then GetAllPersons should return all added persons
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            // Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),
            };

            List<PersonResponse> PersonResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in PersonResponseListExpected)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            // Act
            List<PersonResponse> personsListFromGet = await _personService.GetAllPersons();

            // print persons from GetAllPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromGet)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Assert
            personsListFromGet.Should().BeEquivalentTo(PersonResponseListExpected);
        }
        #endregion

        #region GetFilteredPersonsTests

        // If the search text is empty and search is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            // Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),
            };

            List<PersonResponse> PersonResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in PersonResponseListExpected)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }
            
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            // Act
            List<PersonResponse> personsListFromSearch = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            // print persons from GetAllPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromSearch)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Assert
            personsListFromSearch.Should().BeEquivalentTo(PersonResponseListExpected);
        }

        // First we will add few persons; then we will search based on person name with some search string; it should return matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            // Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),
            };

            List<PersonResponse> PersonResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in PersonResponseListExpected)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            // Act
            List<PersonResponse> personsListFromSearch = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            // print persons from GetAllPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromSearch)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Assert
            personsListFromSearch.Should().BeEquivalentTo(PersonResponseListExpected);
        }
        #endregion

        #region GetSortedPersonsTests

        // When we sort based on PersonName in DESC order, it should return persons sorted in descending order of PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            // Arrange
            // Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.Email, "sample3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),
            };

            List<PersonResponse> PersonResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in PersonResponseListExpected)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Get all persons for reference
            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            // Act
            List<PersonResponse> personsListFromSort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            // print persons from GetSortedPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromSort)
            {
                _testOutputHelper.WriteLine($"Sort Person: {person.ToString()}");
            }

            // Assert
            personsListFromSort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePersonTests

        // When we supply null as PersonUpdateRequest, ArgumentNullException should be thrown
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            // Act
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When we supply invalid personID in PersonUpdateRequest, ArgumentException should be thrown
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

            // Act
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When personName is null in PersonUpdateRequest, ArgumentException should be thrown
        [Fact]
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse personResponse = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            // Act
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        // First, add a new person and try to update the person details; it should update the person details successfully
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse personResponseExpected = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = personResponseExpected.ToPersonUpdateRequest();

            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);


            // Act
            PersonResponse personResponseFromUpdate = await _personService.UpdatePerson(personUpdateRequest);

            // Assert
            personResponseFromUpdate.Should().Be(personResponseExpected);
        }
        #endregion

        #region DeletePersonTests

        // If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act
            bool deleteResult = await _personService.DeletePerson(person.PersonID);

            // Assert
            deleteResult.Should().BeTrue();
        }

        // If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            // Act
            bool deleteResult = await _personService.DeletePerson(Guid.NewGuid());

            // Assert
            deleteResult.Should().BeFalse();
        }
        #endregion
    }
}