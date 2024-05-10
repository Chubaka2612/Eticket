
namespace ETicket.Bll.Services.Cart
{
    public class CartItem
    {
        public CartItem(long eventId, long seatId, long priceId, long userId, DateTime addedAt)
        {
            EventId = eventId;
            SeatId = seatId;
            PriceId = priceId;
            UserId = userId;
            AddedAt = addedAt;
        }

        public long UserId { get; }

        public long EventId { get; }
     
        public long SeatId { get; }
      
        public long PriceId { get; }

        public DateTime AddedAt { get; }
    }
}
