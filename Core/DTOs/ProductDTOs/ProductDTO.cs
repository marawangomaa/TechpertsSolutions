using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;

namespace Core.DTOs.ProductDTOs
{
    public class ProductDTO
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public ProductPendingStatus Status { get; set; }
        public string? CategoryId { get; set; }
        public string? SubCategoryId { get; set; }
        public string? TechManagerId { get; set; }
        public string? StockControlManagerId { get; set; }
        public string CategoryName { get; set; } = null!;
        private ProductCategory? _categoryEnum;
        private string _categoryEnumError;
        public ProductCategory? CategoryEnum
        {
            get
            {
                if (_categoryEnum != null) return _categoryEnum;
                try
                {
                    _categoryEnum = EnumExtensions.ParseFromStringValue<ProductCategory>(CategoryName);
                    _categoryEnumError = null;
                }
                catch (System.ArgumentException)
                {
                    _categoryEnum = null;
                    _categoryEnumError = $"Unknown category: '{CategoryName}'";
                }
                return _categoryEnum;
            }
        }
        public string CategoryEnumError => _categoryEnumError;
        public string? SubCategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string TechManagerName { get; set; } = null!;
        public string StockControlManagerName { get; set; } = null!;

        public List<SpecificationDTO>? Specifications { get; set; }
        public List<WarrantyDTO>? Warranties { get; set; }  
    }
}
