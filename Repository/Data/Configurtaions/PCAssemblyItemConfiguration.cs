using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data.Configurtaions;

public class PCAssemblyItemConfiguration : BaseEntityConfiguration<PCAssemblyItem>
{
    public override void Configure(EntityTypeBuilder<PCAssemblyItem> builder)
    {
        base.Configure(builder);

        builder.HasOne(pci => pci.PCAssembly)
               .WithMany(pc => pc.PCAssemblyItems)
               .HasForeignKey(pci => pci.PCAssemblyId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);  // ok

        builder.HasOne(pci => pci.Product)
               .WithMany(p => p.PCAssemblyItems)
               .HasForeignKey(pci => pci.ProductId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);  // prevents cascade conflicts
    }
}