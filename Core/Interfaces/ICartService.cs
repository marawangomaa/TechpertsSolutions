using Core.DTOs.Cart;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICartService
    {
        Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId);
        Task<string> AddItemAsync(string customerId, CartItemDTO itemDto);
        Task<string> RemoveItemAsync(string customerId, string productId);
    }
}

