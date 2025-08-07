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
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            // Deleting a Category, SubCategory, or TechCompany is restricted if products are associated with them.
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.SubCategory)
                .WithMany(sc => sc.Products)
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.TechCompany)
                .WithMany(tc => tc.Products)
                .HasForeignKey(p => p.TechCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Deleting a Product will cascade to delete its Specifications.
            builder.HasMany(p => p.Specifications)
                .WithOne(s => s.Product)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Deleting a Product will cascade to delete its Warranties.
            builder.HasMany(p => p.Warranties)
                .WithOne(w => w.Product)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
