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
            // Composite key for the join table.
            builder.HasKey(cs => new { cs.CategoryId, cs.SubCategoryId });

            // Deleting a Category will cascade to delete the join table entries.
            builder.HasOne(cs => cs.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(cs => cs.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Deleting a SubCategory will cascade to delete the join table entries.
            builder.HasOne(cs => cs.SubCategory)
                .WithMany(sc => sc.CategorySubCategories)
                .HasForeignKey(cs => cs.SubCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Default value for AssignedAt.
            builder.Property(cs => cs.AssignedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
} 