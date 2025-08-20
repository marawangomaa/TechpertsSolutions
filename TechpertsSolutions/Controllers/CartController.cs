using Core.DTOs;
using Core.DTOs.CartDTOs;
using Core.DTOs.OrderDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechpertsSolutions.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;
        private readonly IPaymentService _paymentService;
        public CartController(ICartService _cartService, IPaymentService paymentService)
        {
            cartService = _cartService;
            _paymentService = paymentService;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCart(string customerId)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = "Invalid Input"
                });
            }
            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID format is invalid.",
                    Data = "Expected GUID"
                });
            }

            try
            {
                var cart = await cartService.GetCartByCustomerIdAsync(customerId);

                if (cart == null)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Customer with ID {customerId} not found.",
                        Data = "Customer not found in database"
                    });
                }

                return Ok(new GeneralResponse<CartReadDTO>
                {
                    Success = true,
                    Message = "Cart retrieved successfully.",
                    Data = cart
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the cart.",
                    Data = ex.Message
                });
            }
        }
        
        [HttpPost("{customerId}/items")]
        public async Task<IActionResult> AddItem(string customerId, [FromBody] CartItemDTO itemDto)
        {
            // Validate customerId
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = "Missing CustomerId"
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID format is invalid.",
                    Data = "Expected GUID format"
                });
            }

            // Validate itemDto
            if (itemDto == null || string.IsNullOrWhiteSpace(itemDto.ProductId) || itemDto.Quantity <= 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid item data provided. ProductId and Quantity (must be > 0) are required.",
                    Data = "Missing or invalid ProductId or Quantity"
                });
            }

            // Validate ProductId format
            if (!Guid.TryParse(itemDto.ProductId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Product ID format is invalid.",
                    Data = "Expected GUID format"
                });
            }

            try
            {
                var resultMessage = await cartService.AddItemAsync(customerId, itemDto);

                // The service returns messages starting with "?" for success
                var isSuccess = resultMessage.StartsWith("?");

                var response = new GeneralResponse<object>
                {
                    Success = isSuccess,
                    Message = resultMessage.TrimStart('?').Trim(),
                    Data = null
                };

                return isSuccess ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred while adding item to cart.",
                    Data = ex.Message
                });
            }
        }

        [HttpPut("{customerId}/items")]
        public async Task<IActionResult> UpdateItemQuantity(string customerId, [FromBody] CartUpdateItemQuantityDTO updateDto)
        {
            // Validate customerId
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = "Missing CustomerId"
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID format is invalid.",
                    Data = "Expected GUID format"
                });
            }

            // Validate updateDto
            if (updateDto == null || string.IsNullOrWhiteSpace(updateDto.ProductId) || updateDto.Quantity <= 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid update data provided. ProductId and Quantity (must be > 0) are required.",
                    Data = "Missing or invalid ProductId or Quantity"
                });
            }

            // Validate ProductId format
            if (!Guid.TryParse(updateDto.ProductId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Product ID format is invalid.",
                    Data = "Expected GUID format"
                });
            }

            try
            {
                var resultMessage = await cartService.UpdateItemQuantityAsync(customerId, updateDto);

                // The service returns messages starting with "?" for success
                var isSuccess = resultMessage.StartsWith("?");

                var response = new GeneralResponse<object>
                {
                    Success = isSuccess,
                    Message = resultMessage.TrimStart('?').Trim(),
                    Data = null
                };

                return isSuccess ? Ok(response) : NotFound(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating item quantity.",
                    Data = ex.Message
                });
            }
        }
        [HttpPut("increase")]
        public async Task<IActionResult> IncreaseQuantity([FromQuery] string customerId, [FromQuery] string productId)
        {
            var result = await cartService.IncreaseQuantity(customerId, productId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("decrease")]
        public async Task<IActionResult> DecreaseQuantity([FromQuery] string customerId, [FromQuery] string productId)
        {
            var result = await cartService.DecreaseQuantity(customerId, productId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{customerId}/items/{productId}")]
        public async Task<IActionResult> RemoveItem(string customerId, string productId)
        {
            // Validate customerId
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = "Missing CustomerId"
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID format is invalid.",
                    Data = "Expected GUID format"
                });
            }

            // Validate productId
            if (string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Product ID must not be null or empty.",
                    Data = "Missing ProductId"
                });
            }

            if (!Guid.TryParse(productId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Product ID format is invalid.",
                    Data = "Expected GUID format"
                });
            }

            try
            {
                var resultMessage = await cartService.RemoveItemAsync(customerId, productId);

                // The service returns messages starting with "?" for success
                var isSuccess = resultMessage.StartsWith("?");

                var response = new GeneralResponse<object>
                {
                    Success = isSuccess,
                    Message = resultMessage.TrimStart('?').Trim(),
                    Data = null
                };

                return isSuccess ? Ok(response) : NotFound(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred while removing item from cart.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{customerId}/clear")]
        public async Task<IActionResult> ClearCart(string customerId)
        {
            // Validate customerId
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = "Missing CustomerId"
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID format is invalid.",
                    Data = "Expected GUID format"
                });
            }

            try
            {
                var resultMessage = await cartService.ClearCartAsync(customerId);

                // The service returns messages starting with "?" for success
                var isSuccess = resultMessage.StartsWith("?");

                var response = new GeneralResponse<object>
                {
                    Success = isSuccess,
                    Message = resultMessage.TrimStart('?').Trim(),
                    Data = null
                };

                return isSuccess ? Ok(response) : NotFound(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred while clearing cart.",
                    Data = ex.Message
                });
            }
        }

        
        
        
        [HttpPost("{customerId}/checkout")]
        public async Task<IActionResult> Checkout(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = null
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                });
            }

            var result = await cartService.PlaceOrderAsync(customerId);

            return result.Success ? Ok(result) : GetErrorResponse(result);
        }
        [HttpPost("{customerId}/checkout-payment")]
        public async Task<IActionResult> Checkout(string customerId, [FromBody] ConfirmPaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = null
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                });
            }

            if (string.IsNullOrWhiteSpace(request.PaymentIntentId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "PaymentIntentId is required.",
                    Data = null
                });
            }

            // ✅ Step 1: Verify payment with Stripe
            var paymentSucceeded = await _paymentService.VerifyPaymentAsync(request.PaymentIntentId);
            if (!paymentSucceeded)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Payment verification failed. Order not placed.",
                    Data = null
                });
            }

            // ✅ Step 2: Place the order if payment succeeded
            var result = await cartService.PlaceOrderAsync(customerId);

            return result.Success ? Ok(result) : GetErrorResponse(result);
        }



        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutWithDetails([FromBody] CartCheckoutDTO checkoutDto)
        {
            if (checkoutDto == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Checkout data is required.",
                    Data = null
                });
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(checkoutDto.CustomerId))
                errors.Add("Customer ID is required.");

            if (errors.Any())
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Validation failed: {string.Join(", ", errors)}",
                    Data = null
                });
            }

            if (!Guid.TryParse(checkoutDto.CustomerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                });
            }

            if (!string.IsNullOrWhiteSpace(checkoutDto.DeliveryId) && !Guid.TryParse(checkoutDto.DeliveryId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Delivery ID format. Expected GUID format.",
                    Data = null
                });
            }

            if (!string.IsNullOrWhiteSpace(checkoutDto.ServiceUsageId) && !Guid.TryParse(checkoutDto.ServiceUsageId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Service Usage ID format. Expected GUID format.",
                    Data = null
                });
            }

            var result = await cartService.PlaceOrderAsync(
                checkoutDto.CustomerId,
                checkoutDto.DeliveryId,
                checkoutDto.ServiceUsageId
            );

            return result.Success ? Ok(result) : GetErrorResponse(result);
        }

        [HttpPost("{customerId}/initiate-payment")]
        public async Task<IActionResult> InitiatePayment(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID must not be null or empty.",
                    Data = null
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                });
            }

            try
            {
                var result = await cartService.InitiatePaymentAsync(customerId);

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while initiating the payment.",
                    Data = ex.Message
                });
            }
        }




        private IActionResult GetErrorResponse(GeneralResponse<OrderReadDTO> result)
        {
            if (result.Message?.Contains("not found") == true || result.Message?.Contains("empty") == true)
            {
                return NotFound(result);
            }
            else if (result.Message?.Contains("stock") == true || result.Message?.Contains("validation") == true)
            {
                return Conflict(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
