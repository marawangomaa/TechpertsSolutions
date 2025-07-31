using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ProductCategory
    {
        [StringValue("None")]
        None = -1,
        
        [StringValue("Processor")]
        Processor = 0,
        
        [StringValue("Motherboard")]
        Motherboard = 1,
        
        [StringValue("CPUCooler")]
        CPUCooler = 2,
        
        [StringValue("Case")]
        Case = 3,
        
        [StringValue("GraphicsCard")]
        GraphicsCard = 4,
        
        [StringValue("RAM")]
        RAM = 5,
        
        [StringValue("Storage")]
        Storage = 6,
        
        [StringValue("CaseCooler")]
        CaseCooler = 7,
        
        [StringValue("PowerSupply")]
        PowerSupply = 8,
        
        [StringValue("Monitor")]
        Monitor = 9,
        
        [StringValue("Accessories")]
        Accessories = 10,
        
        [StringValue("PreBuildPC")]
        PreBuildPC = 11,
        
        [StringValue("Laptop")]
        Laptop = 12
    }
}
