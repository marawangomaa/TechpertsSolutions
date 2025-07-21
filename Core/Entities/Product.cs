using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }
        public ProductPendingStatus status { get; set; } = ProductPendingStatus.Pending;
        public List<Specification>? Specifications { get; set; } = new List<Specification>();
        public List<Warranty>? Warranties { get; set; } = new List<Warranty>();

        public string TechManagerId { get; set; }
        public TechManager? TechManager { get; set; }

        public string StockControlManagerId { get; set; }
        public StockControlManager? StockControlManager { get; set; }
        public List<CartItem>? CartItems { get; set; } = new List<CartItem>();
        public List<WishListItem>? WishListItems { get; set; } = new List<WishListItem>();
        public List<PCAssemblyItem>? PCAssemblyItems { get; set; } = new List<PCAssemblyItem>();
        public List<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
    }
}
