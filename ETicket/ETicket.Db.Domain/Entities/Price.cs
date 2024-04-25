using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Price : Entity
    {
        public string Name { get; set; }

        public decimal Amount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
