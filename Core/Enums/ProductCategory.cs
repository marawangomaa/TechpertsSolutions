using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum ProductCategory
    {
        [StringValue("Processor")]
        Processor,
        [StringValue("Motherboard")]
        Motherboard,
        [StringValue("CPUCooler")]
        CPUCooler,
        [StringValue("Case")]
        Case,
        [StringValue("GraphicsCard")]
        GraphicsCard,
        [StringValue("RAM")]
        RAM,
        [StringValue("Storage")]
        Storage,
        [StringValue("CaseCooler")]
        CaseCooler,
        [StringValue("PowerSupply")]
        PowerSupply,
        [StringValue("Monitor")]
        Monitor,
        [StringValue("Accessories")]
        Accessories
    }
}
