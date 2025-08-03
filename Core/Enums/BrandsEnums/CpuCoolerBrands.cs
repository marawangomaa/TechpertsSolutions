using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum CpuCoolerBrands
    {
        [StringValue("CoolerMaster")]
        CoolerMaster,
        [StringValue("Noctua")]
        Noctua,
        [StringValue("DeepCool")]
        DeepCool,
        [StringValue("Corsair")]
        Corsair,
        [StringValue("BeQuiet")]
        BeQuiet,
        [StringValue("NZXT")]
        NZXT,
        [StringValue("ARCTIC")]
        ARCTIC,
        [StringValue("LianLi")]
        LianLi,
        [StringValue("EKWB")]
        EKWB
    }
}
