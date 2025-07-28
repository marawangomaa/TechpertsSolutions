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
<<<<<<< HEAD
        public string? Name { get; set; }
=======
        public string Name { get; set; }
>>>>>>> 6af70478989f30039b678b4227a199f37565f380
        public string? Description { get; set; }
        public string? Image { get; set; }
        public List<SubCategory>? SubCategories { get; set; } = new List<SubCategory>();
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
