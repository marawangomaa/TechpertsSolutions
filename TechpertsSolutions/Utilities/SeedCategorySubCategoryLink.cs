using Core.Enums;
using Core.Enums.BrandsEnums;
using Core.Interfaces;
using Core.Utilities;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedCategorySubCategoryLink
    {
        public static async Task SeedLinksAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding category-subcategory links started...");

            var categoryRepo = serviceProvider.GetRequiredService<IRepository<Category>>();
            var subCategoryRepo = serviceProvider.GetRequiredService<IRepository<SubCategory>>();
            var linkRepo = serviceProvider.GetRequiredService<IRepository<CategorySubCategory>>();

            var categories = await categoryRepo.GetAllAsync();
            var subCategories = await subCategoryRepo.GetAllAsync();
            var existingLinks = await linkRepo.GetAllAsync();

            var existingPairs = new HashSet<(string, string)>(existingLinks.Select(l => (l.CategoryId, l.SubCategoryId)));

            foreach (ProductCategory categoryEnum in Enum.GetValues(typeof(ProductCategory)))
            {
                var categoryName = categoryEnum.GetStringValue();
                var categoryEntity = categories.FirstOrDefault(c => c.Name == categoryName);
                if (categoryEntity == null) continue;

                var subCategoryNames = GetSubCategoryNamesForCategory(categoryEnum);

                foreach (var subCatName in subCategoryNames)
                {
                    var subCategoryEntity = subCategories.FirstOrDefault(sc => sc.Name == subCatName);
                    if (subCategoryEntity == null) continue;

                    var pair = (categoryEntity.Id, subCategoryEntity.Id);
                    if (!existingPairs.Contains(pair))
                    {
                        var link = new CategorySubCategory
                        {
                            Id = Guid.NewGuid().ToString(),
                            CategoryId = categoryEntity.Id,
                            SubCategoryId = subCategoryEntity.Id
                        };

                        await linkRepo.AddAsync(link);
                        Console.WriteLine($"Linked {categoryName} with {subCatName}");
                    }
                }
            }

            await linkRepo.SaveChangesAsync();
            Console.WriteLine("Seeding category-subcategory links completed.");
        }

        private static List<string> GetSubCategoryNamesForCategory(ProductCategory category)
        {
            return category switch
            {
                ProductCategory.Processor => Enum.GetValues(typeof(ProcessorBrands)).Cast<ProcessorBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.Motherboard => Enum.GetValues(typeof(MotherboardBrands)).Cast<MotherboardBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.GraphicsCard => Enum.GetValues(typeof(GraphicsCardBrands)).Cast<GraphicsCardBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.RAM => Enum.GetValues(typeof(RAMBrands)).Cast<RAMBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.Storage => Enum.GetValues(typeof(StorageBrands)).Cast<StorageBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.CPUCooler => Enum.GetValues(typeof(CpuCoolerBrands)).Cast<CpuCoolerBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.Case => Enum.GetValues(typeof(CaseBrands)).Cast<CaseBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.CaseCooler => Enum.GetValues(typeof(CaseCoolerBrands)).Cast<CaseCoolerBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.PowerSupply => Enum.GetValues(typeof(PowerSupplyBrands)).Cast<PowerSupplyBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.Monitor => Enum.GetValues(typeof(MonitorBrands)).Cast<MonitorBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.Accessories => Enum.GetValues(typeof(AccessoryBrands)).Cast<AccessoryBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.PreBuildPC => Enum.GetValues(typeof(PrebuiltPcBrands)).Cast<PrebuiltPcBrands>().Select(b => b.GetStringValue()).ToList(),
                ProductCategory.Laptop => Enum.GetValues(typeof(LaptopBrands)).Cast<LaptopBrands>().Select(b => b.GetStringValue()).ToList(),
                _ => new List<string>()
            };
        }
    }
}

