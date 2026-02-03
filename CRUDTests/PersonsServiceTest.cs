using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        // Private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        // Constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
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
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.AddPerson(personAddRequest));

        }

        // When we supply null value as PersonName, ArgumentException should be thrown
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.AddPerson(personAddRequest));
        }

        // When we supply proper person details, person should be added successfully and should return a Person object with PersonID populated
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
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
            PersonResponse personResponseFromAdd = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> personsList = await _personService.GetAllPersons();

            // Assert
            Assert.True(personResponseFromAdd.PersonID != Guid.Empty);
            Assert.Contains(personResponseFromAdd, personsList);
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
            Assert.Null(personResponse);
        }

        // If we supply a valid person id, it should return the corresponding Person object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

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
            PersonResponse personResponseFromAdd = await _personService.AddPerson(person_request);

            PersonResponse? personResponse = await _personService.GetPersonByPersonID(personResponseFromAdd.PersonID);

            // Assert
            Assert.Equal(personResponseFromAdd, personResponse);
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
            Assert.Empty(personsList);
        }

        // First, we add multiple persons, then GetAllPersons should return all added persons
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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


            foreach (PersonResponse person in addedPersons)
            {
                // Assert
                Assert.Contains(person, personsListFromGet);
            }
        }
        #endregion

        #region GetFilteredPersonsTests

        // If the search text is empty and search is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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


            foreach (PersonResponse person in addedPersons)
            {
                // Assert
                Assert.Contains(person, personsListFromSearch);
            }
        }

        // First we will add few persons; then we will search based on person name with some search string; it should return matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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
                PersonName = "Mary",
                Email = "person2@gmail.com",
                DateOfBirth = DateTime.Parse("1992-03-15"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse2.CountryID,
                Address = "Address 2",
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Rahman",
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
            foreach (PersonResponse person in addedPersons)
            {
                if (person.PersonName != null)
                {
                    if (person.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person, personsListFromSearch);
                    }
                }
            }
        }
        #endregion

        #region GetSortedPersonsTests

        // When we sort based on PersonName in DESC order, it should return persons sorted in descending order of PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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
                PersonName = "Mary",
                Email = "person2@gmail.com",
                DateOfBirth = DateTime.Parse("1992-03-15"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse2.CountryID,
                Address = "Address 2",
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Rahman",
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

            addedPersons = addedPersons.OrderByDescending(temp => temp.PersonName).ToList();

            // Assert
            for (int i = 0; i < addedPersons.Count; i++)
            {
                Assert.Equal(addedPersons[i], personsListFromSort[i]);
            }
        }
        #endregion

        #region UpdatePersonTests

        // When we supply null as PersonUpdateRequest, ArgumentNullException should be thrown
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.UpdatePerson(personUpdateRequest));
        }

        // When we supply invalid personID in PersonUpdateRequest, ArgumentException should be thrown
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePerson(personUpdateRequest));
        }

        // When personName is null in PersonUpdateRequest, ArgumentException should be thrown
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = countryResponse.CountryID,
                Email = "John@email.com",
                Gender = GenderOptions.Male
            };
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePerson(personUpdateRequest));
        }

        // First, add a new person and try to update the person details; it should update the person details successfully
        [Fact]
        public async Task UpdatePerson_PersonFullDetails()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = countryResponse.CountryID,
                Address = "Some address",
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Email = "John@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse personResponseFromAdd = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Willian";
            personUpdateRequest.Email = "Updated@gmail.com";

            PersonResponse personResponseFromUpdate = await _personService.UpdatePerson(personUpdateRequest);

            // Act
            PersonResponse? personResponseFromGet = await _personService.GetPersonByPersonID(personResponseFromUpdate.PersonID);

            // Assert
            Assert.Equal(personResponseFromUpdate, personResponseFromGet);
        }
        #endregion

        #region DeletePersonTests

        // If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                Email = "John@email.com",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                Address = "Some address",
                DateOfBirth = DateTime.Parse("1990-01-01"),
                ReceiveNewsLetters = true
            };
            PersonResponse personResponseFromAdd = await _personService.AddPerson(personAddRequest);

            // Act
            bool deleteResult = await _personService.DeletePerson(personResponseFromAdd.PersonID);

            // Assert
            Assert.True(deleteResult);
        }

        // If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            // Act
            bool deleteResult = await _personService.DeletePerson(Guid.NewGuid());

            // Assert
            Assert.False(deleteResult);
        }
        #endregion
    }
}