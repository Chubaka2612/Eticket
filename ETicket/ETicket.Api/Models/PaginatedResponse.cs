

namespace ETicket.Api.Models
{
    public class PaginatedResponse<T>
    {
        public PaginatedResponse(int skip, int limit, int count, IEnumerable<T> items)
        {
            Metadata = new PaginatedResponseMetadata
            {
                Count = count,
                Skip = skip,
                Limit = limit,
            };

            Items = items;
        }

        public PaginatedResponseMetadata Metadata { get; }

        public IEnumerable<T> Items { get; }
    }
}