using ETicket.Bll.Models;
using ETicket.Bll.Utils;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETicket.Bll.Services
{
    public class VenueService : BaseService, IVenueService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Venue> _venueRepository;

        private readonly IRepository<Section> _sectionRepository;

        public VenueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _venueRepository = _unitOfWork.Repository<Venue>();
            _sectionRepository = _unitOfWork.Repository<Section>();
        }

        public async Task<IEnumerable<Section>> GetSectionsAsync(long venueId, CancellationToken cancellationToken)
        {
            var venue = await _venueRepository.Queryable(s => s.Manifest)
                .SingleRequiredAsync(v => v.Id == venueId, cancellationToken);

            var sections = await _sectionRepository.Queryable()
                .Where(s => s.ManifestId == venue.Manifest.Id)
                .ToListAsync(cancellationToken);

            return sections;
        }

        public async Task<PaginatedResult<Venue>> GetVenuesAsync(int skip, int limit, CancellationToken cancellationToken)
        {
            var filter = _venueRepository.Queryable();
            return await GetPaginatedResult(
                filter,
                venue => venue.CreatedAt,
                skip,
                limit,
                cancellationToken);
        }
    }
}
