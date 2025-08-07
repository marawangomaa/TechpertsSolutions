using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data.Configurtaions
{
    public class SpecificationConfiguration : BaseEntityConfiguration<Specification>
    {
        public override void Configure(EntityTypeBuilder<Specification> builder)
        {
            base.Configure(builder);
            // Deleting a Product will cascade to delete its Specifications.
            builder.HasOne(s => s.Product)
                .WithMany(p => p.Specifications)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
