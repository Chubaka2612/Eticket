using ETicket.Bll.Models;
using ETicket.Bll.Utils;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using ETicket.Db.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Bll.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Payment> _paymnetRepository;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _paymnetRepository = _unitOfWork.Repository<Payment>();
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

            try
            {
                var payment = await _paymnetRepository.Queryable()
                    .Where(p => p.Id == paymentId)
                    .Include(p => p.OrderItems)
                    .ThenInclude(p => p.Seat)
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
        }
    }
}
