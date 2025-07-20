using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum ProductPendingStatus
    {
        [StringValue("None")]
        None = 0,
        [StringValue("Pending")]
        Pending = 1,
        [StringValue("Approved")]
        Approved = 2,
        [StringValue("Rejected")]
        Rejected = 4
    }
}
