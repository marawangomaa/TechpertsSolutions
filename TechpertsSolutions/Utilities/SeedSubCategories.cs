using Core.Enums;
using Core.Enums.BrandsEnums;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;
using Core.Utilities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedSubCategories
    {
        public static async Task SeedSubCategoriesAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding subcategories started...");
            var subCategoryRepo = serviceProvider.GetRequiredService<IRepository<SubCategory>>();
            var categoryRepo = serviceProvider.GetRequiredService<IRepository<Category>>();

            // Get existing subcategories to avoid duplicates
            var existingSubCategories = await subCategoryRepo.GetAllAsync();
            var existingNames = existingSubCategories.Where(sc => !string.IsNullOrEmpty(sc.Name)).Select(sc => sc.Name).ToHashSet();

            // Get categories for mapping
            var categories = await categoryRepo.GetAllAsync();
            var categoryMap = categories.ToDictionary(c => c.Name, c => c.Id);

            var subCategoriesToAdd = new List<SubCategory>();

            // Seed Processor Brands
            if (!existingNames.Contains("Intel") || !existingNames.Contains("AMD"))
            {
                var processorBrands = Enum.GetValues(typeof(ProcessorBrands))
                    .Cast<ProcessorBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("Processor"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(processorBrands);
            }

            // Seed Motherboard Brands
            if (!existingNames.Contains("ASUS") || !existingNames.Contains("MSI"))
            {
                var motherboardBrands = Enum.GetValues(typeof(MotherboardBrands))
                    .Cast<MotherboardBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("Motherboard"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(motherboardBrands);
            }

            // Seed Graphics Card Brands
            if (!existingNames.Contains("NVIDIA") || !existingNames.Contains("AMD"))
            {
                var graphicsCardBrands = Enum.GetValues(typeof(GraphicsCardBrands))
                    .Cast<GraphicsCardBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("GraphicsCard"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(graphicsCardBrands);
            }

            // Seed RAM Brands
            if (!existingNames.Contains("Corsair") || !existingNames.Contains("G.Skill"))
            {
                var ramBrands = Enum.GetValues(typeof(RAMBrands))
                    .Cast<RAMBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("RAM"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(ramBrands);
            }

            // Seed Storage Brands
            if (!existingNames.Contains("Samsung") || !existingNames.Contains("Western Digital"))
            {
                var storageBrands = Enum.GetValues(typeof(StorageBrands))
                    .Cast<StorageBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("Storage"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(storageBrands);
            }

            // Seed CPU Cooler Brands
            if (!existingNames.Contains("Cooler Master") || !existingNames.Contains("Noctua"))
            {
                var cpuCoolerBrands = Enum.GetValues(typeof(CpuCoolerBrands))
                    .Cast<CpuCoolerBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("CPUCooler"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(cpuCoolerBrands);
            }

            // Seed Case Brands
            if (!existingNames.Contains("NZXT") || !existingNames.Contains("Cooler Master"))
            {
                var caseBrands = Enum.GetValues(typeof(CaseBrands))
                    .Cast<CaseBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("Case"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(caseBrands);
            }

            // Seed Case Cooler Brands
            if (!existingNames.Contains("NZXT") || !existingNames.Contains("Cooler Master"))
            {
                var caseCoolerBrands = Enum.GetValues(typeof(CaseCoolerBrands))
                    .Cast<CaseCoolerBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("CaseCooler"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(caseCoolerBrands);
            }

            // Seed Power Supply Brands
            if (!existingNames.Contains("Corsair") || !existingNames.Contains("EVGA"))
            {
                var powerSupplyBrands = Enum.GetValues(typeof(PowerSupplyBrands))
                    .Cast<PowerSupplyBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("PowerSupply"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(powerSupplyBrands);
            }

            // Seed Monitor Brands
            if (!existingNames.Contains("ASUS") || !existingNames.Contains("Acer"))
            {
                var monitorBrands = Enum.GetValues(typeof(MonitorBrands))
                    .Cast<MonitorBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("Monitor"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(monitorBrands);
            }

            // Seed Accessory Brands
            if (!existingNames.Contains("Logitech") || !existingNames.Contains("Razer"))
            {
                var accessoryBrands = Enum.GetValues(typeof(AccessoryBrands))
                    .Cast<AccessoryBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("Accessories"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(accessoryBrands);
            }

            // Seed Pre-built PC Brands
            if (!existingNames.Contains("CyberPowerPC") || !existingNames.Contains("iBUYPOWER"))
            {
                var prebuiltPcBrands = Enum.GetValues(typeof(PrebuiltPcBrands))
                    .Cast<PrebuiltPcBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("PreBuildPC"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(prebuiltPcBrands);
            }

            // Seed Laptop Brands
            if (!existingNames.Contains("Dell") || !existingNames.Contains("HP"))
            {
                var laptopBrands = Enum.GetValues(typeof(LaptopBrands))
                    .Cast<LaptopBrands>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        CategoryId = categoryMap.GetValueOrDefault("Laptop"),
                        Image = null
                    });
                subCategoriesToAdd.AddRange(laptopBrands);
            }

            // Add all subcategories to database
            foreach (var subCategory in subCategoriesToAdd)
            {
                Console.WriteLine($"Adding subcategory: {subCategory.Name} to category: {subCategory.CategoryId}");
                await subCategoryRepo.AddAsync(subCategory);
            }

            if (subCategoriesToAdd.Count > 0)
            {
                await subCategoryRepo.SaveChangesAsync();
                Console.WriteLine($"Added {subCategoriesToAdd.Count} subcategories.");
            }
            else
            {
                Console.WriteLine("No new subcategories to add.");
            }

            Console.WriteLine("Seeding subcategories completed.");
        }
    }
} 