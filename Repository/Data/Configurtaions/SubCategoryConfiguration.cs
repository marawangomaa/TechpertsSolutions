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
    public class SubCategoryConfiguration : BaseEntityConfiguration<SubCategory>
    {
        public override void Configure(EntityTypeBuilder<SubCategory> builder)
        {
            base.Configure(builder);

            // Configure direct relationship with Category (many-to-one)
            builder.HasOne(sc => sc.Category)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(sc => sc.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull); // Set to null instead of cascade

            // Configure many-to-many relationship through CategorySubCategory
            builder.HasMany(sc => sc.CategorySubCategories)
                   .WithOne(cs => cs.SubCategory)
                   .HasForeignKey(cs => cs.SubCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Products
            builder.HasMany(sc => sc.Products)
                   .WithOne(p => p.SubCategory)
                   .HasForeignKey(p => p.SubCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure unique index on Name
            builder.HasIndex(sc => sc.Name).IsUnique();
        }
    }
}
