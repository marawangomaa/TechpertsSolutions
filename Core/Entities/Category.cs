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
        public string Id { get; set; } = new Guid().ToString();
        public string Name { get; set; }

        public ICollection<SubCategory>? SubCategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
