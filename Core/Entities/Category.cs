using Core.Entities;
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
        public List<SubCategory>? SubCategories { get; set; } = new List<SubCategory>();
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
