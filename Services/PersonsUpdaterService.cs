using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        // Private fields
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        // Constructor
        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            _logger.LogInformation("UpdatePerson of PersonsService");

            // Check if "personUpdateRequest" is not null
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest), "PersonUpdateRequest cannot be null");

            // Validate all properties of "personUpdateRequest"
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Get the matching "Person" object from Database based on "PersonID"
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);

            // Check if matching "Person" object is not null
            if (matchingPerson == null)
            {
                throw new InvalidPersonIDException($"Person with PersonID '{personUpdateRequest.PersonID}' not found");
            }

            // Update all details from "personUpdateRequest" to matching "Person" object
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(matchingPerson);

            // Convert the person object to PersonResponse object and return it
            return matchingPerson.ToPersonResponse();
        }
    }
}
