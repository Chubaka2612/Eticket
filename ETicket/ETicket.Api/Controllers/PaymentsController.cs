
using ETicket.Bll.Services;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Api.Controllers
{
    [ApiController]
    [Route("api/eticket/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentInfo([FromRoute] long paymentId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paymentService.GetPaymentStateAsync(paymentId, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<IActionResult> CompleteOrderPayment([FromRoute]  long paymentId, CancellationToken cancellationToken)
        {
            try
            {
                await _paymentService.CompletePaymentAsync(paymentId, cancellationToken);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("{paymentId}/failed")]
        public async Task<IActionResult> FailOrderPayment([FromRoute] long paymentId, CancellationToken cancellationToken)
        {
            try
            {
                await _paymentService.FailPaymentAsync(paymentId, cancellationToken);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
        }
    }
}