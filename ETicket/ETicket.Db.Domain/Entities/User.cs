using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class User : Entity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
