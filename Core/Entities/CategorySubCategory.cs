using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class CategorySubCategory
    {
        public string CategoryId { get; set; }
        public Category Category { get; set; }
        public string SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        
        // Composite primary key for many-to-many relationship
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
