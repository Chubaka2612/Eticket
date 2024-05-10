using System;
using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Event : Entity
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Venue> Venues { get; set; }

        public ICollection<EventVenue> EventVenues { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = null!;
    }
}
