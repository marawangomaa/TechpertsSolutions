
using Core.DTOs.CartDTOs;
using Core.DTOs.OrderDTOs;
using Core.Enums;
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

            bool isCustom = item.IsCustomBuild;

            return new CartItemReadDTO
            {
                Id = item.Id ?? string.Empty,
                ProductId = isCustom ? item.PcAssemblyId : item.ProductId,
                ProductName = isCustom
                    ? item.PCAssembly?.Name ?? "Custom PC Build"
                    : item.Product?.Name ?? "Unknown Product",
                Price = isCustom
                    ? item.UnitPrice  // Save this when adding to cart
                    : item.Product?.Price ?? 0,
                Quantity = item.Quantity,
                ImageUrl = isCustom
                    ? "/images/custom-build.png" // Replace with actual custom build image if any
                    : item.Product?.ImageUrl ?? string.Empty,
                Stock = isCustom
                    ? 1 // For custom build assume 1 always
                    : item.Product?.Stock ?? 0,
                IsCustomBuild = isCustom
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
                ProductName = orderItem.Product?.Name ?? "Unknown Product", 
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                ImageUrl = orderItem.Product?.ImageUrl ?? string.Empty,
                ItemTotal = orderItem.ItemTotal
            };
        }
    }
}
