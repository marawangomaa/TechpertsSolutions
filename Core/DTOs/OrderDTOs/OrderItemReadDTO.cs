using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.OrderDTOs
{
    public class OrderItemReadDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public decimal ItemTotal { get; set; }

        //public OrderItemReadDTO(OrderItem orderItem)
        //{
        //    Id = orderItem.Id;
        //    ProductId = orderItem.ProductId;
        //    //ProductName = orderItem.ProductName;
        //    Quantity = orderItem.Quantity;
        //    UnitPrice = orderItem.UnitPrice;
        //    //ImageUrl = orderItem.ImageUrl;
        //    ItemTotal = orderItem.ItemTotal;
        //}
    }
}
