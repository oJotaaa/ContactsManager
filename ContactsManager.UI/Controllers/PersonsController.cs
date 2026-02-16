using ContactsManager.Filters.ActionFilters;
using ContactsManager.Filters.AuthorizationFilter;
using ContactsManager.Filters.ExceptionFilters;
using ContactsManager.Filters.ResourceFilter;
using ContactsManager.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsManager.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;

        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(ICountriesGetterService countriesGetterService, ILogger<PersonsController> logger, IPersonsGetterService personsGetterService, IPersonsAdderService personsAdderService, IPersonsSorterService personsSorterService, IPersonsUpdaterService personsUpdaterService, IPersonsDeleterService personsDeleterService)
        {
            _logger = logger;
            _countriesGetterService = countriesGetterService;

            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsSorterService = personsSorterService;
            _personsUpdaterService = personsUpdaterService;
            _personsDeleterService = personsDeleterService;
        }

        [Route("[action]")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            // Log
            _logger.LogInformation("Index action method of PersonsController");
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

            // Search
            List<PersonResponse> persons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            // Sort
            List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);

            return View(sortedPersons);
        }

        // Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() 
            });
            return View(); 
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            // Call the service method
            PersonResponse personResponse = await _personsAdderService.AddPerson(personRequest);

            // Navigate to Index() action method
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString()});

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest) 
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            PersonResponse updatedPerson = await _personsUpdaterService.UpdatePerson(personRequest);
            return RedirectToAction("Index", "Persons");

        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            await _personsDeleterService.DeletePerson(personUpdateRequest.PersonID);

            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            // Get list of persons
            List<PersonResponse> persons = await _personsGetterService.GetAllPersons();

            // Return view as pdf
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins()
                {
                    Top = 20,
                    Right = 20,
                    Bottom = 20,
                    Left = 20
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
