namespace ETicket.Api.Models 
{
    public class PaginatedResponseMetadata
    {
        
        public int Count { get; set; }

        public int Skip { get; set; }

        public int Limit { get; set; }
    }
}