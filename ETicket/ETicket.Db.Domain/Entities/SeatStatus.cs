using ETicket.Db.Domain.Enums;
using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class SeatStatus
    {
        public SeatStatusOption SeatStatusId { get; set; }

        public string Name { get; set; }

        public List<Seat> Seats { get; set; }
    }
}
