// Core.Interfaces.ICartService.cs
using Core.DTOs.Cart;
using Core.DTOs.Order;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs; // Ensure this using directive is present

namespace Core.Interfaces
{
    public interface ICartService
    {
        Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId);
        Task<string> AddItemAsync(string customerId, CartItemDTO itemDto);
        Task<string> UpdateItemQuantityAsync(string customerId, CartUpdateItemQuantityDTO updateDto);
        Task<string> RemoveItemAsync(string customerId, string productId);
        Task<string> ClearCartAsync(string customerId);
        Task<GeneralResponse<OrderReadDTO>> PlaceOrderAsync(string customerId);
    }
}
