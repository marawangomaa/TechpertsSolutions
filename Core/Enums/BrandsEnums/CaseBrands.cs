using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum CaseBrands
    {
        [StringValue("NZXT")]
        NZXT,
        [StringValue("CoolerMaster")]
        CoolerMaster,
        [StringValue("Corsair")]
        Corsair,
        [StringValue("Thermaltake")]
        Thermaltake,
        [StringValue("LianLi")]
        LianLi,
        [StringValue("FractalDesign")]
        FractalDesign,
        [StringValue("BeQuiet")]
        BeQuiet,
        [StringValue("Phanteks")]
        Phanteks,
        [StringValue("Silverstone")]
        Silverstone,
        [StringValue("Antec")]
        Antec
    }
}
