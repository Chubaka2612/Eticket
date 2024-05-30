using ETicket.Bll.Models;
using ETicket.Bll.Services.Cashing;
using ETicket.Db.Domain.Entities;

namespace ETicket.Bll.Services
{
    public class CachedEventService : IEventService
    {
        private readonly IEventService _innerService;
        private readonly ICacheService _cache;

        public CachedEventService(IEventService innerService, ICacheService cache)
        {
            _innerService = innerService;
            _cache = cache;
        }

        public async Task<IEnumerable<BusinessSeat>> GetEventSeatsBySectionAsync(long eventId, long sectionId, long venueId, CancellationToken cancellationToken)
        {
            string cacheKey = $"EventSeats_{eventId}_{sectionId}_{venueId}";
            var cachedSeats = await _cache.GetAsync<IEnumerable<BusinessSeat>>(cacheKey, cancellationToken);

            if (cachedSeats != null)
            {
                return cachedSeats;
            }

            var result = await _innerService.GetEventSeatsBySectionAsync(eventId, sectionId, venueId, cancellationToken);
            await _cache.SetAsync(cacheKey, result, cancellationToken);
            return result;
        }

        public async Task<PaginatedResult<Event>> GetEventsAsync(int skip, int limit, CancellationToken cancellationToken)
        {
            string cacheKey = $"Events_{skip}_{limit}";
            var cachedEvents = await _cache.GetAsync<PaginatedResult<Event>>(cacheKey, cancellationToken);

            if (cachedEvents != null)
            {
                return cachedEvents;
            }

            var result = await _innerService.GetEventsAsync(skip, limit, cancellationToken);
            await _cache.SetAsync(cacheKey, result, cancellationToken);
            return result;
        }
    }
}
