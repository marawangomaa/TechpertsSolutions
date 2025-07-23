using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Utilities;

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
        private ProductCategory? _categoryEnum;
        private string _categoryEnumError;
        public ProductCategory? CategoryEnum
        {
            get
            {
                if (_categoryEnum != null) return _categoryEnum;
                try
                {
                    _categoryEnum = EnumExtensions.ParseFromStringValue<ProductCategory>(Category?.Name);
                    _categoryEnumError = null;
                }
                catch (System.ArgumentException)
                {
                    _categoryEnum = null;
                    _categoryEnumError = $"Unknown category: '{Category?.Name}'";
                }
                return _categoryEnum;
            }
        }
        public string CategoryEnumError => _categoryEnumError;
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
