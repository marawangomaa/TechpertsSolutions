// Service.CartService.cs
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
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<CartItem> cartItemRepo;
        private readonly IRepository<Product> productRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<Order> orderRepo; // New: For order creation
        private readonly IRepository<OrderItem> orderItemRepo; // New: For order item creation

        public CartService(
            IRepository<Cart> _cartRepo,
            IRepository<CartItem> _cartItemRepo,
            IRepository<Product> _productRepo,
            IRepository<Customer> _customerRepo,
            IRepository<Order> _orderRepo, // Inject Order repository
            IRepository<OrderItem> _orderItemRepo) // Inject OrderItem repository
        {
            cartRepo = _cartRepo;
            cartItemRepo = _cartItemRepo;
            productRepo = _productRepo;
            customerRepo = _customerRepo;
            orderRepo = _orderRepo;
            orderItemRepo = _orderItemRepo;
        }

        public async Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId)
        {
            // Input validation
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
                return null; // Customer not found
            }

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems.Product" // Include Product details for CartItems
            );

            if (cart == null)
            {
                // Create a new cart if one doesn't exist for the customer
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
            // Input validation
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

                var cart = await cartRepo.GetFirstOrDefaultAsync(
                    c => c.CustomerId == customerId,
                    includeProperties: "CartItems.Product" // Include Product details for CartItems
                );

                if (cart == null)
                {
                    // Create a new cart if one doesn't exist for the customer
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
                    Message = $"An error occurred while getting or creating cart: {ex.Message}",
                    Data = null
                };
            }
        }

     
        public async Task<string> AddItemAsync(string customerId, CartItemDTO itemDto)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId))
                return "❌ Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "❌ Invalid Customer ID format. Expected GUID format.";

            if (itemDto == null)
                return "❌ Item data cannot be null.";

            if (string.IsNullOrWhiteSpace(itemDto.ProductId))
                return "❌ Product ID cannot be null or empty.";

            if (!Guid.TryParse(itemDto.ProductId, out _))
                return "❌ Invalid Product ID format. Expected GUID format.";

            if (itemDto.Quantity <= 0)
                return "❌ Quantity must be greater than zero.";

            if (itemDto.Quantity > 1000) // Reasonable upper limit
                return "❌ Quantity cannot exceed 1000 items.";

            // Validate customer exists
            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                return $"❌ Customer with ID {customerId} not found.";

            // Validate product exists
            var product = await productRepo.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                return $"❌ Product with ID {itemDto.ProductId} not found.";

            if (product.Stock < itemDto.Quantity)
                return $"❌ Not enough stock for product '{product.Name}'. Available: {product.Stock}, Requested: {itemDto.Quantity}.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems"
            );

            // Create cart if it doesn't exist
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                await cartRepo.AddAsync(cart);
                await cartRepo.SaveChangesAsync(); // Save to get cart.Id
            }

            var existingItem = cart.CartItems?.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                // If item exists, update quantity
                int newQuantity = existingItem.Quantity + itemDto.Quantity;
                if (product.Stock < newQuantity)
                    return $"❌ Cannot add {itemDto.Quantity} more. Total requested ({newQuantity}) exceeds available stock ({product.Stock}).";

                existingItem.Quantity = newQuantity;
                cartItemRepo.Update(existingItem);
                await cartItemRepo.SaveChangesAsync();
                return "✅ Item quantity updated successfully.";
            }
            else
            {
                // Add new item to cart
                var newItem = new CartItem
                {
                    ProductId = itemDto.ProductId,
                    CartId = cart.Id,
                    Quantity = itemDto.Quantity
                };

                await cartItemRepo.AddAsync(newItem);
                await cartItemRepo.SaveChangesAsync();
                return "✅ Item added successfully.";
            }
        }

       
        public async Task<string> UpdateItemQuantityAsync(string customerId, CartUpdateItemQuantityDTO updateDto)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId))
                return "❌ Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "❌ Invalid Customer ID format. Expected GUID format.";

            if (updateDto == null)
                return "❌ Update data cannot be null.";

            if (string.IsNullOrWhiteSpace(updateDto.ProductId))
                return "❌ Product ID cannot be null or empty.";

            if (!Guid.TryParse(updateDto.ProductId, out _))
                return "❌ Invalid Product ID format. Expected GUID format.";

            if (updateDto.Quantity <= 0)
                return "❌ Quantity must be greater than zero. To remove, use the remove endpoint.";

            if (updateDto.Quantity > 1000) // Reasonable upper limit
                return "❌ Quantity cannot exceed 1000 items.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems.Product" // Include Product to check stock
            );

            if (cart == null)
                return $"❌ Cart not found for customer ID {customerId}.";

            var itemToUpdate = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == updateDto.ProductId);
            if (itemToUpdate == null)
                return $"❌ Product with ID {updateDto.ProductId} not found in cart.";

            // Check stock availability for the new quantity
            if (itemToUpdate.Product == null)
                return "❌ Product details not available for stock check."; // Should not happen with includeProperties

            if (itemToUpdate.Product.Stock < updateDto.Quantity)
                return $"❌ Not enough stock for product '{itemToUpdate.Product.Name}'. Available: {itemToUpdate.Product.Stock}, Requested: {updateDto.Quantity}.";

            itemToUpdate.Quantity = updateDto.Quantity;
            cartItemRepo.Update(itemToUpdate);
            await cartItemRepo.SaveChangesAsync();

            return "✅ Item quantity updated successfully.";
        }


        public async Task<string> RemoveItemAsync(string customerId, string productId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId))
                return "❌ Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "❌ Invalid Customer ID format. Expected GUID format.";

            if (string.IsNullOrWhiteSpace(productId))
                return "❌ Product ID cannot be null or empty.";

            if (!Guid.TryParse(productId, out _))
                return "❌ Invalid Product ID format. Expected GUID format.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems"
            );

            if (cart == null)
                return $"❌ Cart not found for customer ID {customerId}.";

            var item = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null)
                return $"❌ Product with ID {productId} not found in cart.";

            cart.CartItems.Remove(item); // Remove from navigation property
            cartItemRepo.Remove(item); // Mark for deletion in DB
            await cartItemRepo.SaveChangesAsync();

            return "✅ Item removed successfully.";
        }

 
        public async Task<string> ClearCartAsync(string customerId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId))
                return "❌ Customer ID cannot be null or empty.";

            if (!Guid.TryParse(customerId, out _))
                return "❌ Invalid Customer ID format. Expected GUID format.";

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems"
            );

            if (cart == null)
                return $"❌ Cart not found for customer ID {customerId}.";

            if (cart.CartItems == null || !cart.CartItems.Any())
                return "ℹ️ Cart is already empty.";

            // Remove all items from the cart
            foreach (var item in cart.CartItems.ToList()) // Use ToList() to avoid modification during iteration
            {
                cart.CartItems.Remove(item);
                cartItemRepo.Remove(item);
            }
            await cartItemRepo.SaveChangesAsync();

            return "✅ Cart cleared successfully.";
        }

      
        public async Task<GeneralResponse<OrderReadDTO>> PlaceOrderAsync(string customerId, string? deliveryId = null, string? serviceUsageId = null)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "❌ Customer ID is required.",
                    Data = null
                };
            }

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems.Product,Customer"
            );

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "❌ Cart is empty or not found.",
                    Data = null
                };
            }

            // 1. Validate stock availability
            var stockValidationErrors = new List<string>();
            foreach (var cartItem in cart.CartItems)
            {
                if (cartItem.Product.Stock < cartItem.Quantity)
                {
                    stockValidationErrors.Add($"❌ {cartItem.Product.Name}: Insufficient stock. Available: {cartItem.Product.Stock}, Requested: {cartItem.Quantity}");
                }
            }

            if (stockValidationErrors.Any())
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = $"❌ Stock validation failed:\n{string.Join("\n", stockValidationErrors)}",
                    Data = null
                };
            }

            try
            {
                var newOrder = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customerId,
                    CartId = cart.Id,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>(),
                    DeliveryId = deliveryId, // Optional - can be null
                    ServiceUsageId = serviceUsageId // Optional - can be null
                };

                decimal totalAmount = 0;

                // 3. Create OrderItem entities from CartItems and update product stock
                foreach (var cartItem in cart.CartItems)
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

                newOrder.TotalAmount = totalAmount;

                
                await orderRepo.AddAsync(newOrder);
                await orderRepo.SaveChangesAsync();

                
                await ClearCartAsync(customerId);

                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message = $"✅ Order placed successfully! Order ID: {newOrder.Id}, Total Amount: ${totalAmount:F2}",
                    Data = CartMapper.MapToOrderReadDTO(newOrder)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = $"❌ Error creating order: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<OrderReadDTO>> PartialCheckoutAsync(string customerId, List<string> productIds, string? promoCode = null)
        {
            if (string.IsNullOrWhiteSpace(customerId) || productIds == null || !productIds.Any())
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "❌ Customer ID and product selection are required.",
                    Data = null
                };
            }

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems.Product,Customer"
            );

            if (cart == null)
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "❌ Cart not found for this customer.",
                    Data = null
                };
            }

            var selectedItems = cart.CartItems?.Where(ci => productIds.Contains(ci.ProductId)).ToList();
            if (selectedItems == null || !selectedItems.Any())
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "❌ None of the selected products are in the cart.",
                    Data = null
                };
            }

            // Validate stock for selected items
            var stockValidationErrors = new List<string>();
            foreach (var cartItem in selectedItems)
            {
                if (cartItem.Product == null)
                {
                    stockValidationErrors.Add($"Product with ID {cartItem.ProductId} not found.");
                    continue;
                }
                if (cartItem.Quantity <= 0)
                {
                    stockValidationErrors.Add($"Invalid quantity ({cartItem.Quantity}) for product '{cartItem.Product.Name}'.");
                    continue;
                }
                if (cartItem.Product.Stock < cartItem.Quantity)
                {
                    stockValidationErrors.Add($"Not enough stock for '{cartItem.Product.Name}'. Available: {cartItem.Product.Stock}, Requested: {cartItem.Quantity}.");
                }
            }
            if (stockValidationErrors.Any())
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = $"❌ Stock validation failed:\n{string.Join("\n", stockValidationErrors)}",
                    Data = null
                };
            }

            try
            {
                var newOrder = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customerId,
                    CartId = cart.Id,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>(),
                    DeliveryId = Guid.NewGuid().ToString(),
                    ServiceUsageId = Guid.NewGuid().ToString()
                };
                decimal totalAmount = 0;
                foreach (var cartItem in selectedItems)
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
                // Apply promo code logic here if needed (future extensibility)
                newOrder.TotalAmount = totalAmount;
                await orderRepo.AddAsync(newOrder);
                await orderRepo.SaveChangesAsync();
                // Remove selected items from cart
                foreach (var cartItem in selectedItems)
                {
                    cart.CartItems.Remove(cartItem);
                    cartItemRepo.Remove(cartItem);
                }
                await cartItemRepo.SaveChangesAsync();
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message = $"✅ Partial order placed successfully! Order ID: {newOrder.Id}, Total Amount: ${totalAmount:F2}",
                    Data = CartMapper.MapToOrderReadDTO(newOrder)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = $"❌ Error creating partial order: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
