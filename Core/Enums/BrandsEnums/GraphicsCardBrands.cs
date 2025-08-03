using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum GraphicsCardBrands
    {
        [StringValue("NVIDIA")]
        NVIDIA,
        [StringValue("AMD")]
        AMD,
        [StringValue("Intel")]
        Intel,
        [StringValue("ASUS")]
        ASUS,
        [StringValue("MSI")]
        MSI,
        [StringValue("Gigabyte")]
        Gigabyte,
        [StringValue("Zotac")]
        Zotac,
        [StringValue("EVGA")]
        EVGA,
        [StringValue("Colorful")]
        Colorful,
        [StringValue("PowerColor")]
        PowerColor,
        [StringValue("SapphireTechnology")]
        SapphireTechnology
    }
}
