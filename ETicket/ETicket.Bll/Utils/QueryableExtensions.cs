using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Bll.Utils
{
    public static class QueryableExtensions
    {
        public static async Task<T> SingleRequiredAsync<T>(this IQueryable<T> dataSet, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            var entity = await dataSet.SingleOrDefaultAsync(predicate, cancellationToken);
            if (entity is not null)
            {
                return entity;
            }

            throw new KeyNotFoundException($"{typeof(T).Name} where: '{predicate}' is not found");
        }
    }
}