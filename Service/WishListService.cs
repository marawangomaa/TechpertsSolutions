using Core.DTOs.WishListDTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class WishListService : IWishListService
    {
        private readonly IRepository<WishList> _wishListRepo;

        public WishListService(IRepository<WishList> wishListRepo)
        {
            _wishListRepo = wishListRepo;
        }

        public async Task<GeneralResponse<WishListReadDTO>> CreateAsync(WishListCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CustomerId))
                return new GeneralResponse<WishListReadDTO> { Success = false, Message = "Invalid data.", Data = null };

            // Check if a wishlist already exists for this customer
            var existing = (await _wishListRepo.FindAsync(w => w.CustomerId == dto.CustomerId)).FirstOrDefault();
            if (existing != null)
            {
                return new GeneralResponse<WishListReadDTO>
                {
                    Success = true,
                    Message = "Wishlist already exists.",
                    Data = ToReadDTO(existing)
                };
            }

            var wishList = new WishList
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = dto.CustomerId,
                WishListItems = dto.Items?.Select(i => new WishListItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = i.ProductId,
                    //CartId = i.CartId
                }).ToList() ?? new List<WishListItem>()
            };

            await _wishListRepo.AddAsync(wishList);
            await _wishListRepo.SaveChangesAsync();

            return new GeneralResponse<WishListReadDTO>
            {
                Success = true,
                Message = "Wishlist created successfully.",
                Data = ToReadDTO(wishList)
            };
        }

        public async Task<GeneralResponse<WishListReadDTO>> GetByIdAsync(string id)
        {
            var wishList = await _wishListRepo.GetByIdWithIncludesAsync(id, w => w.WishListItems);
            if (wishList == null)
                return new GeneralResponse<WishListReadDTO> { Success = false, Message = "Wishlist not found.", Data = null };

            return new GeneralResponse<WishListReadDTO>
            {
                Success = true,
                Message = "Wishlist retrieved successfully.",
                Data = ToReadDTO(wishList)
            };
        }

        public async Task<GeneralResponse<IEnumerable<WishListReadDTO>>> GetByCustomerIdAsync(string customerId)
        {
            var wishLists = await _wishListRepo.FindWithIncludesAsync(w => w.CustomerId == customerId, w => w.WishListItems);
            return new GeneralResponse<IEnumerable<WishListReadDTO>>
            {
                Success = true,
                Message = "Wishlists retrieved successfully.",
                Data = wishLists.Select(ToReadDTO)
            };
        }

        public async Task<GeneralResponse<WishListReadDTO>> AddItemAsync(string wishListId, WishListItemCreateDTO dto)
        {
            var wishList = await _wishListRepo.GetByIdWithIncludesAsync(wishListId, w => w.WishListItems);
            if (wishList == null)
                return new GeneralResponse<WishListReadDTO> { Success = false, Message = "Wishlist not found.", Data = null };

            // Prevent duplicate products
            if (wishList.WishListItems.Any(i => i.ProductId == dto.ProductId))
                return new GeneralResponse<WishListReadDTO> { Success = false, Message = "Product already in wishlist.", Data = ToReadDTO(wishList) };

            wishList.WishListItems?.Add(new WishListItem
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = dto.ProductId,
                //CartId = dto.CartId
            });

            _wishListRepo.Update(wishList);
            await _wishListRepo.SaveChangesAsync();

            return new GeneralResponse<WishListReadDTO>
            {
                Success = true,
                Message = "Item added successfully.",
                Data = ToReadDTO(wishList)
            };
        }

        public async Task<GeneralResponse<bool>> RemoveItemAsync(string wishListId, string itemId)
        {
            var wishList = await _wishListRepo.GetByIdWithIncludesAsync(wishListId, w => w.WishListItems);
            if (wishList == null)
                return new GeneralResponse<bool> { Success = false, Message = "Wishlist not found.", Data = false };

            var item = wishList.WishListItems?.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return new GeneralResponse<bool> { Success = false, Message = "Item not found.", Data = false };

            wishList.WishListItems.Remove(item);
            _wishListRepo.Update(wishList);
            await _wishListRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Item removed successfully.", Data = true };
        }

        public async Task<GeneralResponse<bool>> MoveAllToCartAsync(string customerId, ICartService cartService)
        {
            // Get wishlist
            var wishlist = (await _wishListRepo.FindWithIncludesAsync(w => w.CustomerId == customerId, w => w.WishListItems)).FirstOrDefault();
            if (wishlist == null || wishlist.WishListItems == null || !wishlist.WishListItems.Any())
                return new GeneralResponse<bool> { Success = false, Message = "Wishlist is empty.", Data = false };

            // Move each item
            var itemsToMove = wishlist.WishListItems.ToList();
            foreach (var wishItem in itemsToMove)
            {
                // Use cart service to add item (prevents duplicates)
                await cartService.AddItemAsync(customerId, new Core.DTOs.CartDTOs.CartItemDTO
                {
                    ProductId = wishItem.ProductId,
                    Quantity = 1
                });
                wishlist.WishListItems.Remove(wishItem);
            }

            _wishListRepo.Update(wishlist);
            await _wishListRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "All items moved to cart.", Data = true };
        }

        private WishListReadDTO ToReadDTO(WishList entity)
        {
            return new WishListReadDTO
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                CreatedAt = entity.CreatedAt,
                Items = entity.WishListItems?.Select(i => new WishListItemReadDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    CartId = i.CartId
                }).ToList() ?? new List<WishListItemReadDTO>()
            };
        }
    }
}
