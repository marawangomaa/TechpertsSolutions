using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class OrderHistory : BaseEntity
    {
        public List<Order>? Orders { get; set; } = new List<Order>();
    }
}
