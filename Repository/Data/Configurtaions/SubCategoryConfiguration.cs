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
            // This many-to-many relationship with Category is handled in CategorySubCategoryConfiguration.
            builder.HasMany(sc => sc.CategorySubCategories)
                   .WithOne(cs => cs.SubCategory)
                   .HasForeignKey(cs => cs.SubCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Deleting a SubCategory is restricted if it contains Products.
            builder.HasMany(sc => sc.Products)
                   .WithOne(p => p.SubCategory)
                   .HasForeignKey(p => p.SubCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

         
        }
    }
}
