using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum PowerSupplyBrands
    {
        [StringValue("Corsair")]
        Corsair,
        [StringValue("EVGA")]
        EVGA,
        [StringValue("Seasonic")]
        Seasonic,
        [StringValue("CoolerMaster")]
        CoolerMaster,
        [StringValue("Thermaltake")]
        Thermaltake,
        [StringValue("BeQuiet")]
        BeQuiet,
        [StringValue("SuperFlower")]
        SuperFlower
    }
}
