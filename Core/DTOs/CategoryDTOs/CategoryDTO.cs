using Core.DTOs.ProductDTOs;
using Core.DTOs.SubCategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.CategoryDTOs
{
    public class CategoryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public string Image { get; set; }
        public List<ProductListItemDTO> products { get; set; } = new List<ProductListItemDTO>();
        public List<SubCategoryDTO> SubCategories { get; set; } = new List<SubCategoryDTO>();
    }
}
