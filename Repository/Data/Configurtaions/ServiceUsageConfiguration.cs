using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data.Configurtaions
{
    public class ServiceUsageConfiguration : IEntityTypeConfiguration<ServiceUsage>
    {
        public void Configure(EntityTypeBuilder<ServiceUsage> builder)
        {
            builder.HasOne(su => su.Maintenance)
                   .WithMany(m => m.ServiceUsages)
                   .HasForeignKey(su => su.MaintenanceId)
                   .OnDelete(DeleteBehavior.Cascade); 

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
