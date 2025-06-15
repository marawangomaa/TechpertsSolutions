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
    public class PCAssemblyItemConfiguration : IEntityTypeConfiguration<PCAssemblyItem>
    {
        public void Configure(EntityTypeBuilder<PCAssemblyItem> builder)
        {
            builder.HasKey(pai => new { pai.PCAssemblyId, pai.ProductId });

            builder.HasOne(pi => pi.PCAssembly)
                   .WithMany(pc => pc.PCAssemblyItems)
                   .HasForeignKey(pai => pai.PCAssemblyId)
                   .IsRequired();

            builder.HasOne(pi => pi.Cart)
                   .WithMany(c => c.PCAssemblyItems)
                   .HasForeignKey(pai => pai.CartId)
                   .IsRequired();

            builder.HasOne(pi => pi.Product)
                   .WithOne(p => p.PCAssemblyItem)
                   .HasForeignKey<PCAssemblyItem>(pai => pai.ProductId)
                   .IsRequired();
        }
    }
}
