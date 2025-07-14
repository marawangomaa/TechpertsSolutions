using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class OrderHistory : BaseEntity
    {
        public ICollection<Order>? Orders { get; set; }
    }
}
