using ETicket.Bll.Services;
using ETicket.Db.Dal;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using ETicket.Tests.Abstractions;
using ETicket.Tests.Unit.DataFixtures;
using ETicket.Tests.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ETicket.Tests.Unit.Services
{
    [TestFixture]
    public class EventServiceTest : AbstractTest
    {
        private ETicketDbDataFixture _eventsSeedDataFixture;
        private IUnitOfWork _unitOfWork;
        private EventService _eventService;
        private ETicketDbContext _context;

        [SetUp]
        public void Setup()
        {
            _eventsSeedDataFixture = new ETicketDbDataFixture();
            _context = _eventsSeedDataFixture
                .BuildInMemoryDatabase("EventServiceTests")
                .GetDataContext;

            _unitOfWork = new ETicketUnitOfWork(_context);
            _eventService = new EventService(_unitOfWork);
        }

        [Test]
        public async Task GetEventSeatsBySectionAsync_ShouldReturnSeats()
        {
            // Arrange
            var eventId = ENTITY_STUB_ID;
            var sectionId = ENTITY_STUB_ID;
            var venueId = ENTITY_STUB_ID;
            var cancellationToken = CancellationToken.None;

            var venues = new List<Venue>() { TestDataFactory.GetStubVenue1() };
            _eventsSeedDataFixture.AddVenues(venues);
            _eventsSeedDataFixture.AddManifest(new List<Manifest>() { TestDataFactory.GetStubManifest1() });
            _eventsSeedDataFixture.AddEvents(new List<Event>() { TestDataFactory.GetStubEvent1(venues) });
            _eventsSeedDataFixture.AddSections(new List<Section>() { TestDataFactory.GetStubSection1() });
            _eventsSeedDataFixture.AddRows(new List<Row>() { TestDataFactory.GetStubRow1() });
            var seats = new List<Seat>() { TestDataFactory.GetStubRow1Seat1(), TestDataFactory.GetStubRow1Seat2() };
            _eventsSeedDataFixture.AddSeats(seats);

            // Act
            var seatsResult = await _eventService.GetEventSeatsBySectionAsync(eventId, sectionId, venueId, cancellationToken);

            // Assert
            Assert.That(seatsResult, Is.Not.Null);
            Assert.That(2, Is.EqualTo(seatsResult.ToList().Count));
            Assert.That(seatsResult.Select(seat => seat.Id).ToList(), Is.EquivalentTo(seats.Select(seat => seat.Id).ToList()));
        }

        [Test]
        public async Task GetEventsAsync_ShouldReturnPaginatedResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var events = TestDataFactory.GetStubEvents();
            _eventsSeedDataFixture.AddEvents(events);

            // Act
            var result = await _eventService.GetEventsAsync(DEFAULT_SKIP, DEFAULT_LIMIT, cancellationToken);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(2, Is.EqualTo(result.Count));
            Assert.That(2, Is.EqualTo(result.Items.Count()));
            Assert.That(events, Is.EqualTo(result.Items)); // Sorted by CreatedAt descending
        }

        [Test]
        public void GetEventSeatsBySectionAsync_ShouldThrowKeyNotFoundException_WhenEventNotFound()
        {
            // Arrange
            var eventId = NOT_EXISTING_ENTITY_STUB_ID;
            var sectionId = ENTITY_STUB_ID;
            var venueId = ENTITY_STUB_ID;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                  await _eventService.GetEventSeatsBySectionAsync(eventId, sectionId, venueId, cancellationToken));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _eventsSeedDataFixture.Dispose();
        }

    }
}
