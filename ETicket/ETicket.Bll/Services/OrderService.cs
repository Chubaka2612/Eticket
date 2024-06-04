using ETicket.Bll.Models;
using ETicket.Bll.Services.Cart;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using ETicket.Db.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Bll.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICartStorage _cartStorage;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Seat> _seatRepository;

        private readonly IRepository<Payment> _paymentRepository;

        public OrderService(ICartStorage cartStorage, IUnitOfWork unitOfWork)
        {
            _cartStorage = cartStorage;
            _unitOfWork = unitOfWork;
            _seatRepository = _unitOfWork.Repository<Seat>();
            _paymentRepository = _unitOfWork.Repository<Payment>();
        }

        public IEnumerable<BusinessOrderItem> GetCartItems(Guid cartId)
        {
            var cart = _cartStorage.Get(cartId) ?? throw new ArgumentException($"Cart with id {cartId} doesn't exist");

            return cart.Items.Values.Select(item => new BusinessOrderItem()
            {
                EventId = item.EventId,
                SeatId = item.SeatId,
                PriceId = item.PriceId,
                UserId = item.UserId,
            }).ToList();
        }

        public CartState AddItemToCart(Guid cartId, BusinessOrderItem orderItem)
        {
            //TODO: no validation BusinessOrderItem.SeatId or any other ids with ids from DB. Probably not backenf validation
            var cart = _cartStorage.GetOrCreate(cartId);
            var cartItemToAdd = new CartItem(
                orderItem.EventId,
                orderItem.SeatId,
                orderItem.PriceId,
                orderItem.UserId);
            var isCartItemAdded = cart.TryAddItem(cartItemToAdd);

            if (!isCartItemAdded)
            {
                throw new ArgumentException($"Can't add Cart Item with eventId: {cartItemToAdd.EventId} and seatId: {cartItemToAdd.SeatId}" +
                    $" since it's already added to cart with id: {cartId}");
            }
            return new CartState()
            {
                Id = cartId,
                TotalAmount = cart.Items.Count
            };
        }

        public void DeleteItemFromCart(Guid cartId, long eventId, long seatId)
        {
            var cart = _cartStorage.Get(cartId) ?? throw new ArgumentException($"Cart with id {cartId} doesn't exist");

            // if no cart items in cart - remove cart as well
            if (cart.Items.Count != 0)
            {
                var isCartItemDeleted = cart.TryRemove(seatId);
                if (!isCartItemDeleted)
                {
                    throw new ArgumentException($"Can't delete Cart Item with eventId: {eventId} and seatId {seatId}" +
                        $" since it was not found in cart with id: {cartId}");
                }
            }
            if (cart.Items.Count == 0) { _cartStorage.Remove(cartId); }
        }

        public async Task<long> BookCartSeatsAsync(Guid cartId, CancellationToken cancellationToken)
        {
            using var transaction = _unitOfWork.BeginTransaction();

            Payment payment;

            try
            {
                var cart = _cartStorage.Get(cartId) ?? throw new ArgumentException($"Cart with id: '{cartId}' doesn't exist");

                var cartItems = cart.Items.Values.ToArray();

                var seatIds = cartItems.Select(item => item.SeatId).ToList();

                //TODO: check for empty
                var seats = await _seatRepository.Queryable()
                    .Where(seat => seatIds.Contains(seat.Id))
                    .ToListAsync(cancellationToken); 

                foreach (var seat in seats)
                {
                    if (seat.SeatStatusId != SeatStatusOption.Available)
                    {
                        throw new InvalidOperationException($"Cannot book a seat with the id '{seat.Id}' since it's already 'Booked'.");
                    }

                    seat.SeatStatusId = SeatStatusOption.Booked;
                }
                var cartItemMap = cartItems.ToDictionary(item => item.SeatId);
                
                payment = new Payment
                {
                    PaymentStatusId = PaymentStatusOption.New,
                    OrderItems = seats.Select(s => new OrderItem
                    {
                        SeatId = s.Id,
                        EventId = cartItemMap[s.Id].EventId,
                        PriceId = cartItemMap[s.Id].PriceId,
                        UserId = cartItemMap[s.Id].UserId,
                        CreatedAt = DateTime.UtcNow,
                    }).ToList(),
                    Date = DateTime.UtcNow,
                };
                _paymentRepository.Add(payment);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            //remove from inMemory storage
            _cartStorage.Remove(cartId);

            return payment.Id;
        }
    }
}
