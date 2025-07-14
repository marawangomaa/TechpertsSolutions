// Service.Utilities.CartMapper.cs
using Core.DTOs.Cart;
using Core.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class CartMapper
    {
        /// <summary>
        /// Maps a Cart entity to a CartReadDTO, including detailed cart items and subtotal calculation.
        /// </summary>
        /// <param name="cart">The Cart entity to map.</param>
        /// <returns>A CartReadDTO.</returns>
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

        /// <summary>
        /// Maps a CartItem entity to a CartItemReadDTO, including product details.
        /// </summary>
        /// <param name="item">The CartItem entity to map.</param>
        /// <returns>A CartItemReadDTO.</returns>
        public static CartItemReadDTO MapToCartItemReadDTO(CartItem item)
        {
            return new CartItemReadDTO
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name,
                Price = item.Product?.Price ?? 0, // Ensure Product is loaded for price
                Quantity = item.Quantity, // New: Map quantity
                ImageUrl = item.Product?.ImageUrl, // New: Map image URL
                StockQuantity = item.Product?.StockQuantity ?? 0 // New: Map stock quantity
            };
        }

        /// <summary>
        /// Maps a CartItemDTO to a CartItem entity for adding/updating.
        /// </summary>
        /// <param name="dto">The CartItemDTO.</param>
        /// <returns>A CartItem entity.</returns>
        public static CartItem MapToCartItemEntity(CartItemDTO dto)
        {
            return new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity // New: Map quantity
                // CartId is set when adding to an existing cart
            };
        }

        /// <summary>
        /// Maps a CartItem entity to a CartItemDTO.
        /// </summary>
        /// <param name="item">The CartItem entity.</param>
        /// <returns>A CartItemDTO.</returns>
        public static CartItemDTO MapToCartItemDTO(CartItem item)
        {
            return new CartItemDTO
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity // New: Map quantity
            };
        }

        /// <summary>
        /// Maps a Cart entity and its items to an OrderReadDTO.
        /// </summary>
        /// <param name="order">The Order entity to map.</param>
        /// <returns>An OrderReadDTO.</returns>
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

        /// <summary>
        /// Maps an OrderItem entity to an OrderItemReadDTO.
        /// </summary>
        /// <param name="orderItem">The OrderItem entity to map.</param>
        /// <returns>An OrderItemReadDTO.</returns>
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
