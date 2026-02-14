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
    public class PersonsDeleterService : IPersonsDeleterService
    {
        // Private fields
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        // Constructor
        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            _logger.LogInformation("DeletePerson of PersonsService");

            // Check if "personID" is null
            if (personID == null)
                return false;

            // Get the matching "Person" object from Database based on "personID"
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            // Check if matching "Person" object is not null
            if (person == null)
                return false;

            // Delete the matching "Person" object from Database
            await _personsRepository.DeletePersonByPersonID(personID.Value);

            // Return boolean value indicating whether the deletion was successful or not
            return true;
        }
    }
}
