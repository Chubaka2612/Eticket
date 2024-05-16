using ETicket.Db.Domain.Enums;

namespace ETicket.Bll.Models
{
    public class PaymentState
    {
        public long Id { get; set; }

        public PaymentStatusOption Status { get; set; }
    }
}
