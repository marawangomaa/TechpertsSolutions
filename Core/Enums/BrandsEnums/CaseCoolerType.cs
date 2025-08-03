using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum CaseCoolerType
    {
        [StringValue("Air Cooler")]
        AirCooler,

        [StringValue("Liquid Cooler")]
        LiquidCooler,

        [StringValue("All-in-One (AIO)")]
        AIOCooler,

        [StringValue("Passive Cooler")]
        PassiveCooler,

        [StringValue("Custom Loop")]
        CustomLoop
    }
}
