using ETicket.Messaging.Enums;
using System;
using System.Collections.Generic;

namespace ETicket.Messaging.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Operation Option { get; set; }

        public DateTime Timestamp { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        public List<Ticket> Content { get; set; }
    }
}
