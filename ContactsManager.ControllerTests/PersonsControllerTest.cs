using AutoFixture;
using Castle.Core.Logging;
using ContactsManager.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsManagerTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;

        private readonly ICountriesGetterService _countriesService;

        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;

        private readonly Mock<ICountriesGetterService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;
        private readonly ILogger<PersonsController> _logger;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();

            _countriesServiceMock = new Mock<ICountriesGetterService>();

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;

            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;

            _countriesService = _countriesServiceMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> personResponsesList = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_countriesService, _logger, _personsGetterService, _personsAdderService, _personsSorterService, _personsUpdaterService, _personsDeleterService);

            _personsGetterServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponsesList);
            _personsSorterServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponsesList);

            // Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(personResponsesList);
        }
        #endregion

        #region Create
        [Fact]
        public async Task Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();

            PersonResponse personResponse = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            _personsAdderServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_countriesService, _logger, _personsGetterService, _personsAdderService, _personsSorterService, _personsUpdaterService, _personsDeleterService);

            // Act
            IActionResult result = await personsController.Create(personAddRequest);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Persons");
        }
        #endregion
    }
}
