using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class TechManager
    {
        public int Id { get; set; }
        public string Specialization { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Product>? ManagedProducts { get; set; }
    }
   
}
