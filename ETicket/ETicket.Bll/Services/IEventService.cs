using ETicket.Bll.Models;
using ETicket.Db.Domain.Entities;

namespace ETicket.Bll.Services
{
    public interface IEventService
    {
        Task<PaginatedResult<Event>> GetEventsAsync(int skip, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<Seat>> GetEventSeatsBySectionAsync(long eventId, long sectionId, long venueId, CancellationToken cancellationToken);
    }
}
