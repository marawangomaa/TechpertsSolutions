using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProductDTOs
{
    public class ProductListItemDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Image1Url { get; set; }
        public string? Image2Url { get; set; }
        public string? Image3Url { get; set; }
        public string? Image4Url { get; set; }
        public decimal Price { get; set; }
    }
}
