
namespace ETicket.Bll.Services.Cart
{
    public interface ICartStorage
    {
        Cart Get(Guid cartId);

        Cart GetOrCreate(Guid cartId);

        void Remove(Guid cartId);
    }
}