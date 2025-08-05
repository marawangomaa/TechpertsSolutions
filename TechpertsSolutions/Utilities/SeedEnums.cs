using Core.Enums;
using Core.Enums.BrandsEnums;
using Core.Utilities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedEnums
    {
        public static void LogEnumValues()
        {
            Console.WriteLine("=== Enum Values Available ===");

            LogEnum<ProductPendingStatus>("ProductPendingStatus");
            LogEnum<ServiceType>("ServiceType");
            LogEnum<ProductCategory>("ProductCategory");
            LogEnum<RoleType>("RoleType");

            // Add all brand enums used for subcategory seeding:
            LogEnum<ProcessorBrands>("ProcessorBrands");
            LogEnum<MotherboardBrands>("MotherboardBrands");
            LogEnum<GraphicsCardBrands>("GraphicsCardBrands");
            LogEnum<RAMBrands>("RAMBrands");
            LogEnum<StorageBrands>("StorageBrands");
            LogEnum<CpuCoolerBrands>("CpuCoolerBrands");
            LogEnum<CaseBrands>("CaseBrands");
            LogEnum<CaseCoolerBrands>("CaseCoolerBrands");
            LogEnum<PowerSupplyBrands>("PowerSupplyBrands");
            LogEnum<MonitorBrands>("MonitorBrands");
            LogEnum<AccessoryBrands>("AccessoryBrands");
            LogEnum<PrebuiltPcBrands>("PrebuiltPcBrands");
            LogEnum<LaptopBrands>("LaptopBrands");

            Console.WriteLine("=== Enum Seeding Completed ===");
        }

        private static void LogEnum<TEnum>(string enumName) where TEnum : Enum
        {
            Console.WriteLine($"\n{enumName} Values:");
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                string display = value.ToString();
                string strValue = value.GetStringValue();
                Console.WriteLine($"  {display} = {strValue}");
            }
        }
    }
} 
