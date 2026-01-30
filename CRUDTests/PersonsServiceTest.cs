using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;

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
            _personService = new PersonsService();
            _countriesService = new CountriesService();
            _testOutputHelper = testOutputHelper;
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
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

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

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            List<PersonResponse> personsListFromGet = _personService.GetAllPersons();

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
        public void GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

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

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            List<PersonResponse> personsListFromSearch = _personService.GetFilteredPersons(nameof(Person.PersonName), "");

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
        public void GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

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
                PersonResponse personResponse = _personService.AddPerson(personAddRequest);
                addedPersons.Add(personResponse);
            }

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Act
            List<PersonResponse> personsListFromSearch = _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

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
        public void GetSortedPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

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
                PersonResponse personResponse = _personService.AddPerson(personAddRequest);
                addedPersons.Add(personResponse);
            }

            // print added persons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in addedPersons)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            // Get all persons for reference
            List<PersonResponse> allPersons = _personService.GetAllPersons();

            // Act
            List<PersonResponse> personsListFromSort = _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            // print persons from GetAllPersons for debugging
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in personsListFromSort)
            {
                _testOutputHelper.WriteLine($"Added Person: {person.ToString()}");
            }

            addedPersons.OrderByDescending(temp => temp.PersonName).ToList();

            // Assert
            for (int i = 0; i < addedPersons.Count; i++)
            {
                Assert.Equal(addedPersons[i], personsListFromSort[i]);
            }
        }
        #endregion
    }
}