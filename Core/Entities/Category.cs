using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        
        public List<CategorySubCategory>? SubCategories { get; set; } = new();
        
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
