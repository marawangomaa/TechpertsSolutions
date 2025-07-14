using Core.DTOs.Cart;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService cartService;

    public CartController(ICartService _cartService)
    {
        cartService = _cartService;
    }

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetCart(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return BadRequest(new GeneralResponse<string> 
            {
                Success = false,
                Message = "ID must not be null or empty",
                Data = "Invalid Input"
            });
        }
        if (!Guid.TryParse(customerId, out Guid guidId)) 
        {
            return BadRequest(new GeneralResponse<string>
            {
                Success = false,
                Message = "ID format is invalid",
                Data = "Expected GUID"
            });
        }
        try
        {
            var cart = await cartService.GetCartByCustomerIdAsync(customerId);

            return Ok(new GeneralResponse<CartReadDTO>
            {
                Success = true,
                Message = "Cart retrieved successfully.",
                Data = cart
            });
        }
        catch (Exception ex) 
        {
            return NotFound(new GeneralResponse<string>
            {
                Success = false,
                Message = "Cart not found",
                Data = ex.Message
            });
        }
    }

    [HttpPost("{customerId}/items")]
    public async Task<IActionResult> AddItem(string customerId, [FromBody] CartItemDTO itemDto)
    {
        var resultMessage = await cartService.AddItemAsync(customerId, itemDto);

        var isSuccess = resultMessage == "Item added successfully.";

        var response = new GeneralResponse<object>
        {
            Success = isSuccess,
            Message = resultMessage,
            Data = null
        };

        return isSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{customerId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(string customerId, string productId)
    {
        var resultMessage = await cartService.RemoveItemAsync(customerId, productId);

        var isSuccess = resultMessage == "Item removed successfully.";

        var response = new GeneralResponse<object>
        {
            Success = isSuccess,
            Message = resultMessage,
            Data = null
        };

        return isSuccess ? Ok(response) : NotFound(response);
    }
}

