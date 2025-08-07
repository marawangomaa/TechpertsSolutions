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
    public class PCAssemblyItemConfiguration : BaseEntityConfiguration<PCAssemblyItem>
    {
        public override void Configure(EntityTypeBuilder<PCAssemblyItem> builder)
        {
            base.Configure(builder);
            // Deleting a PCAssembly will cascade to delete its items.
            builder.HasOne(pci => pci.PCAssembly)
                   .WithMany(pc => pc.PCAssemblyItems)
                   .HasForeignKey(pci => pci.PCAssemblyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // Deleting a Product is restricted if it's in a PCAssemblyItem.
            builder.HasOne(pci => pci.Product)
                   .WithMany(p => p.PCAssemblyItems)
                   .HasForeignKey(pci => pci.ProductId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
