using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum DeliveryPersonStatus
    {
        [StringValue("Pending")]
        Pending = 0,

        [StringValue("Accepted")]
        Accepted = 1,

        [StringValue("Rejected")]
        Rejected = 2
    }
}
