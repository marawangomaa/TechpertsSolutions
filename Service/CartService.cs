// Service.CartService.cs
using Core.DTOs.Cart;
using Core.DTOs.Order;
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
    // Assuming these interfaces and entities are defined in Core.Interfaces and TechpertsSolutions.Core.Entities
    // public interface ICartService
    // {
    //     Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId);
    //     Task<string> AddItemAsync(string customerId, CartItemDTO itemDto);
    //     Task<string> UpdateItemQuantityAsync(string customerId, CartUpdateItemQuantityDTO updateDto); // New
    //     Task<string> RemoveItemAsync(string customerId, string productId);
    //     Task<string> ClearCartAsync(string customerId); // New
    //     Task<GeneralResponse<OrderReadDTO>> PlaceOrderAsync(string customerId); // New
    // }

    // public interface IRepository<T> where T : class
    // {
    //     Task<T?> GetByIdAsync(string id);
    //     Task<T?> GetFirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string includeProperties = null);
    //     Task<IEnumerable<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null, string includeProperties = null);
    //     Task AddAsync(T entity);
    //     void Update(T entity);
    //     void Remove(T entity);
    //     Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    //     Task SaveChanges();
    // }

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

        /// <summary>
        /// Retrieves a customer's cart, creating one if it doesn't exist.
        /// Includes cart items and product details for a comprehensive view.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>A CartReadDTO or null if customer not found.</returns>
        public async Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId)
        {
            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                Console.WriteLine($"Customer with ID {customerId} not found.");
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
                await cartRepo.SaveChanges();
            }

            return CartMapper.MapToCartReadDTO(cart);
        }

        /// <summary>
        /// Adds an item to the cart or updates its quantity if it already exists.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="itemDto">The CartItemDTO containing product ID and quantity.</param>
        /// <returns>A success or error message.</returns>
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

            if (product.StockQuantity < itemDto.Quantity)
                return $"❌ Not enough stock for product {product.Name}. Available: {product.StockQuantity}.";

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
                await cartRepo.SaveChanges(); // Save to get cart.Id
            }

            var existingItem = cart.CartItems?.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                // If item exists, update quantity
                int newQuantity = existingItem.Quantity + itemDto.Quantity;
                if (product.StockQuantity < newQuantity)
                    return $"❌ Cannot add {itemDto.Quantity} more. Total requested ({newQuantity}) exceeds available stock ({product.StockQuantity}).";

                existingItem.Quantity = newQuantity;
                cartItemRepo.Update(existingItem);
                await cartItemRepo.SaveChanges();
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
                await cartItemRepo.SaveChanges();
                return "✅ Item added successfully.";
            }
        }

        /// <summary>
        /// Updates the quantity of a specific item in the cart.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="updateDto">The DTO containing product ID and new quantity.</param>
        /// <returns>A success or error message.</returns>
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

            if (itemToUpdate.Product.StockQuantity < updateDto.Quantity)
                return $"❌ Not enough stock for product {itemToUpdate.Product.Name}. Available: {itemToUpdate.Product.StockQuantity}. Requested: {updateDto.Quantity}.";

            itemToUpdate.Quantity = updateDto.Quantity;
            cartItemRepo.Update(itemToUpdate);
            await cartItemRepo.SaveChanges();

            return "✅ Item quantity updated successfully.";
        }

        /// <summary>
        /// Removes a specific item from the cart.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="productId">The ID of the product to remove.</param>
        /// <returns>A success or error message.</returns>
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
            await cartItemRepo.SaveChanges();

            return "✅ Item removed successfully.";
        }

        /// <summary>
        /// Clears all items from a customer's cart.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>A success or error message.</returns>
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
            await cartItemRepo.SaveChanges();

            return "✅ Cart cleared successfully.";
        }

        /// <summary>
        /// Converts the current cart into a new order.
        /// Performs stock validation and updates, then clears the cart.
        /// </summary>
        /// <param name="customerId">The ID of the customer placing the order.</param>
        /// <returns>A GeneralResponse containing the created OrderReadDTO or an error message.</returns>
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

                if (cartItem.Product.StockQuantity < cartItem.Quantity)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"❌ Not enough stock for '{cartItem.Product.Name}'. Available: {cartItem.Product.StockQuantity}, Requested: {cartItem.Quantity}.",
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
                cartItem.Product.StockQuantity -= cartItem.Quantity;
                productRepo.Update(cartItem.Product);
            }

            newOrder.TotalAmount = totalAmount;

            // 4. Save the new order and updated products
            await orderRepo.AddAsync(newOrder);
            await orderRepo.SaveChanges(); // Save order and product stock changes

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
