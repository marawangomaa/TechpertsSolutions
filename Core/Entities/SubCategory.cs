using TechpertsSolutions.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class SubCategory : BaseEntity
    {
        public string Name { get; set; }
        public string? Image { get; set; }

        // Direct relationship with Category (for assigned subcategories)
        public string? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Many-to-many relationship through CategorySubCategory (for flexible assignments)
        public List<CategorySubCategory>? CategorySubCategories { get; set; } = new();

        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
