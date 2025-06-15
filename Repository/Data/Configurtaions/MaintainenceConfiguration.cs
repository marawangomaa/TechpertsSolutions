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
            builder.HasOne(n => n.Customer)
                   .WithMany(c => c.Maintenances)
                    .HasForeignKey(m => m.CustomerId).IsRequired();

            builder.HasOne(n => n.TechCompany)
                    .WithMany(c => c.Maintenances)
                        .HasForeignKey(m => m.TechCompanyId).IsRequired();

            builder.HasOne(m => m.Warranty)
                    .WithOne(w => w.Maintenance)
                        .HasForeignKey<Maintenance>(n => n.WarrantyId);
        }
    }
}
