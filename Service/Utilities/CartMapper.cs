// Service.Utilities.CartMapper.cs
using Core.DTOs.CartDTOs;
using Core.DTOs.OrderDTOs;
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
            if (cart == null)
            {
                return null;
            }

            var cartItemsReadDTO = cart.CartItems != null
                ? cart.CartItems.Where(item => item != null)
                               .Select(MapToCartItemReadDTO)
                               .Where(dto => dto != null)
                               .ToList()
                : new List<CartItemReadDTO>();

            // Calculate subtotal from the mapped cart items
            decimal subTotal = cartItemsReadDTO.Sum(item => item.ItemTotal);

            return new CartReadDTO
            {
                Id = cart.Id ?? string.Empty,
                CustomerId = cart.CustomerId ?? string.Empty,
                CreatedAt = cart.CreatedAt,
                CartItems = cartItemsReadDTO,
                SubTotal = subTotal // Assign calculated subtotal
            };
        }

        public static CartItemReadDTO MapToCartItemReadDTO(CartItem item)
        {
            if (item == null)
            {
                return null;
            }

            return new CartItemReadDTO
            {
                Id = item.Id ?? string.Empty,
                ProductId = item.ProductId ?? string.Empty,
                ProductName = item.Product?.Name ?? "Unknown Product",
                Price = item.Product?.Price ?? 0, // Ensure Product is loaded for price
                Quantity = item.Quantity, // New: Map quantity
                ImageUrl = item.Product?.ImageUrl ?? string.Empty, // New: Map image URL
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
            if (order == null)
            {
                return null;
            }

            return new OrderReadDTO
            {
                Id = order.Id ?? string.Empty,
                CustomerId = order.CustomerId ?? string.Empty,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status ?? "Unknown",
                OrderItems = order.OrderItems != null
                    ? order.OrderItems.Where(item => item != null)
                                     .Select(MapToOrderItemReadDTO)
                                     .Where(dto => dto != null)
                                     .ToList()
                    : new List<OrderItemReadDTO>()
            };
        }

   
        public static OrderItemReadDTO MapToOrderItemReadDTO(OrderItem orderItem)
        {
            if (orderItem == null)
            {
                return null;
            }

            return new OrderItemReadDTO
            {
                Id = orderItem.Id ?? string.Empty,
                ProductId = orderItem.ProductId ?? string.Empty,
                ProductName = orderItem.Product?.Name ?? "Unknown Product", // Assuming Product is loaded
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                ImageUrl = orderItem.Product?.ImageUrl ?? string.Empty,
                ItemTotal = orderItem.ItemTotal
            };
        }
    }
}
