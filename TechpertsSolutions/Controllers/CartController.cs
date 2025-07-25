// Controllers.CartController.cs (Assuming this file path)
using Core.DTOs.CartDTOs;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using System;
using System.Threading.Tasks;
using Core.Interfaces.Services;
using System.Linq;

namespace TechpertsSolutions.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;

        public CartController(ICartService _cartService)
        {
            cartService = _cartService;
        }

        /// <summary>
        /// Retrieves the shopping cart for a specific customer.
        /// If no cart exists, a new empty cart is created.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>The customer's cart details.</returns>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCart(string customerId)
        {
            // Input validation for customerId
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
                    // This case should ideally be handled by the service creating a cart,
                    // but as a fallback, if the customer ID itself was invalid within the service.
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Customer with ID {customerId} not found.",
                        Data = null
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
                // Catching generic Exception for unexpected server errors
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the cart.",
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// Adds a product to the customer's cart or updates its quantity if already present.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <param name="itemDto">The details of the item to add/update (ProductId, Quantity).</param>
        /// <returns>A response indicating success or failure.</returns>
        [HttpPost("{customerId}/items")]
        public async Task<IActionResult> AddItem(string customerId, [FromForm] CartItemDTO itemDto)
        {
            // Basic DTO validation
            if (itemDto == null || string.IsNullOrWhiteSpace(itemDto.ProductId) || itemDto.Quantity <= 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid item data provided. ProductId and Quantity (must be > 0) are required.",
                    Data = null
                });
            }

            var resultMessage = await cartService.AddItemAsync(customerId, itemDto);

            // Determine success based on the message from the service
            var isSuccess = resultMessage.StartsWith("✅");

            var response = new GeneralResponse<object>
            {
                Success = isSuccess,
                Message = resultMessage.TrimStart('✅', '❌').Trim(), // Clean up emoji prefixes
                Data = null
            };

            return isSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Updates the quantity of a specific item in the customer's cart.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <param name="updateDto">The DTO containing ProductId and the new Quantity.</param>
        /// <returns>A response indicating success or failure.</returns>
        [HttpPut("{customerId}/items")]
        public async Task<IActionResult> UpdateItemQuantity(string customerId, [FromForm] CartUpdateItemQuantityDTO updateDto)
        {
            // Basic DTO validation
            if (updateDto == null || string.IsNullOrWhiteSpace(updateDto.ProductId) || updateDto.Quantity <= 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid update data provided. ProductId and Quantity (must be > 0) are required.",
                    Data = null
                });
            }

            var resultMessage = await cartService.UpdateItemQuantityAsync(customerId, updateDto);

            var isSuccess = resultMessage.StartsWith("✅");

            var response = new GeneralResponse<object>
            {
                Success = isSuccess,
                Message = resultMessage.TrimStart('✅', '❌').Trim(),
                Data = null
            };

            return isSuccess ? Ok(response) : NotFound(response); // Use NotFound if item/cart not found
        }


        /// <summary>
        /// Removes a specific product from the customer's cart.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <param name="productId">The unique identifier of the product to remove.</param>
        /// <returns>A response indicating success or failure.</returns>
        [HttpDelete("{customerId}/items/{productId}")]
        public async Task<IActionResult> RemoveItem(string customerId, string productId)
        {
            if (string.IsNullOrWhiteSpace(customerId) || string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID and Product ID must not be null or empty.",
                    Data = null
                });
            }

            var resultMessage = await cartService.RemoveItemAsync(customerId, productId);

            var isSuccess = resultMessage.StartsWith("✅");

            var response = new GeneralResponse<object>
            {
                Success = isSuccess,
                Message = resultMessage.TrimStart('✅', '❌').Trim(),
                Data = null
            };

            return isSuccess ? Ok(response) : NotFound(response);
        }

        /// <summary>
        /// Clears all items from the customer's cart.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>A response indicating success or failure.</returns>
        [HttpDelete("{customerId}/clear")]
        public async Task<IActionResult> ClearCart(string customerId)
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

            var resultMessage = await cartService.ClearCartAsync(customerId);

            var isSuccess = resultMessage.StartsWith("✅");

            var response = new GeneralResponse<object>
            {
                Success = isSuccess,
                Message = resultMessage.TrimStart('✅', '❌').Trim(),
                Data = null
            };

            return isSuccess ? Ok(response) : NotFound(response); // Use NotFound if cart not found
        }

        /// <summary>
        /// Converts the customer's current cart into a new order.
        /// Performs stock validation and clears the cart upon successful order creation.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>A response containing the created order details or an error message.</returns>
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

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                // Use appropriate status codes based on the error message from the service
                if (result.Message.Contains("not found") || result.Message.Contains("empty"))
                {
                    return NotFound(result); // 404 for resource not found/empty
                }
                else if (result.Message.Contains("stock") || result.Message.Contains("validation"))
                {
                    return Conflict(result); // 409 Conflict for stock issues
                }
                else
                {
                    return BadRequest(result); // 400 for other validation/business logic errors
                }
            }
        }

        /// <summary>
        /// Converts the customer's current cart into a new order with additional parameters.
        /// Performs comprehensive validation and clears the cart upon successful order creation.
        /// </summary>
        /// <param name="checkoutDto">The checkout details including customer ID and optional parameters.</param>
        /// <returns>A response containing the created order details or an error message.</returns>
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutWithDetails([FromBody] CartCheckoutDTO checkoutDto)
        {
            if (checkoutDto == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Checkout details cannot be null.",
                    Data = null
                });
            }

            // Validate the model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Validation failed: {string.Join(", ", errors)}",
                    Data = null
                });
            }

            // Additional validation
            if (string.IsNullOrWhiteSpace(checkoutDto.CustomerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID is required.",
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

            // Validate optional GUIDs if provided
            if (!string.IsNullOrWhiteSpace(checkoutDto.DeliveryId) && !Guid.TryParse(checkoutDto.DeliveryId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Delivery ID format. Expected GUID format.",
                    Data = null
                });
            }

            if (!string.IsNullOrWhiteSpace(checkoutDto.SalesManagerId) && !Guid.TryParse(checkoutDto.SalesManagerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Sales Manager ID format. Expected GUID format.",
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
                checkoutDto.SalesManagerId,
                checkoutDto.ServiceUsageId
            );

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                // Use appropriate status codes based on the error message from the service
                if (result.Message.Contains("not found") || result.Message.Contains("empty"))
                {
                    return NotFound(result); // 404 for resource not found/empty
                }
                else if (result.Message.Contains("stock") || result.Message.Contains("validation"))
                {
                    return Conflict(result); // 409 Conflict for stock issues
                }
                else
                {
                    return BadRequest(result); // 400 for other validation/business logic errors
                }
            }
        }

        [HttpPost("{customerId}/partial-checkout")]
        public async Task<IActionResult> PartialCheckout(string customerId, [FromBody] PartialCheckoutDTO dto)
        {
            if (dto == null || dto.ProductIds == null || !dto.ProductIds.Any())
                return BadRequest(new { Success = false, Message = "No products selected for partial checkout." });
            var result = await cartService.PartialCheckoutAsync(customerId, dto.ProductIds, dto.PromoCode);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}