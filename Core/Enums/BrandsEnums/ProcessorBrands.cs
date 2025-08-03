using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum ProcessorBrands
    {
        [StringValue("Intel")]
        Intel,
        [StringValue("AMD")]
        AMD,
        [StringValue("Apple")]
        Apple,
        [StringValue("NIVIDIA")]
        NIVIDIA,
        [StringValue("Qualcomm")]
        Qualcomm
    }
}
