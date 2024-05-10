using System.Collections;

namespace ETicket.Bll.Services.Cart
{
    public class Cart: IEnumerable<CartItem>
    {
        public Guid Id { get; set; }

        public IDictionary<long, CartItem> Items { get; private set; } = new Dictionary<long, CartItem>();

        public int TotalItems()
        {
            return Items.Count;
        }
     
        public IEnumerator<CartItem> GetEnumerator()
        {
           return Items.Values.GetEnumerator();
        }

        public bool TryAddItem(CartItem item)
        { 
            return Items.TryAdd(item.SeatId, item);
        }

        public bool TryRemove(long seatId)
        { 
            return Items.Remove(seatId);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
