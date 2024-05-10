namespace ETicket.Db.Domain.Entities
{
    public class EventVenue
    {
        public long VenueId { get; set; }

        public long EventId { get; set; }

        public Venue Venue { get; set; }

        public Event Event { get; set; }
    }
}
