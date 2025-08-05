using Core.Enums;
using Core.Enums.BrandsEnums;
using Core.Interfaces;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedSubCategories
    {
        public static async Task<List<SubCategory>> SeedSubCategoriesAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding subcategories started...");
            var subCategoryRepo = serviceProvider.GetRequiredService<IRepository<SubCategory>>();

            var existingSubCategories = await subCategoryRepo.GetAllAsync();
            var existingNames = existingSubCategories.Where(sc => !string.IsNullOrEmpty(sc.Name)).Select(sc => sc.Name).ToHashSet();

            var allSubCategories = new List<SubCategory>();

            // Helper method to process enums
            void AddSubCategoriesFromEnum<T>() where T : Enum
            {
                var subCats = Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Where(brand => !existingNames.Contains(brand.GetStringValue()))
                    .Select(brand => new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = brand.GetStringValue(),
                        Image = null
                    });
                allSubCategories.AddRange(subCats);
            }

            AddSubCategoriesFromEnum<ProcessorBrands>();
            AddSubCategoriesFromEnum<MotherboardBrands>();
            AddSubCategoriesFromEnum<GraphicsCardBrands>();
            AddSubCategoriesFromEnum<RAMBrands>();
            AddSubCategoriesFromEnum<StorageBrands>();
            AddSubCategoriesFromEnum<CpuCoolerBrands>();
            AddSubCategoriesFromEnum<CaseBrands>();
            AddSubCategoriesFromEnum<CaseCoolerBrands>();
            AddSubCategoriesFromEnum<PowerSupplyBrands>();
            AddSubCategoriesFromEnum<MonitorBrands>();
            AddSubCategoriesFromEnum<AccessoryBrands>();
            AddSubCategoriesFromEnum<PrebuiltPcBrands>();
            AddSubCategoriesFromEnum<LaptopBrands>();

            foreach (var subCategory in allSubCategories)
            {
                Console.WriteLine($"Adding subcategory: {subCategory.Name}");
                await subCategoryRepo.AddAsync(subCategory);
            }

            if (allSubCategories.Count > 0)
            {
                await subCategoryRepo.SaveChangesAsync();
                Console.WriteLine($"Added {allSubCategories.Count} subcategories.");
            }
            else
            {
                Console.WriteLine("No new subcategories to add.");
            }

            Console.WriteLine("Seeding subcategories completed.");
            return allSubCategories;
        }
    }
}
