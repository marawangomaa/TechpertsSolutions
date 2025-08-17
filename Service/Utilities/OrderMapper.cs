using Core.DTOs.OrderDTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class OrderMapper
    {
        public static OrderReadDTO ToReadDTO(Order order)
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
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                //DeliveryName = order.Delivery?.DeliveryPerson?.User?.FullName?? "UnKnown Delivery",
                OrderItems = order.OrderItems != null 
                    ? order.OrderItems.Select(ToItemReadDTO).ToList() 
                    : new List<OrderItemReadDTO>()
            };
        }

        public static OrderItemReadDTO ToItemReadDTO(OrderItem item)
        {
            if (item == null)
            {
                return null;
            }

            return new OrderItemReadDTO
            {
                Id = item.Id ?? string.Empty,
                ProductId = item.ProductId ?? string.Empty,
                ProductName = item.Product?.Name ?? "Unknown Product",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ImageUrl = item.Product?.ImageUrl ?? "assets/products/default-product.png",
                ItemTotal = item.ItemTotal
            };
        }

        public static Order ToEntity(OrderCreateDTO dto)
        {
            return new Order
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Ordered,
                ServiceUsageId = dto.ServiceUsageId,
                OrderHistoryId = null,
                OrderItems = dto.OrderItems?.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ItemTotal = (int)(item.Quantity * item.UnitPrice)
                }).ToList() ?? new List<OrderItem>()
            };
        }
    }
}
