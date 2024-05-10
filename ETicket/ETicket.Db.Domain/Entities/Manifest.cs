using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Manifest : Entity
    {
        public long VenueId { get; set; }

        public Venue Venue { get; set; }

        public ICollection<Section> Sections { get; set; }
    }
}
