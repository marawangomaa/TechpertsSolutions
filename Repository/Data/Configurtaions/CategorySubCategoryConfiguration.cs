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
    public class CategorySubCategoryConfiguration : IEntityTypeConfiguration<CategorySubCategory>
    {
        public void Configure(EntityTypeBuilder<CategorySubCategory> builder)
        {
            // Configure as many-to-many relationship table
            builder.HasKey(cs => new { cs.CategoryId, cs.SubCategoryId });

            // Configure relationship with Category
            builder.HasOne(cs => cs.Category)
                   .WithMany(c => c.CategorySubCategories)
                   .HasForeignKey(cs => cs.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with SubCategory
            builder.HasOne(cs => cs.SubCategory)
                   .WithMany(sc => sc.CategorySubCategories)
                   .HasForeignKey(cs => cs.SubCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure AssignedAt as required
            builder.Property(cs => cs.AssignedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");
        }
    }
} 