using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SubCategoryDTOs
{
    public class SubCategoryByCategoryDTO
    {
        public ProductCategory Category { get; set; }
        public List<string> SubCategoryNames { get; set; } = new List<string>();
    }
} 