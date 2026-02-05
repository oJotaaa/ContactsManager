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

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        // Private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        // Constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            var dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(dbContext);

            _personService = new PersonsService(dbContext, _countriesService);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPersonTests

        // When we supply null value as PersonAddRequest, ArgumentNullException should be thrown
        [Fact]
        public async Task AddPerson_NullPerson()
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
        public async Task AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            // Act/Assert
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);

            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When we supply proper person details, person should be added successfully and should return a Person object with PersonID populated
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@example.com").Create();

            // Act
            PersonResponse personResponseFromAdd = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> personsList = await _personService.GetAllPersons();

            // Assert
            personResponseFromAdd.PersonID.Should().NotBe(Guid.Empty);
            personsList.Should().Contain(personResponseFromAdd);
        }
        #endregion

        #region GetPersonByPersonIDTests

        // If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
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
        public async Task GetPersonByPersonID_WithPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            // Act
            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample@example.com")
                .Create();

            PersonResponse personResponseFromAdd = await _personService.AddPerson(person_request);

            PersonResponse? personResponse = await _personService.GetPersonByPersonID(personResponseFromAdd.PersonID);

            // Assert
            personResponse.Should().Be(personResponseFromAdd);
        }
        #endregion

        #region GetAllPersonsTests

        // The GetAllPersons should return an empty list when there are no persons added
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            // Act
            List<PersonResponse> personsList = await _personService.GetAllPersons();

            // Assert
            personsList.Should().BeEmpty();
        }

        // First, we add multiple persons, then GetAllPersons should return all added persons
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample1@example.com")
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample2@example.com")
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample3@example.com")
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3
            };

            List<PersonResponse> addedPersons = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                addedPersons.Add(personResponse);
            }

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            List<PersonResponse> personsListFromGet = await _personService.GetAllPersons();

            // print persons from GetAllPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromGet)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Assert
            personsListFromGet.Should().BeEquivalentTo(addedPersons);
        }
        #endregion

        #region GetFilteredPersonsTests

        // If the search text is empty and search is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample1@example.com")
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample2@example.com")
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample3@example.com")
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3
            };

            List<PersonResponse> addedPersons = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                addedPersons.Add(personResponse);
            }

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            List<PersonResponse> personsListFromSearch = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            // print persons from GetAllPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromSearch)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Assert
            personsListFromSearch.Should().BeEquivalentTo(addedPersons);
        }

        // First we will add few persons; then we will search based on person name with some search string; it should return matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "mary")
                .With(temp => temp.Email, "sample2@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "scott")
                .With(temp => temp.Email, "sample3@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3
            };

            List<PersonResponse> addedPersons = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                addedPersons.Add(personResponse);
            }

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Act
            List<PersonResponse> personsListFromSearch = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            // print persons from GetAllPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromSearch)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Assert
            personsListFromSearch.Should().OnlyContain(temp => temp.PersonName!.Contains("ma", StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region GetSortedPersonsTests

        // When we sort based on PersonName in DESC order, it should return persons sorted in descending order of PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Smith")
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "sample2@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "sample3@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3
            };

            List<PersonResponse> addedPersons = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                addedPersons.Add(personResponse);
            }

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
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
        public async Task UpdatePerson_NullPerson()
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
        public async Task UpdatePerson_InvalidPersonID()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePerson(personUpdateRequest));
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When personName is null in PersonUpdateRequest, ArgumentException should be thrown
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Smith")
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

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
        public async Task UpdatePerson_PersonFullDetails()
        {
            // Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Smith")
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse personResponseFromAdd = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Willian";
            personUpdateRequest.Email = "Updated@gmail.com";

            PersonResponse personResponseFromUpdate = await _personService.UpdatePerson(personUpdateRequest);

            // Act
            PersonResponse? personResponseFromGet = await _personService.GetPersonByPersonID(personResponseFromUpdate.PersonID);

            // Assert
            personResponseFromUpdate.Should().Be(personResponseFromGet);
        }
        #endregion

        #region DeletePersonTests

        // If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Smith")
                .With(temp => temp.Email, "sample1@example.com")
                .With(temp => temp.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse personResponseFromAdd = await _personService.AddPerson(personAddRequest);

            // Act
            bool deleteResult = await _personService.DeletePerson(personResponseFromAdd.PersonID);

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