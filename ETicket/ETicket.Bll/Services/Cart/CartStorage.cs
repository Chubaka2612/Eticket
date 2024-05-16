using System.Collections.Concurrent;

namespace ETicket.Bll.Services.Cart
{
    public class CartStorage : ICartStorage
    {
        private readonly ConcurrentDictionary<Guid, Cart> cartDictionary = new ConcurrentDictionary<Guid, Cart>();

        public Cart Get(Guid cartId)
        {
            return cartDictionary.TryGetValue(cartId, out var cart) ? cart : 
                throw new ArgumentException($"Cart with Id '{cartId}' was not found");
        }

        public Cart GetOrCreate(Guid cartId)
        {
            return cartDictionary.GetOrAdd(cartId, new Cart { Id = cartId });
        }

        public void Remove(Guid cartId)
        {
            cartDictionary.TryRemove(cartId, out var _);
        }
    }
}