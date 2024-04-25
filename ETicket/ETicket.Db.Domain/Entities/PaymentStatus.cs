using ETicket.Db.Domain.Enums;
using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class PaymentStatus
    {
        public PaymentStatusOption PaymentStatusId { get; set; }

        public string Name { get; set; }

        public List<Payment> Payments { get; set; }
    }
}
