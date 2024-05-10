using ETicket.Bll.Models;
namespace ETicket.Bll.Services
{
    public interface IPaymentService
    {
        Task<PaymentState> GetPaymentStateAsync(long paymentId, CancellationToken cancellationToken);

        Task CompletePaymentAsync(long paymentId, CancellationToken cancellationToken);

        Task FailPaymentAsync(long paymentId, CancellationToken cancellationToken);
    }
}
