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
    public class CategoryConfiguration : BaseEntityConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);
            // The many-to-many relationship with SubCategory is handled in CategorySubCategoryConfiguration.
            builder.HasMany(c => c.SubCategories)
                   .WithOne(cs => cs.Category)
                   .HasForeignKey(cs => cs.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Deleting a Category is restricted if it contains Products.
            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Category names must be unique.
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
