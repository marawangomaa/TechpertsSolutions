using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;
using Core.Utilities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedCategories
    {
        public static async Task SeedCategoriesAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding categories started...");
            var categoryRepo = serviceProvider.GetRequiredService<IRepository<Category>>();

            var existingCategories = await categoryRepo.GetAllAsync();
            var existingNames = existingCategories.Where(c => !string.IsNullOrEmpty(c.Name)).Select(c => c.Name).ToHashSet();


            var categoryImageMap = new Dictionary<ProductCategory, string>
            {
                { ProductCategory.UnCategorized, "uncategorized.png" },
                { ProductCategory.Processor, "processor.png" },
                { ProductCategory.Motherboard, "motherboard.png" },
                { ProductCategory.CPUCooler, "cpu-cooler.png" },
                { ProductCategory.Case, "case.png" },
                { ProductCategory.GraphicsCard, "graphics-card.png" },
                { ProductCategory.RAM, "ram.png" },
                { ProductCategory.Storage, "storage.png" },
                { ProductCategory.CaseCooler, "case-cooler.png" },
                { ProductCategory.PowerSupply, "power-supply.png" },
                { ProductCategory.Monitor, "monitor.png" },
                { ProductCategory.Accessories, "accessories.png" },
                { ProductCategory.PreBuildPC, "pre-build-pc.png" },
                { ProductCategory.Laptop, "laptop.png" }
            };

            var categories = Enum.GetValues(typeof(ProductCategory))
                .Cast<ProductCategory>()
                .Where(categoryEnum => !existingNames.Contains(categoryEnum.GetStringValue()))
                .Select(categoryEnum => new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = categoryEnum.GetStringValue(),
                    Description = $"{categoryEnum.GetStringValue()} related products",
                    Image = $"/assets/categories/{categoryImageMap[categoryEnum]}"
                }).ToList();

            foreach (var category in categories)
            {
                Console.WriteLine($"Adding category: {category.Name}");
                await categoryRepo.AddAsync(category);
            }

            if (categories.Count > 0)
                await categoryRepo.SaveChangesAsync();
            else
                Console.WriteLine("No new categories to add.");

            Console.WriteLine("Seeding categories completed.");
        }
    }
}
