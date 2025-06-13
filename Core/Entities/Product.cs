using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }

        public ICollection<Specification>? Specifications { get; set; }
        public ICollection<Warranty>? Warranties { get; set; }

        public int TechManagerId { get; set; }
        public TechManager? TechManager { get; set; }

        public int StockControlManagerId { get; set; }
        public StockControlManager? StockControlManager { get; set; }
    }
}
