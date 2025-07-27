using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data.Configurtaions
{
    public class SubCategoryConfiguration : IEntityTypeConfiguration<SubCategory>
    {
        public void Configure(EntityTypeBuilder<SubCategory> builder)
        {
            builder.HasOne(sc => sc.Category)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(sc => sc.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade); // Delete subcategory when category is deleted

            builder.HasMany(sc => sc.Products)
                   .WithOne(p => p.SubCategory)
                   .HasForeignKey(p => p.SubCategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent subcategory deletion if it has products
        }
    }
}
