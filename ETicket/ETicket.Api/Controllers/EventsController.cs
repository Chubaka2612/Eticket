using ETicket.Api.Models;
using ETicket.Bll.Services;
using ETicket.Db.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace ETicket.Api.Controllers
{
    [ApiController]
    [Route("api/eticket/events")]

    public class EventsController: ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [OutputCache(Tags = ["Events"])]
        public async Task<IActionResult> GetEvents([FromQuery] PaginatedRequest request, CancellationToken cancellationToken)
        {
            var paginatedResult = await _eventService.GetEventsAsync(request.Skip, request.Limit, cancellationToken);
            var venuesResponse = new PaginatedResponse<Event>(paginatedResult.Skip, paginatedResult.Limit, paginatedResult.Count, paginatedResult.Items);
            
            return Ok(venuesResponse);
        }


        [HttpGet("{eventId}/venues/{venueId}/sections/{sectionId}/seats")]
        [OutputCache(Tags = ["Events"] )]
        public async Task<IActionResult> GetSections([FromRoute] long eventId, [FromRoute] long venueId, [FromRoute] long sectionId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _eventService.GetEventSeatsBySectionAsync(eventId: eventId, sectionId: sectionId, venueId: venueId, cancellationToken);
                return Ok(response);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
