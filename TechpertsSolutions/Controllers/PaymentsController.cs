using Core.DTOs;
using Core.DTOs.PaymentsDTOs;
using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-intent")]
        public async Task<ActionResult<GeneralResponse<object>>> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            if (request.Amount <= 0 || string.IsNullOrEmpty(request.Currency))
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid payment request",
                    Data = null
                });
            }

            try
            {
                var clientSecret = await _paymentService.CreatePaymentIntentAsync(request.Amount, request.Currency);

                return Ok(new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Payment intent created successfully",
                    Data = new { clientSecret }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Error creating payment intent: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("publishable-key")]
        public ActionResult<GeneralResponse<object>> GetPublishableKey([FromServices] IOptions<StripeSettings> options)
        {
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Publishable key retrieved successfully",
                Data = new { key = options.Value.PublishableKey }
            });
        }
    }
}