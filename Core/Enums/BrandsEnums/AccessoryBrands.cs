using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum AccessoryBrands
    {
        [StringValue("Logitech")]
        Logitech,
        [StringValue("Razer")]
        Razer,
        [StringValue("Corsair")]
        Corsair,
        [StringValue("SteelSeries")]
        SteelSeries,
        [StringValue("Redragon")]
        Redragon,
        [StringValue("HyperX")]
        HyperX,
        [StringValue("Glorious")]
        Glorious,
        [StringValue("ROCCAT")]
        ROCCAT,
        [StringValue("Elgato")]
        Elgato
    }
}
