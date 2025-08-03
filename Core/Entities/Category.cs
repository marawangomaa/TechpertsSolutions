using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        
        // Direct relationship with SubCategories (for assigned subcategories)
        public List<SubCategory>? SubCategories { get; set; } = new();
        
        // Many-to-many relationship through CategorySubCategory (for flexible assignments)
        public List<CategorySubCategory>? CategorySubCategories { get; set; } = new();
        
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
