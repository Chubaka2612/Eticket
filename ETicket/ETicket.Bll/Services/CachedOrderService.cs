using ETicket.Bll.Models;
using ETicket.Bll.Services.Caching;

namespace ETicket.Bll.Services
{
    public class CachedOrderService : IOrderService
    {
        private readonly IOrderService _innerService;
        private readonly ICacheService _cache;

        public CachedOrderService(IOrderService innerService, ICacheService cache)
        {
            _innerService = innerService;
            _cache = cache;
        }

        public IEnumerable<BusinessOrderItem> GetCartItems(Guid cartId)
        {
            return _innerService.GetCartItems(cartId);
        }

        public CartState AddItemToCart(Guid cartId, BusinessOrderItem orderItem)
        {
            return _innerService.AddItemToCart(cartId, orderItem);
        }

        public async Task<long> BookCartSeatsAsync(Guid cartId, CancellationToken cancellationToken)
        {
            var eventIds = _innerService.GetCartItems(cartId).Select(item => item.EventId).Distinct().ToList();//for further caching invalidation

            var result = await _innerService.BookCartSeatsAsync(cartId, cancellationToken);

            foreach (var eventId in eventIds)
            {
                var cacheKeyPattern = $"EventSeats_{eventId}_";
                await _cache.RemoveByPrefixAsync(cacheKeyPattern, cancellationToken);
            }
            return result;
        }

        public void DeleteItemFromCart(Guid cartId, long eventId, long seatId)
        {
            _innerService.DeleteItemFromCart(cartId, eventId, seatId);
        }
    }
}
