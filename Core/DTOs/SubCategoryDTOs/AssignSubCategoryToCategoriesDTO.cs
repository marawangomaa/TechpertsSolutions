using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SubCategoryDTOs
{
    public class AssignSubCategoryToCategoriesDTO
    {
        public string SubCategoryId { get; set; }
        public List<string> CategoryIds { get; set; } = new List<string>();
    }
} 