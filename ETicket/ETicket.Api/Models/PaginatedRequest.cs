using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Api.Models
{
    public class PaginatedRequest
    {
        [FromQuery(Name = "skip")]
        public int Skip { get; set; }

        [Range(0, 100)]
        [FromQuery(Name = "limit")]
        public int Limit { get; set; } = 10;
    }
}