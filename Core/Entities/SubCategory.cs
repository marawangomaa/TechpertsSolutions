using TechpertsSolutions.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class SubCategory : BaseEntity
    {
        public string Name { get; set; }
        public string? Image { get; set; }

        public string CategoryId { get; set; }
        public Category? Category { get; set; }

        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
