using Core.DTOs.Cart;
using Core.Interfaces;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<CartItem> cartItemRepo;
        private readonly IRepository<Product> productRepo;
        private readonly IRepository<Customer> customerRepo;

        public CartService(
            IRepository<Cart> _cartRepo,
            IRepository<CartItem> _cartItemRepo,
            IRepository<Product> _productRepo,
            IRepository<Customer> _customerRepo)
        {
            cartRepo = _cartRepo;
            cartItemRepo = _cartItemRepo;
            productRepo = _productRepo;
            customerRepo = _customerRepo;
        }

        public async Task<CartReadDTO?> GetCartByCustomerIdAsync(string customerId)
        {
            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                return null;

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems.Product"
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
                await cartRepo.SaveChanges();
            }

            return new CartReadDTO
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                CreatedAt = cart.CreatedAt,
                CartItems = cart.CartItems.Select(item => new CartItemReadDTO
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name
                }).ToList()
            };
        }

        public async Task<string> AddItemAsync(string customerId, CartItemDTO itemDto)
        {
            // Validate Customer
            var customer = await customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                return $"❌ Customer with ID {customerId} does not exist.";

            // Validate Product (safely!)
            var productExists = await productRepo.AnyAsync(p => p.Id == itemDto.ProductId);
            if (!productExists)
                return $"❌ Product with ID {itemDto.ProductId} does not exist.";

            // Retrieve or create Cart
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
                await cartRepo.SaveChanges();
            }

            // Check if item already exists
            var existingItem = cart.CartItems?.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
            if (existingItem != null)
                return "ℹ️ Item already exists in cart.";

            // Add new item
            var newItem = new CartItem
            {
                ProductId = itemDto.ProductId,
                CartId = cart.Id
            };

            await cartItemRepo.AddAsync(newItem);
            await cartItemRepo.SaveChanges();

            return "✅ Item added successfully.";
        }

        public async Task<string> RemoveItemAsync(string customerId, string productId)
        {
            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                includeProperties: "CartItems"
            );

            if (cart == null)
                return $"Cart not found for customer ID {customerId}.";

            var item = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null)
                return $"Product with ID {productId} not found in cart.";

            cart.CartItems.Remove(item);
            cartItemRepo.Remove(item);
            await cartItemRepo.SaveChanges();

            return "Item removed successfully.";
        }
    }
}

