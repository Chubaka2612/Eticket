using ETicket.Bll.Models;
using ETicket.Bll.Services.Notifications;
using ETicket.Bll.Utils;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using ETicket.Db.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ETicket.Messaging.Models;
using ETicket.Messaging.Enums;


namespace ETicket.Bll.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Payment> _paymnetRepository;
        private readonly INotificationProducer _notificationProducer;

        public PaymentService(IUnitOfWork unitOfWork, INotificationProducer notificationProducer)
        {
            _unitOfWork = unitOfWork;
            _paymnetRepository = _unitOfWork.Repository<Payment>();
            _notificationProducer = notificationProducer;
        }

        public async Task<PaymentState> GetPaymentStateAsync(long paymentId, CancellationToken cancellationToken)
        {
            var payment = await _paymnetRepository.Queryable()
               .SingleRequiredAsync(p => p.Id == paymentId, cancellationToken);

            return new PaymentState
            {
                Id = paymentId,
                Status = payment.PaymentStatusId
            };
        }

        public async Task CompletePaymentAsync(long paymentId, CancellationToken cancellationToken)
        {
            await UpdatePaymentAndSeatsStatusTransaction(paymentId, PaymentStatusOption.Completed, SeatStatusOption.Sold, cancellationToken);
        }

        public async Task FailPaymentAsync(long paymentId, CancellationToken cancellationToken)
        {
            await UpdatePaymentAndSeatsStatusTransaction(paymentId, PaymentStatusOption.Failed, SeatStatusOption.Available, cancellationToken);
        }

        private async Task UpdatePaymentAndSeatsStatusTransaction(long paymentId, PaymentStatusOption toUpdatePaymentStatus, SeatStatusOption toUpdateSeatStatus, CancellationToken cancellationToken)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            Payment payment = null;
            try
            {
                 payment = await _paymnetRepository.Queryable()
                    .Where(p => p.Id == paymentId)
                    .Include(p => p.OrderItems)
                    .ThenInclude(p => p.Price)
                    .Include(p => p.OrderItems)
                    .ThenInclude(p => p.Event)
                    .ThenInclude(e => e.EventVenues)
                    .ThenInclude(e => e.Venue)
                    .Include(p => p.OrderItems)
                    .ThenInclude(p => p.Price)
                    .Include(p => p.OrderItems)
                    .ThenInclude(p => p.Seat)
                    .ThenInclude(p => p.Row)
                    .FirstOrDefaultAsync(cancellationToken) ?? throw new ArgumentException($"There is no payment with id: {paymentId}");

                if (payment.PaymentStatusId != PaymentStatusOption.New)
                {
                    throw new Exception($"Status of payment with id{paymentId} can't be changed to '{toUpdatePaymentStatus}'" +
                        $" since it's not in '{PaymentStatusOption.New}'");
                }

                payment.PaymentStatusId = toUpdatePaymentStatus;
                foreach (var orderItem in payment.OrderItems)
                {
                    orderItem.Seat.SeatStatusId = toUpdateSeatStatus;
                }

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            try
            {
                // Produce notification and send to Message Queue 
                var notification = BuildNotification(payment);

                await _notificationProducer.PublishMessageAsync(notification, "notificationsqueue", cancellationToken);
            }
            catch 
            {
                throw;
            }
        }

        public Notification BuildNotification(Payment payment)
        {
            // Build array of Ticket objects from payment's order items
            var tickets = payment.OrderItems.Select(orderItem => new Ticket
            {
                EventTitle = orderItem.Event.Name,
                EventDateAndTime = orderItem.Event.Date,
                Venue = orderItem.Event.Venues.First().Name,
                SeatNumber = orderItem.Seat.Number,
                RowNumber = orderItem.Seat.Row.Number,
                TicketPrice = orderItem.Price.Amount
            }).ToList();

            // Prepare notification message
            var message = new Notification
            {
                Id = Guid.NewGuid(),
                Option = Operation.Checkedout,
                Parameters = new Dictionary<string, string>
                {
                    { "CustomerEmail", "7309386e-2397-4469-a64f-d0447d69b8c1@mailslurp.mx" },
                    { "CustomerName", "7309386e-2397-4469-a64f-d0447d69b8c1@mailslurp.mx" }
                },
                Timestamp = DateTime.UtcNow,
                Content = tickets 
            };

            return message;
        }
    }
}
