

using ETicket.Db.Domain.Enums;

namespace ETicket.Bll.Models
{
    public class BusinessSeat
    {
        public long Id { get; set; }

        public long Number { get; set; }

        public SeatStatusOption SeatStatus { get; set; }
 
        public long RowId { get; set; }
    }
}
