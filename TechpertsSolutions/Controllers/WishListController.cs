using Core.DTOs.WishListDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;
        private readonly ICartService _cartService;
        public WishListController(IWishListService wishListService, ICartService cartService)
        {
            _wishListService = wishListService;
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WishListCreateDTO dto)
        {
            var result = await _wishListService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _wishListService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(string customerId)
        {
            var result = await _wishListService.GetByCustomerIdAsync(customerId);
            return Ok(result);
        }

        [HttpPost("{wishListId}/items")]
        public async Task<IActionResult> AddItem(string wishListId, [FromBody] WishListItemCreateDTO dto)
        {
            var result = await _wishListService.AddItemAsync(wishListId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{wishListId}/items/{itemId}")]
        public async Task<IActionResult> RemoveItem(string wishListId, string itemId)
        {
            var result = await _wishListService.RemoveItemAsync(wishListId, itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("{wishListId}/move-to-cart")]
        public async Task<IActionResult> MoveAllToCart(string wishListId, [FromQuery] string customerId)
        {
            var result = await _wishListService.MoveAllToCartAsync(customerId, _cartService);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{customerId}/move-selected-to-cart")]
        public async Task<IActionResult> MoveSelectedToCart(string customerId, [FromBody] List<string> wishListItemIds)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(customerId, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            if (wishListItemIds == null || !wishListItemIds.Any())
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Wishlist item IDs cannot be null or empty.",
                    Data = "Invalid input"
                });

            var result = await _wishListService.MoveSelectedToCartAsync(customerId, wishListItemIds, _cartService);
            return Ok(result);
        }

        [HttpPost("{customerId}/move-item-to-cart/{wishListItemId}")]
        public async Task<IActionResult> MoveItemToCart(string customerId, string wishListItemId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(customerId, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            if (string.IsNullOrWhiteSpace(wishListItemId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Wishlist item ID cannot be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(wishListItemId, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Wishlist item ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            var result = await _wishListService.MoveItemToCartAsync(customerId, wishListItemId, _cartService);
            return Ok(result);
        }
    }
}
