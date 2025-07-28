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
    public class PCAssemblyItemConfiguration : IEntityTypeConfiguration<PCAssemblyItem>
    {
        public void Configure(EntityTypeBuilder<PCAssemblyItem> builder)
        {
            builder.HasOne(pci => pci.PCAssembly)
                   .WithMany(pc => pc.PCAssemblyItems)
                   .HasForeignKey(pci => pci.PCAssemblyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(pci => pci.Product)
                   .WithMany(p => p.PCAssemblyItems)
                   .HasForeignKey(pci => pci.ProductId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
