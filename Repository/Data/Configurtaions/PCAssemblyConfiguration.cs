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
    public class PCAssemblyConfiguration : IEntityTypeConfiguration<PCAssembly>
    {
        public void Configure(EntityTypeBuilder<PCAssembly> builder)
        {
            builder.HasOne(pc => pc.Customer)
                   .WithMany(c => c.PCAssembly)
                   .HasForeignKey(pc => pc.CustomerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(pc => pc.ServiceUsage)
                   .WithMany(su => su.PCAssemblies)
                   .HasForeignKey(pc => pc.ServiceUsageId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(pc => pc.PCAssemblyItems)
                   .WithOne(pci => pci.PCAssembly)
                   .HasForeignKey(pci => pci.PCAssemblyId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
