using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class StockControlManager
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Product> ControlledProducts { get; set; }
    }
    
}
