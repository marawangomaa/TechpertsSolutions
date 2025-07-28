using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data.Configurtaions
{
    public class SpecificationConfiguration : IEntityTypeConfiguration<Specification>
    {
        public void Configure(EntityTypeBuilder<Specification> builder)
        {
            builder.HasOne(s => s.Product)
                   .WithMany(p => p.Specifications)
                   .HasForeignKey(s => s.ProductId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
