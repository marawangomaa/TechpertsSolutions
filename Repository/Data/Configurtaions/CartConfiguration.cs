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
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasOne(c => c.Customer)
                   .WithOne(ct => ct.Cart)
                   .HasForeignKey<Cart>(c => c.CustomerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.PCAssemblyItems)
                   .WithOne(pai => pai.Cart)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
