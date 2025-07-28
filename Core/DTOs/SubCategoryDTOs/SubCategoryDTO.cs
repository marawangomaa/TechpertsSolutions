using Core.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SubCategoryDTOs
{
    public class SubCategoryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; } 
        public List<ProductListItemDTO> Products { get; set; } = new List<ProductListItemDTO>();
    }
}
