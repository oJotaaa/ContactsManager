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
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;
        private readonly ILogger<PersonsController> _logger;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _personsServiceMock = new Mock<IPersonsService>();
            _countriesServiceMock = new Mock<ICountriesService>();

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;

            _personsService = _personsServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> personResponsesList = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponsesList);
            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponsesList);

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
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);


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
