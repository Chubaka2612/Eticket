using ETicket.Bll.Models;
using ETicket.Db.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace ETicket.Bll.Services
{
    public class BaseService
    {
        protected static async Task<PaginatedResult<T>> GetPaginatedResult<T>(IQueryable<T> filter, Expression<Func<T, object>> orderBySelector, int skip, int limit, CancellationToken cancellationToken) where T : Entity
        {
            var items = await filter
                .OrderBy(orderBySelector)
                .Skip(skip)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var count = await filter.CountAsync(cancellationToken);

            var result = new PaginatedResult<T>
            {
                Count = count,
                Skip = skip,
                Limit = limit,
                Items = items,
            };

            return result;
        }
    }
}
