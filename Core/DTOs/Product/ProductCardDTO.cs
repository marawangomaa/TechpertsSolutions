using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Product
{
    public class ProductCardDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }  
        public string? ImageUrl { get; set; }

        public string? CategoryName { get; set; }
        public string? SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }

        public string Status { get; set; } = "Pending";
    

    }
}
