using Core.DTOs.Orders;
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
            return new OrderReadDTO
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems?.Select(ToItemReadDTO).ToList() ?? new()
            };
        }

        public static OrderItemReadDTO ToItemReadDTO(OrderItem item)
        {
            return new OrderItemReadDTO
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
        }

        public static Order ToEntity(OrderCreateDTO dto)
        {
            return new Order
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                CartId = dto.CartId,
                DeliveryId = dto.DeliveryId,
                SalesManagerId = dto.SalesManagerId,
                ServiceUsageId = dto.ServiceUsageId,
                OrderItems = dto.OrderItems.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ItemTotal = (int)(item.Quantity * item.UnitPrice)
                }).ToList()
            };
        }
    }
}
