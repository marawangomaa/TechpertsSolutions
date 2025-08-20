using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.OrderDTOs
{
    public class OrderReadDTO
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemReadDTO> OrderItems { get; set; } = new List<OrderItemReadDTO>();
        //public OrderReadDTO(Order order)
        //{
        //    Id = order.Id;
        //    CustomerId = order.CustomerId;
        //    //CustomerName = order.CustomerName;
        //    OrderDate = order.OrderDate;
        //    //DeliveryName = order.DeliveryName;
        //    TotalAmount = order.TotalAmount;
        //    Status = order.Status;
        //    OrderItems = order.OrderItems.Select(item => new OrderItemReadDTO(item)).ToList();
        //}
    }
}
