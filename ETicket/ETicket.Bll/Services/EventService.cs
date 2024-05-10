using ETicket.Bll.Models;
using ETicket.Bll.Utils;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Bll.Services
{
    public class EventService : BaseService, IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Event> _eventRepository;

        private readonly IRepository<Seat> _seatRepository;
    
        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _eventRepository = _unitOfWork.Repository<Event>();
            _seatRepository = _unitOfWork.Repository<Seat>();
        }

        public async Task<IEnumerable<Seat>> GetEventSeatsBySectionAsync(long eventId, long sectionId, long venueId, CancellationToken cancellationToken)
        {
            var eevent = await _eventRepository.Queryable(new[] 
            { 
                nameof(Event.Venues),
                $"{nameof(Event.Venues)}.{nameof(Venue.Manifest)}" 
            }).SingleRequiredAsync(s => s.Id == eventId, cancellationToken);

            var venue = eevent.Venues.FirstOrDefault(s => s.Id == eventId) ?? throw new KeyNotFoundException($"Venue with id {venueId} not found") ;

            var seats = await _seatRepository.Queryable(new[]
            {
                    nameof(Seat.Row),
                    $"{nameof(Seat.Row)}.{nameof(Row.Section)}",
                      $"{nameof(Seat.OrderItem)}",
                      $"{nameof(Seat.OrderItem)}.{nameof(OrderItem.Price)}"
                })
                .Where(s => s.Row.Section.ManifestId == venue.Manifest.Id)
                .ToListAsync(cancellationToken);

            return seats;
        }

        public async Task<PaginatedResult<Event>> GetEventsAsync(int skip, int limit, CancellationToken cancellationToken)
        {
            var filter = _eventRepository.Queryable();
            return await GetPaginatedResult(
                filter,
                venue => venue.CreatedAt,
                skip,
                limit,
                cancellationToken);
        }
    }
}
