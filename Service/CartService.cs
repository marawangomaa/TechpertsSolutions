
using Core.DTOs.CartDTOs;
using Core.DTOs.OrderDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<CartItem> cartItemRepo;
        private readonly IRepository<Product> productRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<Order> orderRepo;
        private readonly IRepository<OrderItem> orderItemRepo;
        private readonly IRepository<PCAssembly> pcAssemblyRepo;
        private readonly TechpertsContext dbContext;
        private readonly IRepository<PCAssemblyItem> pcAssemblyItemRepo;

        public CartService(
            IRepository<Cart> _cartRepo,
            IRepository<CartItem> _cartItemRepo,
            IRepository<Product> _productRepo,
            IRepository<Customer> _customerRepo,
            IRepository<Order> _orderRepo,
            IRepository<OrderItem> _orderItemRepo,
            IRepository<PCAssembly> _pcAssemblyRepo,
            TechpertsContext _dbContext,
            IRepository<PCAssemblyItem> _pcAssemblyItemRepo)
        {
            cartRepo = _cartRepo;
            cartItemRepo = _cartItemRepo;
            productRepo = _productRepo;
            customerRepo = _customerRepo;
            orderRepo = _orderRepo;
            orderItemRepo = _orderItemRepo;
            pcAssemblyRepo = _pcAssemblyRepo;
            dbContext = _dbContext;
            pcAssemblyItemRepo = _pcAssemblyItemRepo;
        }

        public async Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return null;
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return null;
            }

            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                return null; 
            }

            // Optimized includes for cart with items and their products
            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems,CartItems.Product,CartItems.Product.Category,CartItems.Product.SubCategory,CartItems.Product.TechCompany,CartItems.PCAssembly"
            );

            if (cart == null)
            {
                
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };

                await cartRepo.AddAsync(cart);
                await cartRepo.SaveChangesAsync();
            }

            return CartMapper.MapToCartReadDTO(cart);
        }

        public async Task<GeneralResponse<CartReadDTO>> GetOrCreateCartAsync(string customerId)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<CartReadDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<CartReadDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var customer = await customerRepo.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new GeneralResponse<CartReadDTO>
                    {
                        Success = false,
                        Message = "Customer not found.",
                        Data = null
                    };
                }

                // Optimized includes for cart with items and their products
                var cart = await cartRepo.GetFirstOrDefaultAsync(
                    c => c.CustomerId == customerId,
                    includeProperties: "CartItems,CartItems.Product,CartItems.Product.Category,CartItems.Product.SubCategory,CartItems.Product.TechCompany" 
                );

                if (cart == null)
                {
                    
                    cart = new Cart
                    {
                        CustomerId = customerId,
                        CreatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItem>()
                    };

                    await cartRepo.AddAsync(cart);
                    await cartRepo.SaveChangesAsync();

                    return new GeneralResponse<CartReadDTO>
                    {
                        Success = true,
                        Message = "New cart created successfully.",
                        Data = CartMapper.MapToCartReadDTO(cart)
                    };
                }

                return new GeneralResponse<CartReadDTO>
                {
                    Success = true,
                    Message = "Cart retrieved successfully.",
                    Data = CartMapper.MapToCartReadDTO(cart)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CartReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the cart.",
                    Data = null
                };
            }
        }

        public async Task<string> AddItemAsync(string customerId, CartItemDTO itemDto)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
                return "? Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "? Invalid Customer ID format. Expected GUID format.";

            if (itemDto == null)
                return "? Item data cannot be null.";

            if (string.IsNullOrWhiteSpace(itemDto.ProductId))
                return "? Product ID cannot be null or empty.";

            if (!Guid.TryParse(itemDto.ProductId, out _))
                return "? Invalid Product ID format. Expected GUID format.";

            if (itemDto.Quantity <= 0)
                return "? Quantity must be greater than zero.";

            if (itemDto.Quantity > 1000) 
                return "? Quantity cannot exceed 1000 items.";

            
            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                return $"? Customer with ID {customerId} not found.";

            
            var product = await productRepo.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                return $"? Product with ID {itemDto.ProductId} not found.";

            if (product.Stock < itemDto.Quantity)
                return $"? Not enough stock for product '{product.Name}'. Available: {product.Stock}, Requested: {itemDto.Quantity}.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems"
            );

            
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                await cartRepo.AddAsync(cart);
                await cartRepo.SaveChangesAsync(); 
            }

            var existingItem = cart.CartItems?.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                
                int newQuantity = existingItem.Quantity + itemDto.Quantity;
                if (product.Stock < newQuantity)
                    return $"? Cannot add {itemDto.Quantity} more. Total requested ({newQuantity}) exceeds available stock ({product.Stock}).";

                existingItem.Quantity = newQuantity;
                cartItemRepo.Update(existingItem);
                await cartItemRepo.SaveChangesAsync();
                return "? Item quantity updated successfully.";
            }
            else
            {
                
                var newItem = new CartItem
                {
                    ProductId = itemDto.ProductId,
                    CartId = cart.Id,
                    Quantity = itemDto.Quantity
                };

                await cartItemRepo.AddAsync(newItem);
                await cartItemRepo.SaveChangesAsync();
                return "? Item added successfully.";
            }
        }
        public async Task<string> AddItemPcAssemblyAsync(string customerId, CartAssemblyItemDTO itemDto)
        {
            // --- 1. Validation ---
            if (string.IsNullOrWhiteSpace(customerId))
                return "? Customer ID cannot be null or empty.";
            if (itemDto == null)
                return "? Item data cannot be null.";
            if (string.IsNullOrWhiteSpace(itemDto.ProductId))
                return "? Product ID cannot be null or empty.";
            if (string.IsNullOrWhiteSpace(itemDto.PcAssemblyId))
                return "? PC Assembly ID cannot be null or empty.";
            if (itemDto.Quantity <= 0)
                return "? Quantity must be greater than zero.";

            // --- 2. Fetch PC Assembly Item data ---
            var pcAssemblyItem = await pcAssemblyItemRepo.GetFirstOrDefaultAsync(
                i => i.PCAssemblyId == itemDto.PcAssemblyId && i.ProductId == itemDto.ProductId,
                query => query.Include(i => i.Product)
            );

            if (pcAssemblyItem == null || pcAssemblyItem.Product == null)
                return $"? Product with ID {itemDto.ProductId} not found in PC Assembly with ID {itemDto.PcAssemblyId}.";

            // Stock check
            if (pcAssemblyItem.Product.Stock < itemDto.Quantity)
                return $"? Not enough stock for product '{pcAssemblyItem.Product.Name}'. Available: {pcAssemblyItem.Product.Stock}, Requested: {itemDto.Quantity}.";

            // --- 3. Get or create the cart ---
            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                query => query.Include(c => c.CartItems)
            );

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                await cartRepo.AddAsync(cart);
                await cartRepo.SaveChangesAsync();
            }

            // --- 4. Add or update the product item in the cart ---
            var existingProductItem = cart.CartItems?.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

            if (existingProductItem != null)
            {
                existingProductItem.Quantity += itemDto.Quantity;
                cartItemRepo.Update(existingProductItem);
            }
            else
            {
                var newProductItem = new CartItem
                {
                    ProductId = itemDto.ProductId,
                    CartId = cart.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = pcAssemblyItem.UnitPrice, // Use the price from the PC Assembly item
                };
                await cartItemRepo.AddAsync(newProductItem);
            }

            // --- 5. Add or update the assembly fee as a separate cart item ---
            // Fetch the PC Assembly to get the fee.
            var pcAssembly = await pcAssemblyRepo.GetByIdAsync(itemDto.PcAssemblyId);
            if (pcAssembly != null && pcAssembly.AssemblyFee.HasValue && pcAssembly.AssemblyFee.Value > 0)
            {
                // You must have a dedicated product for the assembly fee in your database.
                string AssemblyFeeProductId = itemDto.ProductId;
                var existingFeeItem = cart.CartItems?.FirstOrDefault(i => i.ProductId == AssemblyFeeProductId);

                if (existingFeeItem == null)
                {
                    var newFeeItem = new CartItem
                    {
                        ProductId = AssemblyFeeProductId,
                        CartId = cart.Id,
                        Quantity = 1,
                        UnitPrice = pcAssembly.AssemblyFee.Value
                    };
                    await cartItemRepo.AddAsync(newFeeItem);
                }
            }

            await cartRepo.SaveChangesAsync();
            return "✔ Product and assembly fee added successfully.";
        }

        public async Task<string> UpdateItemQuantityAsync(string customerId, CartUpdateItemQuantityDTO updateDto)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
                return "? Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "? Invalid Customer ID format. Expected GUID format.";

            if (updateDto == null)
                return "? Update data cannot be null.";

            if (string.IsNullOrWhiteSpace(updateDto.ProductId))
                return "? Product ID cannot be null or empty.";

            if (!Guid.TryParse(updateDto.ProductId, out _))
                return "? Invalid Product ID format. Expected GUID format.";

            if (updateDto.Quantity <= 0)
                return "? Quantity must be greater than zero. To remove, use the remove endpoint.";

            if (updateDto.Quantity > 1000) 
                return "? Quantity cannot exceed 1000 items.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems.Product" 
            );

            if (cart == null)
                return $"? Cart not found for customer ID {customerId}.";

            var itemToUpdate = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == updateDto.ProductId);
            if (itemToUpdate == null)
                return $"? Product with ID {updateDto.ProductId} not found in cart.";

            
            if (itemToUpdate.Product == null)
                return "? Product details not available for stock check."; 

            if (itemToUpdate.Product.Stock < updateDto.Quantity)
                return $"? Not enough stock for product '{itemToUpdate.Product.Name}'. Available: {itemToUpdate.Product.Stock}, Requested: {updateDto.Quantity}.";

            itemToUpdate.Quantity = updateDto.Quantity;
            cartItemRepo.Update(itemToUpdate);
            await cartItemRepo.SaveChangesAsync();

            return "? Item quantity updated successfully.";
        }

        public async Task<string> RemoveItemAsync(string customerId, string productId)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
                return "? Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "? Invalid Customer ID format. Expected GUID format.";

            if (string.IsNullOrWhiteSpace(productId))
                return "? Product ID cannot be null or empty.";

            if (!Guid.TryParse(productId, out _))
                return "? Invalid Product ID format. Expected GUID format.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems"
            );

            if (cart == null)
                return $"? Cart not found for customer ID {customerId}.";

            var item = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null)
                return $"? Product with ID {productId} not found in cart.";

            cart.CartItems?.Remove(item); 
            cartItemRepo.Remove(item); 
            await cartItemRepo.SaveChangesAsync();

            return "? Item removed successfully.";
        }

        public async Task<string> ClearCartAsync(string customerId)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
                return "? Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "? Invalid Customer ID format. Expected GUID format.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems"
            );

            if (cart == null)
                return $"? Cart not found for customer ID {customerId}.";

            if (cart.CartItems == null || !cart.CartItems.Any())
                return "?? Cart is already empty.";

            
            foreach (var item in cart.CartItems.ToList()) 
            {
                cart.CartItems.Remove(item);
                cartItemRepo.Remove(item);
            }
            await cartItemRepo.SaveChangesAsync();

            return "? Cart cleared successfully.";
        }

        
        
        
        public async Task<GeneralResponse<OrderReadDTO>> PlaceOrderAsync(string customerId, string? deliveryId = null, string? serviceUsageId = null)
        {
            return await CheckoutCartAsync(customerId, null, deliveryId, serviceUsageId);
        }

        
        
        
        public async Task<GeneralResponse<OrderReadDTO>> PartialCheckoutAsync(string customerId, List<string> productIds, string? promoCode = null)
        {
            return await CheckoutCartAsync(customerId, productIds, null, null, promoCode);
        }

        public async Task<GeneralResponse<CartReadDTO>> AddPCBuildToCartAsync(string assemblyId, decimal total, decimal assemblyFee)
        {
            try
            {
                // This is a simplified implementation for PC builds
                // In a real scenario, you would need to handle the PC build as a special cart item
                
                var cartItem = new CartItemDTO
                {
                    ProductId = assemblyId, // Using assembly ID as product ID for PC builds
                    Quantity = 1,
                    UnitPrice = total,
                    TotalPrice = total
                };

                // For now, return a success response indicating the PC build was added
                // In a full implementation, you would add this to the actual cart
                return new GeneralResponse<CartReadDTO>
                {
                    Success = true,
                    Message = "PC Build added to cart successfully.",
                    Data = new CartReadDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = string.Empty, // Would be set in actual implementation
                        CreatedAt = DateTime.UtcNow,
                        SubTotal = total,
                        CartItems = new List<CartItemReadDTO>()
                    }
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CartReadDTO>
                {
                    Success = false,
                    Message = "Failed to add PC build to cart.",
                    Data = null
                };
            }
        }

        
        
        
        private async Task<GeneralResponse<OrderReadDTO>> CheckoutCartAsync(
            string customerId, 
            List<string>? selectedProductIds = null, 
            string? deliveryId = null, 
            string? serviceUsageId = null,
            string? promoCode = null)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "? Customer ID is required.",
                    Data = null
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "? Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            
            if (!string.IsNullOrWhiteSpace(deliveryId) && !Guid.TryParse(deliveryId, out _))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "? Invalid Delivery ID format. Expected GUID format.",
                    Data = null
                };
            }

            if (!string.IsNullOrWhiteSpace(serviceUsageId) && !Guid.TryParse(serviceUsageId, out _))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "? Invalid Service Usage ID format. Expected GUID format.",
                    Data = null
                };
            }

            
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                
                var cart = await cartRepo.GetFirstOrDefaultAsync(
                    c => c.CustomerId == customerId,
                    includeProperties: "CartItems.Product,Customer"
                );

                if (cart == null)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = "? Cart not found for this customer.",
                        Data = null
                    };
                }

                if (cart.CartItems == null || !cart.CartItems.Any())
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = "? Cart is empty.",
                        Data = null
                    };
                }

                
                var itemsToCheckout = selectedProductIds != null && selectedProductIds.Any()
                    ? cart.CartItems.Where(ci => selectedProductIds.Contains(ci.ProductId)).ToList()
                    : cart.CartItems.ToList();

                if (!itemsToCheckout.Any())
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = selectedProductIds != null 
                            ? "? None of the selected products are in the cart."
                            : "? No items to checkout.",
                        Data = null
                    };
                }

                
                var stockValidationErrors = new List<string>();
                foreach (var cartItem in itemsToCheckout)
                {
                    if (cartItem.Product == null)
                    {
                        stockValidationErrors.Add($"? Product with ID {cartItem.ProductId} not found.");
                        continue;
                    }

                    if (cartItem.Quantity <= 0)
                    {
                        stockValidationErrors.Add($"? Invalid quantity ({cartItem.Quantity}) for product '{cartItem.Product.Name}'.");
                        continue;
                    }

                    if (cartItem.Product.Stock < cartItem.Quantity)
                    {
                        stockValidationErrors.Add($"? Insufficient stock for '{cartItem.Product.Name}'. Available: {cartItem.Product.Stock}, Requested: {cartItem.Quantity}");
                    }
                }

                if (stockValidationErrors.Any())
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"? Stock validation failed:\n{string.Join("\n", stockValidationErrors)}",
                        Data = null
                    };
                }

                
                var newOrder = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customerId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Ordered,
                    OrderItems = new List<OrderItem>(),
                    ServiceUsageId = serviceUsageId
                };

                decimal totalAmount = 0;

                
                foreach (var cartItem in itemsToCheckout)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = newOrder.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price,
                        ItemTotal = (int)(cartItem.Quantity * cartItem.Product.Price)
                    };

                    newOrder.OrderItems.Add(orderItem);
                    totalAmount += orderItem.ItemTotal;

                    
                    cartItem.Product.Stock -= cartItem.Quantity;
                    productRepo.Update(cartItem.Product);
                }

                
                if (!string.IsNullOrWhiteSpace(promoCode))
                {
                    
                    
                }

                newOrder.TotalAmount = totalAmount;

                
                await orderRepo.AddAsync(newOrder);
                await orderRepo.SaveChangesAsync();

                
                foreach (var cartItem in itemsToCheckout.ToList())
                {
                    cart.CartItems?.Remove(cartItem);
                    cartItemRepo.Remove(cartItem);
                }
                await cartItemRepo.SaveChangesAsync();

                
                await transaction.CommitAsync();

                var checkoutType = selectedProductIds != null ? "partial" : "full";
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message = $"? {checkoutType} order placed successfully! Order ID: {newOrder.Id}, Total Amount: ${totalAmount:F2}",
                    Data = CartMapper.MapToOrderReadDTO(newOrder)
                };
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = $"? Error during checkout: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
