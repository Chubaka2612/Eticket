using ETicket.Db.Domain.Enums;

namespace ETicket.Db.Domain.Entities
{
    public class Seat : Entity
    {
        public SeatStatusOption SeatStatusId { get; set; }

        public long RowId { get; set; }

        public int Number { get; set; }

        public OrderItem OrderItem { get; set; } 

        public Row Row { get; set; }

        public SeatStatus SeatStatus { get; set; }
    }
}
