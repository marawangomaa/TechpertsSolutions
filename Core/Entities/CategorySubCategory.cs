using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Interfaces;

namespace TechpertsSolutions.Core.Entities
{
    public class CategorySubCategory : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CategoryId { get; set; }
        public Category? Category { get; set; }
        public string SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }
        
        // Composite primary key for many-to-many relationship
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
