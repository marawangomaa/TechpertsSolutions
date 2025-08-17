using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum DeliveryOfferStatus
    {
        [StringValue("Pending")]
        Pending = 0,

        [StringValue("Accepted")]
        Accepted = 1,

        [StringValue("Declined")]
        Declined = 2,

        [StringValue("Canceled")]
        Canceled = 3,

        [StringValue("Expired")]
        Expired = 4
    }
}
