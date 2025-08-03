using Core.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.BrandsEnums
{
    public enum MotherboardBrands
    {
        [StringValue("ASUS")]
        ASUS,
        [StringValue("MSI")]
        MSI,
        [StringValue("Gigabyte")]
        Gigabyte,
        [StringValue("ASRock")]
        ASRock,
        [StringValue("BioStar")]
        BioStar,
        [StringValue("EVGA")]
        EVGA
    }
}
