using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace Repository.Data.Configurtaions
{
    public class ServiceUsageConfiguration : BaseEntityConfiguration<ServiceUsage>
    {
        public override void Configure(EntityTypeBuilder<ServiceUsage> builder)
        {
            base.Configure(builder);
            
            builder.Property(su => su.ServiceType)
            .HasConversion<string>();

            // Deleting a Maintenance record will cascade to delete its associated ServiceUsages.
            builder.HasOne(su => su.Maintenance)
                .WithMany(m => m.ServiceUsages)
                .HasForeignKey(su => su.MaintenanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Deleting a ServiceUsage is restricted if it's linked to Orders or PCAssemblies.
            builder.HasMany(su => su.Orders)
                .WithOne(o => o.ServiceUsage)
                .HasForeignKey(o => o.ServiceUsageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(su => su.PCAssemblies)
                .WithOne(pca => pca.ServiceUsage)
                .HasForeignKey(pca => pca.ServiceUsageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
