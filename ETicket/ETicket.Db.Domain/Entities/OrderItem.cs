namespace ETicket.Db.Domain.Entities
{
    public class OrderItem : Entity
    {
        public long PriceId { get; set; }

        public long SeatId { get; set; }

        public long OrderId { get; set; }

        public Order Order { get; set; }

        public Seat Seat { get; set; }

        public Price Price { get; set; }
    }
}
