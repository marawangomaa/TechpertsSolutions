using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Repository.Data.Configurtaions
{
    public class MaintainenceConfiguration : IEntityTypeConfiguration<Maintenance>
    {
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
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

            builder.HasOne(m => m.Warranty)
                   .WithMany()
                   .HasForeignKey(m => m.WarrantyId)
                   .OnDelete(DeleteBehavior.SetNull); 

            builder.HasMany(m => m.ServiceUsages)
                   .WithOne(su => su.Maintenance)
                   .HasForeignKey(su => su.MaintenanceId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
