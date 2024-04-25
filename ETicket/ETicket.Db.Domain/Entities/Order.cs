using System;
using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Order : Entity
    {
        public long UserId { get; set; }

        public long PaymentId { get; set; }

        public long EventId { get; set; }

        public DateTime Date { get; set; }

        public User User { get; set; }

        public Event Event { get; set; }

        public Payment Payment { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
