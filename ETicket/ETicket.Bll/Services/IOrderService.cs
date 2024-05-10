using ETicket.Bll.Models;


namespace ETicket.Bll.Services
{
    public interface IOrderService
    {
        IEnumerable<BusinessOrderItem> GetCartItems(Guid cartId);

        public CartState AddItemToCart(Guid cartId, BusinessOrderItem orderItem);

        void DeleteItemFromCart(Guid cartId, long eventId, long seatId);

        Task<long> BookCartSeatsAsync(Guid cartId, CancellationToken cancellationToken);
    }
}
