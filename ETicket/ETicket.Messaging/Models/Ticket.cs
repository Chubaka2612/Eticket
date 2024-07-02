using System;

namespace ETicket.Messaging.Models
{
    public class Ticket
    {
        public string EventTitle { get; set; }

        public DateTime EventDateAndTime { get; set; }

        public string Venue { get; set; }

        public int SeatNumber { get; set; }

        public int RowNumber { get; set; }

        public decimal TicketPrice { get; set; }
    }
}
