// Service.CartService.cs
using Core.DTOs.Cart;
using Core.DTOs.Orders;
using Core.Interfaces;
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
            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                return null; // Or throw a specific exception
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

     
        public async Task<string> AddItemAsync(string customerId, CartItemDTO itemDto)
        {
            if (itemDto.Quantity <= 0)
                return "❌ Quantity must be greater than zero.";

            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                return $"❌ Customer with ID {customerId} does not exist.";

            var product = await productRepo.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                return $"❌ Product with ID {itemDto.ProductId} does not exist.";

            if (product.Stock < itemDto.Quantity)
                return $"❌ Not enough stock for product {product.Name}. Available: {product.Stock}.";

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
            if (updateDto.Quantity <= 0)
                return "❌ Quantity must be greater than zero. To remove, use the remove endpoint.";

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
                return $"❌ Not enough stock for product {itemToUpdate.Product.Name}. Available: {itemToUpdate.Product.Stock}. Requested: {updateDto.Quantity}.";

            itemToUpdate.Quantity = updateDto.Quantity;
            cartItemRepo.Update(itemToUpdate);
            await cartItemRepo.SaveChangesAsync();

            return "✅ Item quantity updated successfully.";
        }


        public async Task<string> RemoveItemAsync(string customerId, string productId)
        {
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

      
        public async Task<GeneralResponse<OrderReadDTO>> PlaceOrderAsync(string customerId)
        {
            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems.Product" // Crucial to load product details for validation
            );

            if (cart == null || !cart.CartItems.Any())
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "❌ Cart is empty or not found for this customer.",
                    Data = null
                };
            }

            // 1. Validate stock for all items in the cart
            foreach (var cartItem in cart.CartItems)
            {
                if (cartItem.Product == null)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"❌ Product details missing for item ID {cartItem.ProductId}.",
                        Data = null
                    };
                }

                if (cartItem.Product.Stock < cartItem.Quantity)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"❌ Not enough stock for '{cartItem.Product.Name}'. Available: {cartItem.Product.Stock}, Requested: {cartItem.Quantity}.",
                        Data = null
                    };
                }
            }

            // 2. Create the Order entity
            var newOrder = new Order
            {
                Id = Guid.NewGuid().ToString(), // Generate a new GUID for the order
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending", // Initial status
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            // 3. Create OrderItem entities from CartItems and update product stock
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToString(), // Generate a new GUID for the order item
                    OrderId = newOrder.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price // Use current product price
                };
                newOrder.OrderItems.Add(orderItem);
                totalAmount += orderItem.ItemTotal;

                // Update product stock
                cartItem.Product.Stock -= cartItem.Quantity;
                productRepo.Update(cartItem.Product);
            }

            newOrder.TotalAmount = totalAmount;

            // 4. Save the new order and updated products
            await orderRepo.AddAsync(newOrder);
            await orderRepo.SaveChangesAsync(); // Save order and product stock changes

            // 5. Clear the cart after successful order placement
            await ClearCartAsync(customerId); // Re-use existing method

            return new GeneralResponse<OrderReadDTO>
            {
                Success = true,
                Message = "✅ Order placed successfully!",
                Data = CartMapper.MapToOrderReadDTO(newOrder) // Map the created order to DTO
            };
        }
    }
}
