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

            builder.HasOne(pi => pi.PCAssembly)
                   .WithMany(pc => pc.PCAssemblyItems)
                   .HasForeignKey(pai => pai.PCAssemblyId)
                   .IsRequired();

            builder.HasOne(pi => pi.Cart)
                   .WithMany(c => c.PCAssemblyItems)
                   .HasForeignKey(pai => pai.CartId)
                   .IsRequired();

            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.PCAssemblyItems)
                   .HasForeignKey(pai => pai.ProductId)
                   .IsRequired();
        }
    }
}
