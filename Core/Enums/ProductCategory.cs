using Core.Enums.Attributes;

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
        Accessories,
        [StringValue("PreBuildPC")]
        PreBuildPC,
        [StringValue("Laptop")]
        Laptop
    }
}
