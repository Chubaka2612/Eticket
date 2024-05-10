using ETicket.Db.Domain.Enums;
using System;

namespace ETicket.Db.Domain.Entities
{
    public class Payment : Entity
    {
        public PaymentStatusOption PaymentStatus { get; set; }

        public DateTime Date { get; set; }

        public Order Order { get; set; }
    }
}
