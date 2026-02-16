using ContactsManager.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactsManager.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ILogger<PersonCreateAndEditPostActionFilter> _logger;

        public PersonCreateAndEditPostActionFilter(ICountriesGetterService countriesGetterService, ILogger<PersonCreateAndEditPostActionFilter> logger)
        {
            _countriesGetterService = countriesGetterService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
                    personsController.ViewBag.Countries = countries;
                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).SelectMany(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personAddRequest"];
                    context.Result = personsController.View(personRequest);
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }

            // After Logic
            _logger.LogInformation("In after logic of PersonsCreateAndEdit Action filter");
        }
    }
}
