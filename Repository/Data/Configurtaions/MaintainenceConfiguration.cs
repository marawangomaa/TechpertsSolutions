using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace Repository.Data.Configurtaions
{
    public class MaintainenceConfiguration : BaseEntityConfiguration<Maintenance>
    {
        public override void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            base.Configure(builder);
            // Deleting a Customer or TechCompany is restricted if they have maintenance requests.
            builder.HasOne(m => m.Customer)
                   .WithMany(c => c.Maintenances)
                   .HasForeignKey(m => m.CustomerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.TechCompany)
                   .WithMany(tc => tc.Maintenances)
                   .HasForeignKey(m => m.TechCompanyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting a Warranty will set the foreign key on Maintenance to null.
            builder.HasOne(m => m.Warranty)
                   .WithMany()
                   .HasForeignKey(m => m.WarrantyId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Deleting a Maintenance record will cascade to delete its ServiceUsages.
            builder.HasMany(m => m.ServiceUsages)
                   .WithOne(su => su.Maintenance)
                   .HasForeignKey(su => su.MaintenanceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
