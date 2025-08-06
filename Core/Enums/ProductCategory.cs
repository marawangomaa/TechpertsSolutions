using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ProductCategory
    {
        [StringValue("UnCategorized")]
        UnCategorized = 0,

        [StringValue("Processor")]
        Processor = 1,
        
        [StringValue("Motherboard")]
        Motherboard = 2,
        
        [StringValue("CPUCooler")]
        CPUCooler = 3,
        
        [StringValue("Case")]
        Case = 4,
        
        [StringValue("GraphicsCard")]
        GraphicsCard = 5,
        
        [StringValue("RAM")]
        RAM = 6,
        
        [StringValue("Storage")]
        Storage = 7,

        [StringValue("CaseCooler")]
        CaseCooler = 8,
        
        [StringValue("PowerSupply")]
        PowerSupply = 9,
        
        [StringValue("Monitor")]
        Monitor = 10,
        
        [StringValue("Accessories")]
        Accessories = 11,
        
        [StringValue("PreBuildPC")]
        PreBuildPC = 12,

        [StringValue("Laptop")]
        Laptop = 13
    }
}
