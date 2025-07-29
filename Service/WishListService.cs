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
            var wishLists = await _wishListRepo.FindWithStringIncludesAsync(w => w.CustomerId == customerId, includeProperties: "WishListItems,WishListItems.Product");
            return new GeneralResponse<IEnumerable<WishListReadDTO>>
            {
                Success = true,
                Message = "Wishlists retrieved successfully.",
                Data = wishLists.Select(ToReadDTO)
            };
        }

        public async Task<GeneralResponse<WishListReadDTO>> GetOrCreateWishListAsync(string customerId)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<WishListReadDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<WishListReadDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                
                var existingWishList = await _wishListRepo.GetFirstOrDefaultAsync(
                    w => w.CustomerId == customerId,
                    includeProperties: "WishListItems,WishListItems.Product"
                );

                if (existingWishList != null)
                {
                    return new GeneralResponse<WishListReadDTO>
                    {
                        Success = true,
                        Message = "Wishlist retrieved successfully.",
                        Data = ToReadDTO(existingWishList)
                    };
                }

                
                var newWishList = new WishList
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    WishListItems = new List<WishListItem>()
                };

                await _wishListRepo.AddAsync(newWishList);
                await _wishListRepo.SaveChangesAsync();

                return new GeneralResponse<WishListReadDTO>
                {
                    Success = true,
                    Message = "New wishlist created successfully.",
                    Data = ToReadDTO(newWishList)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<WishListReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving or creating the wishlist.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<WishListReadDTO>> AddItemAsync(string wishListId, WishListItemCreateDTO dto)
        {
            var wishList = await _wishListRepo.GetByIdWithIncludesAsync(wishListId, w => w.WishListItems);
            if (wishList == null)
                return new GeneralResponse<WishListReadDTO> { Success = false, Message = "Wishlist not found.", Data = null };

            
            if (wishList.WishListItems.Any(i => i.ProductId == dto.ProductId))
                return new GeneralResponse<WishListReadDTO> { Success = false, Message = "Product already in wishlist.", Data = ToReadDTO(wishList) };

            wishList.WishListItems?.Add(new WishListItem
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = dto.ProductId,
                
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
            
            var wishlist = (await _wishListRepo.FindWithStringIncludesAsync(w => w.CustomerId == customerId, includeProperties: "WishListItems")).FirstOrDefault();
            if (wishlist == null || wishlist.WishListItems == null || !wishlist.WishListItems.Any())
                return new GeneralResponse<bool> { Success = false, Message = "Wishlist is empty.", Data = false };

            
            var itemsToMove = wishlist.WishListItems.ToList();
            foreach (var wishItem in itemsToMove)
            {
                
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

        public async Task<GeneralResponse<bool>> MoveSelectedToCartAsync(string customerId, List<string> wishListItemIds, ICartService cartService)
        {
            
            var wishlist = (await _wishListRepo.FindWithStringIncludesAsync(w => w.CustomerId == customerId, includeProperties: "WishListItems")).FirstOrDefault();
            if (wishlist == null || wishlist.WishListItems == null || !wishlist.WishListItems.Any())
                return new GeneralResponse<bool> { Success = false, Message = "Wishlist is empty.", Data = false };

            var itemsToMove = wishlist.WishListItems.Where(i => wishListItemIds.Contains(i.Id)).ToList();
            if (!itemsToMove.Any())
                return new GeneralResponse<bool> { Success = false, Message = "No selected items found in wishlist.", Data = false };

            foreach (var wishItem in itemsToMove)
            {
                await cartService.AddItemAsync(customerId, new Core.DTOs.CartDTOs.CartItemDTO
                {
                    ProductId = wishItem.ProductId,
                    Quantity = 1
                });
                wishlist.WishListItems.Remove(wishItem);
            }

            _wishListRepo.Update(wishlist);
            await _wishListRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Selected items moved to cart.", Data = true };
        }

        public async Task<GeneralResponse<bool>> MoveItemToCartAsync(string customerId, string wishListItemId, ICartService cartService)
        {
            
            var wishlist = (await _wishListRepo.FindWithStringIncludesAsync(w => w.CustomerId == customerId, includeProperties: "WishListItems")).FirstOrDefault();
            if (wishlist == null || wishlist.WishListItems == null || !wishlist.WishListItems.Any())
                return new GeneralResponse<bool> { Success = false, Message = "Wishlist is empty.", Data = false };

            var wishItem = wishlist.WishListItems.FirstOrDefault(i => i.Id == wishListItemId);
            if (wishItem == null)
                return new GeneralResponse<bool> { Success = false, Message = "Wishlist item not found.", Data = false };

            
            await cartService.AddItemAsync(customerId, new Core.DTOs.CartDTOs.CartItemDTO
            {
                ProductId = wishItem.ProductId,
                Quantity = 1
            });

            
            wishlist.WishListItems.Remove(wishItem);
            _wishListRepo.Update(wishlist);
            await _wishListRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Item moved to cart successfully.", Data = true };
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
                    WishListId = i.WishListId,
                    ProductName = i.Product?.Name ?? "Unknown Product",
                    ProductPrice = i.Product?.Price ?? 0,
                    ProductImageUrl = i.Product?.ImageUrl
                }).ToList() ?? new List<WishListItemReadDTO>()
            };
        }
    }
}
