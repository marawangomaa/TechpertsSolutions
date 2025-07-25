using Core.DTOs.WishListDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("{wishListId}/move-selected-to-cart")]
        public async Task<IActionResult> MoveSelectedToCart(string wishListId, [FromQuery] string customerId, [FromBody] List<string> wishListItemIds)
        {
            if (wishListItemIds == null || !wishListItemIds.Any())
                return BadRequest(new { Success = false, Message = "No wishlist items selected." });
            var result = await _wishListService.MoveSelectedToCartAsync(customerId, wishListItemIds, _cartService);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
