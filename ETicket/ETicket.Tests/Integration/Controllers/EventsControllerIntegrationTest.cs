using ETicket.Api.Models;
using ETicket.Bll.Models;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using ETicket.Tests.Abstractions;
using ETicket.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ETicket.Tests.Integration.Controllers
{
    public class EventsControllerIntegrationTest : AbstractApiTest
    {
        InMemoryWebFactory webFactory;
        HttpClient httpClient;
    
        [OneTimeSetUp]
        public async Task Setup()
        {
            webFactory = new InMemoryWebFactory("EventsControllerIntegrationTest");
            httpClient = webFactory.CreateNoRedirectClient();
        }

        [Test]
        public async Task GetEvents_ShouldReturnAllEvents()
        {
            // Arrange
            using (var scope = webFactory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                db.Repository<Event>().Add(TestDataFactory.GetStubEvent1());
                db.Repository<Event>().Add(TestDataFactory.GetStubEvent2());
                await db.SaveChangesAsync();
            }

            // Act
            using var response = await httpClient.GetAsync(BASE_EVENTS_ROUTE);

            //Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            var json = await response.Content.ReadAsStringAsync();
            var countriesResponse = CommonUtil.PopulateFromJson<PaginatedResponse<Event>>(json);

            Assert.That(countriesResponse.Items, Is.Not.Null);
            Assert.That(countriesResponse.Items.Select(item => item.Name), 
                Is.EquivalentTo(new List<string>() { TestDataFactory.GetStubEvent1().Name, TestDataFactory.GetStubEvent2().Name }));
        }

        [Test]
        public async Task GetEvents_ShouldReturnEmptyList_WhenNoEventsExist()
        {
            // Arrange
            using (var scope = webFactory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            }

            // Act
            using var response = await httpClient.GetAsync(BASE_EVENTS_ROUTE);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadAsStringAsync();
            var eventsResponse = CommonUtil.PopulateFromJson<PaginatedResponse<Event>>(json);

            Assert.That(eventsResponse.Items, Is.Empty);
        }

        [Test]
        public async Task GetSections_ShouldReturnSeats_ForValidEventVenueSection()
        {
            var seat = TestDataFactory.GetStubRow1Seat1();
            // Arrange
            using (var scope = webFactory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var venue = TestDataFactory.GetStubVenue1();
                db.Repository<Venue>().Add(venue);
                db.Repository<Manifest>().Add(TestDataFactory.GetStubManifest1());
                db.Repository<Event>().Add(TestDataFactory.GetStubEvent1(new List<Venue>() { venue }));
                db.Repository<Section>().Add(TestDataFactory.GetStubSection1());
                db.Repository<Row>().Add(TestDataFactory.GetStubRow1());
                db.Repository<Seat>().Add(seat);
                await db.SaveChangesAsync();
            }

            var testEventId = TestDataFactory.GetStubEvent1().Id;
            var testVenueId = TestDataFactory.GetStubVenue1().Id;
            var testSectionId = TestDataFactory.GetStubSection1().Id;

            // Act
            using var response = await httpClient.GetAsync($"{BASE_EVENTS_ROUTE}/{testEventId}/venues/{testVenueId}/sections/{testSectionId}/seats");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadAsStringAsync();
            var seatsResponse = CommonUtil.PopulateFromJson<IEnumerable<BusinessSeat>>(json);

            Assert.Multiple(() =>
            {
                Assert.That(seatsResponse, Is.Not.Null);
                Assert.That(seatsResponse.Count, Is.EqualTo(1));
                Assert.That(seat.Id, Is.EqualTo(seatsResponse.First().Id));
                Assert.That(seat.Number, Is.EqualTo(seatsResponse.First().Number));
                Assert.That(seat.SeatStatusId, Is.EqualTo(seatsResponse.First().SeatStatus));
                Assert.That(seat.RowId, Is.EqualTo(seatsResponse.First().RowId));
            });
        }

        [Test]
        public async Task GetSections_ShouldReturnBadRequest_ForInvalidEventVenueSection()
        {
            // Arrange
            var invalidEventId = NOT_EXISTING_ENTITY_STUB_ID;
            var invalidVenueId = NOT_EXISTING_ENTITY_STUB_ID;
            var invalidSectionId = NOT_EXISTING_ENTITY_STUB_ID;

            // Act
            using var response = await httpClient.GetAsync($"{BASE_EVENTS_ROUTE}/{invalidEventId}/venues/{invalidVenueId}/sections/{invalidSectionId}/seats");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
