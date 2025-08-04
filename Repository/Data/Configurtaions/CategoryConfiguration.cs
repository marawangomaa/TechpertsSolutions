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

            // Configure many-to-many relationship through CategorySubCategory
            builder.HasMany(c => c.CategorySubCategories)
                   .WithOne(cs => cs.Category)
                   .HasForeignKey(cs => cs.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Products
            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure unique index on Name
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
