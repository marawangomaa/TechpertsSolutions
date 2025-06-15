using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class SalesManager
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
    
}
