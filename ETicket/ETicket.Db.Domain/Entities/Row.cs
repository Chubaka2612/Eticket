using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Row : Entity
    {
        public long SectionId { get; set; }

        public int Number { get; set; }

        public Section Section { get; set; }

        public ICollection<Seat> Seats { get; set; }
    }
}
