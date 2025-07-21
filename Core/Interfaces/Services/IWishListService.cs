using Core.DTOs.WishList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface IWishListService
    {
        Task<GeneralResponse<WishListReadDTO>> CreateAsync(WishListCreateDTO dto);
        Task<GeneralResponse<WishListReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<WishListReadDTO>>> GetByCustomerIdAsync(string customerId);
        Task<GeneralResponse<WishListReadDTO>> AddItemAsync(string wishListId, WishListItemCreateDTO dto);
        Task<GeneralResponse<bool>> RemoveItemAsync(string wishListId, string itemId);
    }
}
