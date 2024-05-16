using ETicket.Db.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Payment : Entity
    {
        public PaymentStatusOption PaymentStatusId { get; set; }

        public DateTime Date { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = null!;
    }
}
