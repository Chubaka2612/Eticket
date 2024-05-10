using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class OrderItem : Entity
    {
        public long UserId { get; set; }

        public long PaymentId { get; set; }

        public long EventId { get; set; }

        public long PriceId { get; set; }

        public long SeatId { get; set; }

        public Seat Seat { get; set; }

        public Price Price { get; set; }

        public User User { get; set; }

        public Event Event { get; set; }

        public Payment Payment { get; set; }

    }
}
