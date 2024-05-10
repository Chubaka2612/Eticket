using ETicket.Api.Models;
using ETicket.Bll.Services;
using ETicket.Db.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Api.Controllers
{
    [ApiController]
    [Route("api/eticket/venues")]
    public class VenuesController: ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVenues([FromQuery] PaginatedRequest request, CancellationToken cancellationToken)
        {
            var paginatedResult = await _venueService.GetVenuesAsync(request.Skip, request.Limit, cancellationToken);
            var venuesResponse = new PaginatedResponse<Venue>(paginatedResult.Skip, paginatedResult.Limit, paginatedResult.Count, paginatedResult.Items);
            
            return Ok(venuesResponse);
        }

        //TODO: endless query on Manifest but service layer is working fine
        [HttpGet("{venueId}/sections")]
        public async Task<IActionResult> GetSections([FromRoute] long venueId, CancellationToken cancellationToken)
        {
            var response = await _venueService.GetSectionsAsync(venueId, cancellationToken);
            
            return Ok(response);
        }
    }
}
