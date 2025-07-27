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
                   .OnDelete(DeleteBehavior.Cascade); // Delete PC assembly item when PC assembly is deleted

            builder.HasOne(pci => pci.Cart)
                   .WithMany(c => c.PCAssemblyItems)
                   .HasForeignKey(pci => pci.CartId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // Delete PC assembly item when cart is deleted

            builder.HasOne(pci => pci.Product)
                   .WithMany(p => p.PCAssemblyItems)
                   .HasForeignKey(pci => pci.ProductId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); // Prevent product deletion if it's in PC assembly items
        }
    }
}
