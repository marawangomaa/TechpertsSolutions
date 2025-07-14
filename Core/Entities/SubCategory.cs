using Core.Entities;
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

        public string CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
