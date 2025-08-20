using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCAssemblyItemReadDTO
    {
        public string ItemId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public string? SubCategoryName { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        //public PCAssemblyItemReadDTO(PCAssemblyItem pCAssemblyItem)
        //{
        //    //ItemId = pCAssemblyItem.ItemId;
        //    ProductId = pCAssemblyItem.ProductId;
        //    //ProductName = pCAssemblyItem.ProductName;
        //    //ProductImageUrl = pCAssemblyItem.ProductImageUrl;
        //    //SubCategoryName = pCAssemblyItem.SubCategoryName;
        //    //Category = pCAssemblyItem.Category;
        //    //Status = pCAssemblyItem.Status;
        //    Price = pCAssemblyItem.Price;
        //    //Discount = pCAssemblyItem.Discount;
        //    Quantity = pCAssemblyItem.Quantity;
        //    Total = pCAssemblyItem.Total;
        //}
    }
}
