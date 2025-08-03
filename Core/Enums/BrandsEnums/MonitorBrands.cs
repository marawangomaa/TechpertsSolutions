using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum MonitorBrands
    {
        [StringValue("ASUS")]
        ASUS,
        [StringValue("Acer")]
        Acer,
        [StringValue("LG")]
        LG,
        [StringValue("Dell")]
        Dell,
        [StringValue("Samsung")]
        Samsung,
        [StringValue("MSI")]
        MSI,
        [StringValue("BenQ")]
        BenQ,
        [StringValue("HP")]
        HP,
        [StringValue("ViewSonic")]
        ViewSonic,
        [StringValue("AOC")]
        AOC
    }
}
