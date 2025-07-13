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
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ICollection<Order>? Orders { get; set; }
    }
}
