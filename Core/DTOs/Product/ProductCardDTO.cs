using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Utilities;

namespace Core.DTOs.Product
{
    public class ProductCardDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }  
        public string? ImageUrl { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
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
        public string? SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }

        public string Status { get; set; } = "Pending";
    

    }
}
