
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
                ProductId = item.ProductId,
                ProductName = item.Product.Name ?? "Default Name",
                Price = isCustom
                    ? item.UnitPrice
                    : item.Product?.Price ?? 0,
                Quantity = item.Quantity,
                ImageUrl = isCustom
                    ? "/images/custom-build.png"
                    : item.Product?.ImageUrl ?? "assets/products/default-product.png",
                Stock = isCustom
                    ? 1
                    : item.Product?.Stock ?? 0,
                IsCustomBuild = isCustom,

                AssemblyFee = isCustom ? item.AssemblyFee : null,
                ProductTotal = isCustom ? item.ProductTotal : null
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
                CustomerName = order.Customer?.User?.FullName ?? "Unknown Customer",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                //DeliveryName = order.Delivery?.DeliveryPerson?.User?.FullName ?? "No Delivery Assigned",
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
                ImageUrl = orderItem.Product?.ImageUrl ?? "assets/products/default-product.png",
                ItemTotal = orderItem.ItemTotal
            };
        }
    }
}
