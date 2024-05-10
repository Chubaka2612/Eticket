using ETicket.Bll.Models;
using ETicket.Bll.Services;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Api.Controllers
{

    [ApiController]
    [Route("api/eticket/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService cartService)
        {
            _orderService = cartService;
        }

        [HttpGet("carts/{cartId}")]
        public IActionResult GetCartItems([FromRoute] string cartId)
        {
            try
            {
                var response = _orderService.GetCartItems(Guid.Parse(cartId));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("carts/{cartId}")]
        public IActionResult AddToCart([FromRoute] string cartId, [FromBody] BusinessOrderItem orderItem)
        {
            try
            {
                return Ok(_orderService.AddItemToCart(Guid.Parse(cartId), orderItem));
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("carts/{cartId}/events/{eventId}/seats/{seatId}")]
        public IActionResult RemoveFromCart([FromRoute] string cartId, [FromRoute] long eventId, [FromRoute] long seatId)
        {
            try
            {
                _orderService.DeleteItemFromCart(Guid.Parse(cartId), eventId, seatId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("carts/{cartId}/book")]
        public async Task<IActionResult> BookSeats([FromRoute] string cartId, CancellationToken cancellationToken)
        {
            try
            {
                var paymentInfo = await _orderService.BookCartSeatsAsync(Guid.Parse(cartId), cancellationToken);
                return Ok(paymentInfo);
            }
            catch (ArgumentException ex1)
            {
                return NotFound(ex1);
            }

        }
    }
}