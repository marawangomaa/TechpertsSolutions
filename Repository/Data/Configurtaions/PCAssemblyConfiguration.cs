using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace Repository.Data.Configurtaions
{
    public class PCAssemblyConfiguration : BaseEntityConfiguration<PCAssembly>
    {
        public override void Configure(EntityTypeBuilder<PCAssembly> builder)
        {
            base.Configure(builder);
            // Deleting a Customer is restricted if they have an existing PCAssembly.
            builder.HasOne(pc => pc.Customer)
                   .WithMany(c => c.PCAssembly)
                   .HasForeignKey(pc => pc.CustomerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting a ServiceUsage is restricted if it's linked to a PCAssembly.
            builder.HasOne(pc => pc.ServiceUsage)
                   .WithMany(su => su.PCAssemblies)
                   .HasForeignKey(pc => pc.ServiceUsageId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting a PCAssembly will cascade to delete all its items.
            builder.HasMany(pc => pc.PCAssemblyItems)
                   .WithOne(pci => pci.PCAssembly)
                   .HasForeignKey(pci => pci.PCAssemblyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
