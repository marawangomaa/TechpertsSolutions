using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum CaseCoolerBrands
    {
        [StringValue("NZXT")]
        NZXT,
        [StringValue("Cooler Master")]
        CoolerMaster,
        [StringValue("Corsair")]
        Corsair,
        [StringValue("Thermaltake")]
        Thermaltake,
        [StringValue("Lian Li")]
        LianLi,
        [StringValue("Fractal Design")]
        FractalDesign,
        [StringValue("be quiet!")]
        BeQuiet,
        [StringValue("Phanteks")]
        Phanteks,
        [StringValue("Silverstone")]
        Silverstone,
        [StringValue("Antec")]
        Antec
    }
} 