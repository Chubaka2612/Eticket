using System;
using System.Collections.Generic;
using ETicket.Db.Dal;
using ETicket.Db.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace ETicket.Tests.Unit.DataFixtures
{
    public class ETicketDbDataFixture : IDisposable
    {
        protected ETicketDbContext ETicketDbContext { get; private set; }

        public ETicketDbContext GetDataContext => ETicketDbContext;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual ETicketDbDataFixture BuildInMemoryDatabase(string name)
        {
            var options = new DbContextOptionsBuilder<ETicketDbContext>()
                .UseInMemoryDatabase(name)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            ETicketDbContext = new ETicketDbContext(options);
            return this;
        }

        public virtual ETicketDbDataFixture AddEvents(IEnumerable<Event> events)
        {
            ETicketDbContext.Events.AddRange(events);
            ETicketDbContext.SaveChanges();

            return this;
        }

        public virtual ETicketDbDataFixture AddVenues(IEnumerable<Venue> venues)
        {
            ETicketDbContext.Venues.AddRange(venues);
            ETicketDbContext.SaveChanges();

            return this;
        }

        public virtual ETicketDbDataFixture AddManifest(IEnumerable<Manifest> manifets)
        {
            ETicketDbContext.Manifests.AddRange(manifets);
            ETicketDbContext.SaveChanges();

            return this;
        }

        public virtual ETicketDbDataFixture AddSections(IEnumerable<Section> sections)
        {
            ETicketDbContext.Sections.AddRange(sections);
            ETicketDbContext.SaveChanges();

            return this;
        }

        public virtual ETicketDbDataFixture AddRows(IEnumerable<Row> rows)
        {
            ETicketDbContext.Rows.AddRange(rows);
            ETicketDbContext.SaveChanges();

            return this;
        }

        public virtual ETicketDbDataFixture AddSeats(IEnumerable<Seat> seats)
        {
            ETicketDbContext.Seats.AddRange(seats);
            ETicketDbContext.SaveChanges();

            return this;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ETicketDbContext?.Dispose();
            }
        }
    }
}