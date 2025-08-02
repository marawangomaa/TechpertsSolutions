using Core.DTOs.CartDTOs;
using Core.DTOs.OrderDTOs;
using System.Threading.Tasks;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId);
        Task<GeneralResponse<CartReadDTO>> GetOrCreateCartAsync(string customerId);
        Task<string> AddItemAsync(string customerId, CartItemDTO itemDto);
        Task<string> UpdateItemQuantityAsync(string customerId, CartUpdateItemQuantityDTO updateDto);
        Task<string> RemoveItemAsync(string customerId, string productId);
        Task<string> ClearCartAsync(string customerId);
        Task<GeneralResponse<OrderReadDTO>> PlaceOrderAsync(string customerId, string? deliveryId = null, string? serviceUsageId = null);
        Task<GeneralResponse<OrderReadDTO>> PartialCheckoutAsync(string customerId, List<string> productIds, string? promoCode = null);
        Task<GeneralResponse<CartReadDTO>> AddPCBuildToCartAsync(string assemblyId, decimal total, decimal assemblyFee);
    }
}
