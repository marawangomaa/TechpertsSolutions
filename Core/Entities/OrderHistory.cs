using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class OrderHistory : BaseEntity
    {
        public List<Order>? Orders { get; set; } = new List<Order>();
    }
}
