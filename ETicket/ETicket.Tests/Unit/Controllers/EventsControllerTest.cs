using ETicket.Api.Controllers;
using ETicket.Api.Models;
using ETicket.Bll.Models;
using ETicket.Bll.Services;
using ETicket.Db.Domain.Entities;
using ETicket.Tests.Abstractions;
using ETicket.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace ETicket.Tests.Unit.Controllers
{
    [TestFixture]
    public class EventsControllerTest : AbstractTest
    {
        private Mock<IEventService> _mockEventService;
        private EventsController _controller;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _mockEventService = new Mock<IEventService>();
            _controller = new EventsController(_mockEventService.Object);
        }

        [Test]
        public async Task GetEvents_ShouldReturnOk_WithPaginatedResponse()
        {
            // Arrange
            var request = new PaginatedRequest { Skip = DEFAULT_SKIP, Limit = DEFAULT_LIMIT };
            var cancellationToken = CancellationToken.None;
            var paginatedResult = new PaginatedResult<Event>
            {
                Skip = DEFAULT_SKIP,
                Limit = DEFAULT_LIMIT,
                Count = 2,
                Items = TestDataFactory.GetStubEvents()
            };

            _mockEventService
                .Setup(service => service.GetEventsAsync(request.Skip, request.Limit, cancellationToken))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetEvents(request, cancellationToken);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var response = okResult.Value as PaginatedResponse<Event>;
            Assert.That(response, Is.Not.Null);

            Assert.That(response.Metadata.Count, Is.EqualTo(2));
            Assert.That(response.Metadata.Skip, Is.EqualTo(DEFAULT_SKIP));
            Assert.That(response.Metadata.Limit, Is.EqualTo(DEFAULT_LIMIT));
            Assert.That(response.Items.Any(item => item.Name == "Test Event 1"));
            Assert.That(response.Items.Any(item => item.Name == "Test Event 2"));
        }

        [Test]
        public async Task GetSeatsBySection_ShouldReturnOkResult_WithSeatsResponse()
        {
            // Arrange
            var eventId = ENTITY_STUB_ID;
            var venueId = ENTITY_STUB_ID;
            var sectionId = ENTITY_STUB_ID;
            var cancellationToken = CancellationToken.None;
            var seatsResponse = TestDataFactory.GetStubSeats();

            _mockEventService
                .Setup(service => service.GetEventSeatsBySectionAsync(eventId, venueId, sectionId, cancellationToken))
                .ReturnsAsync(seatsResponse);

            // Act
            var result = await _controller.GetSections(eventId, venueId, sectionId, cancellationToken);

            //Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult.Value as List<BusinessSeat>;
            Assert.That(response, Is.Not.Null);
            Assert.That(seatsResponse, Is.EqualTo(response));
        }

        [Test]
        public async Task GetSeatsBySection_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var eventId = ENTITY_STUB_ID;
            var venueId = ENTITY_STUB_ID;
            var sectionId = ENTITY_STUB_ID;
            var cancellationToken = CancellationToken.None;

            _mockEventService
                .Setup(service => service.GetEventSeatsBySectionAsync(eventId, venueId, sectionId, cancellationToken))
                .ThrowsAsync(new Exception("Failed to get seats with provided parameters"));

            // Act
            var result = await _controller.GetSections(eventId, venueId, sectionId, cancellationToken);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That("Failed to get seats with provided parameters", Is.EqualTo(badRequestResult.Value));
        }

        [Test]
        public async Task GetEvents_ShouldReturnOkResult_WithEmptyArray()
        {
            // Arrange
            var request = new PaginatedRequest { Skip = DEFAULT_SKIP, Limit = DEFAULT_LIMIT };
            var cancellationToken = CancellationToken.None;
            var paginatedResult = new PaginatedResult<Event>
            {
                Skip = DEFAULT_SKIP,
                Limit = DEFAULT_LIMIT,
                Count = 0,
                Items = new List<Event>()
            };

            _mockEventService.Setup(service => service.GetEventsAsync(request.Skip, request.Limit, cancellationToken))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetEvents(request, cancellationToken);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var response = okResult.Value as PaginatedResponse<Event>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Items, Is.Empty);
        }
    }
}
