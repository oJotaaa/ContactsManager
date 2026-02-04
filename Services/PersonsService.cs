using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        // Private fields
        private readonly ApplicationDbContext _db;
        private readonly ICountriesService _countriesService;

        // Constructor
        public PersonsService(ApplicationDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
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
            await _db.SaveChangesAsync();

            // Return PersonResponse object with generated PersonID and other details
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _db.Persons.Include("Country").ToListAsync();

            return persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            // Check if "personID" is null
            if (personID == null)
                return null;

            // Get matching person from Database based personID
            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.PersonID == personID);
            if (person == null)
                return null;

            // Convert matching person object to PersonResponse and returns it
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await GetAllPersons();
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

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortyBy, SortOrderOptions sortOrder)
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

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            // Check if "personUpdateRequest" is not null
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest), "PersonUpdateRequest cannot be null");

            // Validate all properties of "personUpdateRequest"
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Get the matching "Person" object from Database based on "PersonID"
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(person => person.PersonID == personUpdateRequest.PersonID);

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

            await _db.SaveChangesAsync();

            // Convert the person object to PersonResponse object and return it
            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            // Check if "personID" is null
            if (personID == null)
                return false;

            // Get the matching "Person" object from Database based on "personID"
            Person? person = await _db.Persons.FirstOrDefaultAsync(person => person.PersonID == personID);

            // Check if matching "Person" object is not null
            if (person == null)
                return false;

            // Delete the matching "Person" object from Database
            _db.Persons.Remove(await _db.Persons.FirstAsync(temp => temp.PersonID == personID));

            await _db.SaveChangesAsync();

            // Return boolean value indicating whether the deletion was successful or not
            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvWriter.NextRecord();

            List<PersonResponse> persons = _db.Persons
                .Include("Country")
                .Select(temp => temp.ToPersonResponse()).ToList();

            foreach (PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth);
                else
                    csvWriter.WriteField("");
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream)) 
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date of Birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Country";
                worksheet.Cells["G1"].Value = "Address";
                worksheet.Cells["H1"].Value = "Receive News Letters";

                using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = _db.Persons
                    .Include("Country")
                    .Select(person => person.ToPersonResponse())
                    .ToList();

                foreach (PersonResponse person in persons)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("dd-MM-yyyy");
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Country;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();

                memoryStream.Position = 0;

                return memoryStream;
            }
        }
    }
}
