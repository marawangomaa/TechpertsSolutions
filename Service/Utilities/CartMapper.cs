// Service.Utilities.CartMapper.cs
using Core.DTOs.Cart;
using Core.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class CartMapper
    {
       
        public static CartReadDTO MapToCartReadDTO(Cart cart)
        {
            var cartItemsReadDTO = cart.CartItems?
                .Select(MapToCartItemReadDTO)
                .ToList() ?? new List<CartItemReadDTO>();

            // Calculate subtotal from the mapped cart items
            decimal subTotal = cartItemsReadDTO.Sum(item => item.ItemTotal);

            return new CartReadDTO
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                CreatedAt = cart.CreatedAt,
                CartItems = cartItemsReadDTO,
                SubTotal = subTotal // Assign calculated subtotal
            };
        }

        public static CartItemReadDTO MapToCartItemReadDTO(CartItem item)
        {
            return new CartItemReadDTO
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Price = item.Product?.Price ?? 0, // Ensure Product is loaded for price
                Quantity = item.Quantity, // New: Map quantity
                ImageUrl = item.Product.ImageUrl, // New: Map image URL
                Stock = item.Product?.Stock ?? 0 // New: Map stock quantity
            };
        }

        
        public static CartItem MapToCartItemEntity(CartItemDTO dto)
        {
            return new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity // New: Map quantity
                // CartId is set when adding to an existing cart
            };
        }
        public static CartItemDTO MapToCartItemDTO(CartItem item)
        {
            return new CartItemDTO
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity // New: Map quantity
            };
        }

        public static OrderReadDTO MapToOrderReadDTO(Order order)
        {
            return new OrderReadDTO
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = order.OrderItems?.Select(MapToOrderItemReadDTO).ToList() ?? new List<OrderItemReadDTO>()
            };
        }

   
        public static OrderItemReadDTO MapToOrderItemReadDTO(OrderItem orderItem)
        {
            return new OrderItemReadDTO
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductName = orderItem.Product?.Name, // Assuming Product is loaded
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice
            };
        }
    }
}
