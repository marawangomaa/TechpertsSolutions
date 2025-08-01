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

            builder.HasMany(c => c.SubCategories)
                   .WithOne(sc => sc.Category)
                   .HasForeignKey(sc => sc.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
