using ETicket.Bll.Models;
using ETicket.Db.Domain.Entities;

namespace ETicket.Bll.Services
{
    public interface IVenueService
    {
        Task<PaginatedResult<Venue>> GetVenuesAsync(int skip, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<BusinessSection>> GetSectionsAsync(long venueId, CancellationToken cancellationToken);
    }
}
