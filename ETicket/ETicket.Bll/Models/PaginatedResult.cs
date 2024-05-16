
namespace ETicket.Bll.Models
{
    public class PaginatedResult<T>
    {
        public int Count { get; set; }

        public int Skip { get; set; }

        public int Limit { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}