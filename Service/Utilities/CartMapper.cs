using Core.DTOs.CartDTOs;
using Core.DTOs.OrderDTOs;
using Core.Enums;
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

            var cartItemsReadDTO = cart.CartItems?
                                     .Where(item => item != null)
                                     .Select(MapToCartItemReadDTO)
                                     .Where(dto => dto != null)
                                     .ToList()
                                 ?? new List<CartItemReadDTO>();

            decimal subTotal = cartItemsReadDTO.Sum(item => item.ItemTotal);

            return new CartReadDTO
            {
                Id = cart.Id ?? string.Empty,
                CustomerId = cart.CustomerId ?? string.Empty,
                CreatedAt = cart.CreatedAt,
                CartItems = cartItemsReadDTO,
                SubTotal = subTotal
            };
        }

        public static CartItemReadDTO MapToCartItemReadDTO(CartItem item)
        {
            if (item == null)
                return null;

            // This mapper assumes the CartService has already calculated the final prices
            // and has passed a DTO with the correct values.
            return new CartItemReadDTO
            {
                Id = item.Id ?? string.Empty,
                ProductId = item.ProductId ?? string.Empty,
                ProductName = item.Product?.Name ?? "Unknown Product",
                UnitPrice = item.UnitPrice, // UnitPrice should be the final calculated price from the service
                Quantity = item.Quantity,
                ImageUrl = item.Product?.ImageUrl ?? string.Empty,
                Stock = item.Product?.Stock ?? 0,
                // Assuming ItemTotal is a calculated property in CartItemReadDTO
                ItemTotal = item.UnitPrice * item.Quantity
            };
        }

        public static CartItem MapToCartItemEntity(CartItemDTO dto)
        {
            return new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
        }

        public static CartItemDTO MapToCartItemDTO(CartItem item)
        {
            return new CartItemDTO
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
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
                Status = order.Status,
                OrderItems = order.OrderItems?
                                .Where(item => item != null)
                                .Select(MapToOrderItemReadDTO)
                                .Where(dto => dto != null)
                                .ToList()
                            ?? new List<OrderItemReadDTO>()
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
                ProductName = orderItem.Product?.Name ?? "Unknown Product",
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                ImageUrl = orderItem.Product?.ImageUrl ?? string.Empty,
                ItemTotal = orderItem.ItemTotal
            };
        }
    }
}