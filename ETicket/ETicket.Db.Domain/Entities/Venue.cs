using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Venue : Entity
    {
        public string Name { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public ICollection<Event> Events { get; set; }

        public ICollection<EventVenue> EventVenues { get; set; }

        public Manifest Manifest { get; set; }
    }
}
